using Lexia.Studio.Application;
using Lexia.Studio.Application.Abstractions;
using Lexia.Studio.Application.Journals;
using Lexia.Studio.Application.PuzzleBooks;
using Lexia.Studio.Domain.Puzzles;
using Lexia.Studio.Infrastructure;
using Lexia.BuildingBlocks.Application.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using PdfSharp.Pdf.IO;

namespace Lexia.Studio.UnitTests.Interiors;

/// <summary>Runs the real handlers, decorators and PDF renderer, then reads the PDF back.</summary>
public sealed class InteriorGenerationTests
{
    private static ServiceProvider Services()
    {
        var services = new ServiceCollection();
        services.AddSingleton(typeof(Microsoft.Extensions.Logging.ILogger<>), typeof(NullLogger<>));
        services.AddStudioApplication();
        services.AddStudioInfrastructure();
        return services.BuildServiceProvider();
    }

    [Theory]
    [InlineData(PuzzleType.Sudoku, 30, true)]
    [InlineData(PuzzleType.Maze, 4, false)]
    [InlineData(PuzzleType.WordSearch, 5, true)]
    public async Task Puzzle_books_are_padded_to_an_even_count_of_at_least_24_pages(PuzzleType type, int count, bool solutions)
    {
        using var provider = Services();
        using var scope = provider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<GeneratePuzzleBookCommand, GeneratedFile>>();
        WordListInput[] lists = [new("Fruit", ["apple", "banana", "cherry", "grape", "mango"])];

        var result = await handler.Handle(
            new GeneratePuzzleBookCommand("Test Book", "Author", "8.5x11", type, Difficulty.Easy, count, lists, solutions, Seed: 1),
            CancellationToken.None);

        var file = result.Value;
        file.ContentType.ShouldBe("application/pdf");
        file.FileName.ShouldBe("test-book-interior.pdf");
        using var pdf = PdfReader.Open(new MemoryStream(file.Content), PdfDocumentOpenMode.Import);
        pdf.PageCount.ShouldBe(file.PageCount);
        pdf.PageCount.ShouldBeGreaterThanOrEqualTo(24);
        (pdf.PageCount % 2).ShouldBe(0);
        pdf.Pages[0].Width.Inch.ShouldBe(8.5, tolerance: 0.001);
        pdf.Pages[0].Height.Inch.ShouldBe(11, tolerance: 0.001);
    }

    [Fact]
    public async Task Journals_have_the_requested_page_count_at_trim_size()
    {
        using var provider = Services();
        using var scope = provider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<GenerateJournalCommand, GeneratedFile>>();

        var file = (await handler.Handle(new GenerateJournalCommand("Notes", null, "6x9", JournalStyle.Lined, 120), CancellationToken.None)).Value;

        using var pdf = PdfReader.Open(new MemoryStream(file.Content), PdfDocumentOpenMode.Import);
        pdf.PageCount.ShouldBe(120);
        pdf.Pages[0].Width.Inch.ShouldBe(6, tolerance: 0.001);
    }

    [Fact]
    public async Task Invalid_requests_return_validation_errors_instead_of_files()
    {
        using var provider = Services();
        using var scope = provider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<GeneratePuzzleBookCommand, GeneratedFile>>();

        var result = await handler.Handle(
            new GeneratePuzzleBookCommand("", null, "nope", PuzzleType.WordSearch, Difficulty.Easy, 0, null, true, null),
            CancellationToken.None);

        var error = result.Error.ShouldBeOfType<Lexia.BuildingBlocks.Domain.ValidationError>();
        error.Errors.Keys.ShouldBe(["Title", "TrimSizeId", "PuzzleCount", "WordLists"], ignoreOrder: true);
    }
}
