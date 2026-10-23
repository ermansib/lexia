using Lexia.Studio.Domain.Kdp;
using Lexia.Studio.Domain.Puzzles;

namespace Lexia.Studio.Application.Abstractions;

/// <summary>A file Lexia generated for the author to download.</summary>
public sealed record GeneratedFile(string FileName, string ContentType, byte[] Content, int PageCount);

/// <summary>Everything needed to lay out a puzzle book interior.</summary>
public sealed record PuzzleBook(
    string Title,
    string? Author,
    PuzzleType Type,
    IReadOnlyList<WordSearch> WordSearches,
    IReadOnlyList<Sudoku> Sudokus,
    IReadOnlyList<Maze> Mazes,
    bool IncludeSolutions)
{
    public int PuzzleCount => Type switch
    {
        PuzzleType.WordSearch => WordSearches.Count,
        PuzzleType.Sudoku => Sudokus.Count,
        _ => Mazes.Count,
    };
}

public enum JournalStyle
{
    Lined,
    Dotted,
    Grid,
    Blank,
}

public sealed record Journal(string Title, string? Author, JournalStyle Style, int PageCount);

/// <summary>Turns book content into a print-ready interior PDF.</summary>
public interface IInteriorRenderer
{
    GeneratedFile RenderPuzzleBook(PuzzleBook book, TrimSize trim, BookPagePlan plan);

    GeneratedFile RenderJournal(Journal journal, TrimSize trim, BookPagePlan plan);
}
