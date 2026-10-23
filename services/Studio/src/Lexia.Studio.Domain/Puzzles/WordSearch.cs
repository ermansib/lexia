using Lexia.BuildingBlocks.Domain;

namespace Lexia.Studio.Domain.Puzzles;

public sealed record WordPlacement(string Word, int Row, int Column, int RowStep, int ColumnStep);

/// <summary>A filled letter grid with the hidden words and where they are.</summary>
public sealed class WordSearch
{
    private const int MaxAttemptsPerWord = 500;
    private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    private WordSearch(string title, char[,] grid, IReadOnlyList<WordPlacement> placements, IReadOnlyList<string> skipped)
    {
        Title = title;
        Grid = grid;
        Placements = placements;
        SkippedWords = skipped;
    }

    public string Title { get; }

    public char[,] Grid { get; }

    public int Size => Grid.GetLength(0);

    public IReadOnlyList<WordPlacement> Placements { get; }

    /// <summary>Words that would not fit after many attempts. They are left off the word list.</summary>
    public IReadOnlyList<string> SkippedWords { get; }

    /// <summary>Directions a word may run in. Harder puzzles add diagonals and backwards words.</summary>
    public static IReadOnlyList<(int Row, int Column)> DirectionsFor(Difficulty difficulty) => difficulty switch
    {
        Difficulty.Easy => [(0, 1), (1, 0)],
        Difficulty.Medium => [(0, 1), (1, 0), (1, 1), (-1, 1)],
        _ => [(0, 1), (1, 0), (1, 1), (-1, 1), (0, -1), (-1, 0), (-1, -1), (1, -1)],
    };

    public static Result<WordSearch> Generate(string title, IEnumerable<string> words, int size, Difficulty difficulty, Random random)
    {
        var cleaned = words
            .Select(w => new string([.. w.ToUpperInvariant().Where(char.IsAsciiLetterUpper)]))
            .Where(w => w.Length > 0)
            .Distinct()
            .OrderByDescending(w => w.Length)
            .ToList();

        if (cleaned.Count == 0)
        {
            return Result.Failure<WordSearch>(PuzzleErrors.NoWords);
        }

        var tooLong = cleaned.FirstOrDefault(w => w.Length > size);
        if (tooLong is not null)
        {
            return Result.Failure<WordSearch>(PuzzleErrors.WordTooLong(tooLong, size));
        }

        var grid = new char[size, size];
        var directions = DirectionsFor(difficulty);
        List<WordPlacement> placements = [];
        List<string> skipped = [];

        foreach (var word in cleaned)
        {
            var placement = TryPlace(grid, word, directions, random);
            if (placement is null)
            {
                skipped.Add(word);
            }
            else
            {
                placements.Add(placement);
            }
        }

        FillBlanks(grid, random);
        return new WordSearch(title, grid, placements, skipped);
    }

    private static WordPlacement? TryPlace(char[,] grid, string word, IReadOnlyList<(int Row, int Column)> directions, Random random)
    {
        var size = grid.GetLength(0);
        for (var attempt = 0; attempt < MaxAttemptsPerWord; attempt++)
        {
            var (dr, dc) = directions[random.Next(directions.Count)];
            var row = random.Next(size);
            var column = random.Next(size);
            if (!Fits(grid, word, row, column, dr, dc))
            {
                continue;
            }

            for (var i = 0; i < word.Length; i++)
            {
                grid[row + i * dr, column + i * dc] = word[i];
            }

            return new WordPlacement(word, row, column, dr, dc);
        }

        return null;
    }

    private static bool Fits(char[,] grid, string word, int row, int column, int dr, int dc)
    {
        var size = grid.GetLength(0);
        var endRow = row + (word.Length - 1) * dr;
        var endColumn = column + (word.Length - 1) * dc;
        if (endRow < 0 || endRow >= size || endColumn < 0 || endColumn >= size)
        {
            return false;
        }

        for (var i = 0; i < word.Length; i++)
        {
            var existing = grid[row + i * dr, column + i * dc];
            if (existing != default && existing != word[i])
            {
                return false;
            }
        }

        return true;
    }

    private static void FillBlanks(char[,] grid, Random random)
    {
        for (var r = 0; r < grid.GetLength(0); r++)
        {
            for (var c = 0; c < grid.GetLength(1); c++)
            {
                if (grid[r, c] == default)
                {
                    grid[r, c] = Alphabet[random.Next(Alphabet.Length)];
                }
            }
        }
    }
}
