using Lexia.Books.Application;
using Lexia.Books.Application.Abstractions;
using Lexia.Books.Application.Books;
using Lexia.Books.Domain.Books;
using Lexia.BuildingBlocks.Application.Abstractions;
using Lexia.BuildingBlocks.Application.Messaging;
using Lexia.BuildingBlocks.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace Lexia.Books.UnitTests;

public sealed class BookHandlerTests
{
    private static readonly Guid Author = Guid.NewGuid();
    private readonly IBookRepository _books = Substitute.For<IBookRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private ServiceProvider Services()
    {
        var user = Substitute.For<ICurrentUser>();
        user.Id.Returns(Author);
        var clock = Substitute.For<IDateTimeProvider>();
        clock.UtcNow.Returns(new DateTimeOffset(2026, 10, 23, 9, 0, 0, TimeSpan.Zero));

        var services = new ServiceCollection();
        services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
        services.AddSingleton(user);
        services.AddSingleton(clock);
        services.AddSingleton(_books);
        services.AddSingleton(_unitOfWork);
        services.AddBooksApplication();
        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task Creating_a_book_saves_it_for_the_signed_in_author()
    {
        using var provider = Services();
        var handler = provider.CreateScope().ServiceProvider.GetRequiredService<ICommandHandler<CreateBookCommand, BookDto>>();

        var result = await handler.Handle(new CreateBookCommand("Space Mazes", BookType.Maze, "Mansi"), CancellationToken.None);

        result.Value.AuthorName.ShouldBe("Mansi");
        _books.Received(1).Add(Arg.Is<Book>(b => b.OwnerId == Author && b.Title == "Space Mazes"));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Invalid_commands_are_rejected_before_the_handler_runs()
    {
        using var provider = Services();
        var handler = provider.CreateScope().ServiceProvider.GetRequiredService<ICommandHandler<CreateBookCommand, BookDto>>();

        var result = await handler.Handle(new CreateBookCommand("", BookType.Maze, null), CancellationToken.None);

        result.Error.ShouldBeOfType<ValidationError>().Errors.Keys.ShouldContain("Title");
        _books.DidNotReceiveWithAnyArgs().Add(default!);
    }

    [Fact]
    public async Task Another_authors_book_is_not_found()
    {
        var bookId = Guid.NewGuid();
        _books.GetForOwnerAsync(bookId, Author, Arg.Any<CancellationToken>()).Returns((Book?)null);
        using var provider = Services();
        var handler = provider.CreateScope().ServiceProvider.GetRequiredService<ICommandHandler<MoveBookToStageCommand, BookDto>>();

        var result = await handler.Handle(new MoveBookToStageCommand(bookId, BookStage.Research), CancellationToken.None);

        result.Error.Type.ShouldBe(ErrorType.NotFound);
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task A_broken_rule_is_returned_and_nothing_is_saved()
    {
        var book = Book.Create(Author, "Puzzles", BookType.Sudoku, DateTimeOffset.UtcNow).Value;
        _books.GetForOwnerAsync(book.Id, Author, Arg.Any<CancellationToken>()).Returns(book);
        using var provider = Services();
        var handler = provider.CreateScope().ServiceProvider.GetRequiredService<ICommandHandler<MoveBookToStageCommand, BookDto>>();

        var result = await handler.Handle(new MoveBookToStageCommand(book.Id, BookStage.Published), CancellationToken.None);

        result.Error.Code.ShouldBe("Book.StageSkipped");
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
