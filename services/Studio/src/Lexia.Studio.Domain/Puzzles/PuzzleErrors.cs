using Lexia.BuildingBlocks.Domain;

namespace Lexia.Studio.Domain.Puzzles;

public static class PuzzleErrors
{
    public static Error WordTooLong(string word, int size) =>
        Error.Validation("WordSearch.WordTooLong", $"'{word}' is longer than the {size}x{size} grid.");

    public static readonly Error NoWords =
        Error.Validation("WordSearch.NoWords", "A word search needs at least one word made of letters.");

    public static Error MazeSizeOutOfRange(int min, int max) =>
        Error.Validation("Maze.SizeOutOfRange", $"Mazes must be between {min} and {max} cells on each side.");
}
