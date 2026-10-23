using FluentValidation;
using Lexia.BuildingBlocks.Application.Messaging;
using Lexia.BuildingBlocks.Domain;
using Lexia.Studio.Application.Abstractions;
using Lexia.Studio.Domain.Kdp;
using Lexia.Studio.Domain.Puzzles;

namespace Lexia.Studio.Application.PuzzleBooks;

public sealed record WordListInput(string Title, IReadOnlyList<string> Words);

public sealed record GeneratePuzzleBookCommand(
    string Title,
    string? Author,
    string TrimSizeId,
    PuzzleType PuzzleType,
    Difficulty Difficulty,
    int PuzzleCount,
    IReadOnlyList<WordListInput>? WordLists,
    bool IncludeSolutions,
    int? Seed) : ICommand<GeneratedFile>;

internal sealed class GeneratePuzzleBookCommandValidator : AbstractValidator<GeneratePuzzleBookCommand>
{
    public const int MaxPuzzles = 200;
    public const int MaxTitleLength = 200;

    public GeneratePuzzleBookCommandValidator()
    {
        RuleFor(c => c.Title).NotEmpty().MaximumLength(MaxTitleLength);
        RuleFor(c => c.Author).MaximumLength(MaxTitleLength);
        RuleFor(c => c.TrimSizeId).Must(id => TrimSize.Find(id) is not null).WithMessage("Choose a KDP trim size.");
        RuleFor(c => c.PuzzleType).IsInEnum();
        RuleFor(c => c.Difficulty).IsInEnum();
        RuleFor(c => c.PuzzleCount).InclusiveBetween(1, MaxPuzzles);
        RuleFor(c => c.WordLists)
            .NotEmpty()
            .WithMessage("Word search books need at least one word list.")
            .When(c => c.PuzzleType == PuzzleType.WordSearch);
    }
}

internal sealed class GeneratePuzzleBookCommandHandler(IInteriorRenderer renderer)
    : ICommandHandler<GeneratePuzzleBookCommand, GeneratedFile>
{
    private const int WordSearchSize = 15;

    public Task<Result<GeneratedFile>> Handle(GeneratePuzzleBookCommand command, CancellationToken ct) =>
        Task.FromResult(Generate(command));

    private Result<GeneratedFile> Generate(GeneratePuzzleBookCommand command)
    {
        var trim = TrimSize.Find(command.TrimSizeId)!;
        var random = new Random(command.Seed ?? Random.Shared.Next());

        List<WordSearch> wordSearches = [];
        List<Sudoku> sudokus = [];
        List<Maze> mazes = [];

        for (var i = 0; i < command.PuzzleCount; i++)
        {
            switch (command.PuzzleType)
            {
                case PuzzleType.WordSearch:
                    var list = command.WordLists![i % command.WordLists.Count];
                    var search = WordSearch.Generate(list.Title, list.Words, WordSearchSize, command.Difficulty, random);
                    if (search.IsFailure)
                    {
                        return search.Error;
                    }

                    wordSearches.Add(search.Value);
                    break;
                case PuzzleType.Sudoku:
                    sudokus.Add(Sudoku.Generate(command.Difficulty, random));
                    break;
                default:
                    var size = Maze.SizeFor(command.Difficulty);
                    mazes.Add(Maze.Generate(size, size, random).Value);
                    break;
            }
        }

        var book = new PuzzleBook(command.Title, command.Author, command.PuzzleType, wordSearches, sudokus, mazes, command.IncludeSolutions);
        return renderer.RenderPuzzleBook(book, trim, BookPagePlan.For(trim, PuzzleBookPages.Count(book)));
    }
}

/// <summary>Counts the pages a puzzle book needs before padding, so the gutter margin can be chosen up front.</summary>
public static class PuzzleBookPages
{
    public const int FrontMatterPages = 2;
    public const int SudokuSolutionsPerPage = 4;

    public static int Count(PuzzleBook book)
    {
        var puzzles = book.PuzzleCount;
        if (!book.IncludeSolutions)
        {
            return FrontMatterPages + puzzles;
        }

        var solutionPages = book.Type == PuzzleType.Sudoku
            ? (int)Math.Ceiling(puzzles / (double)SudokuSolutionsPerPage)
            : puzzles;

        // One divider page introduces the solutions section.
        return FrontMatterPages + puzzles + 1 + solutionPages;
    }
}
