namespace Lexia.Studio.Domain.Puzzles;

/// <summary>A 9x9 sudoku with exactly one solution. Cells hold 1 to 9, and 0 means blank.</summary>
public sealed class Sudoku
{
    public const int Size = 9;
    private const int BoxSize = 3;
    private const int CellCount = Size * Size;

    private Sudoku(int[] puzzle, int[] solution, Difficulty difficulty)
    {
        Puzzle = puzzle;
        Solution = solution;
        Difficulty = difficulty;
    }

    public IReadOnlyList<int> Puzzle { get; }

    public IReadOnlyList<int> Solution { get; }

    public Difficulty Difficulty { get; }

    public int ClueCount => Puzzle.Count(v => v != 0);

    /// <summary>How many givens to leave. Fewer clues make a harder puzzle.</summary>
    public static int TargetClues(Difficulty difficulty) => difficulty switch
    {
        Difficulty.Easy => 40,
        Difficulty.Medium => 32,
        _ => 26,
    };

    public static Sudoku Generate(Difficulty difficulty, Random random)
    {
        var solution = new int[CellCount];
        FillGrid(solution, random);

        var puzzle = (int[])solution.Clone();
        var target = TargetClues(difficulty);
        var clues = CellCount;

        foreach (var cell in Enumerable.Range(0, CellCount).OrderBy(_ => random.Next()))
        {
            if (clues <= target)
            {
                break;
            }

            var kept = puzzle[cell];
            puzzle[cell] = 0;
            if (CountSolutions((int[])puzzle.Clone(), limit: 2) == 1)
            {
                clues--;
            }
            else
            {
                puzzle[cell] = kept;
            }
        }

        return new Sudoku(puzzle, solution, difficulty);
    }

    /// <summary>Counts solutions, stopping once <paramref name="limit"/> is reached.</summary>
    public static int CountSolutions(int[] grid, int limit)
    {
        var cell = Array.IndexOf(grid, 0);
        if (cell < 0)
        {
            return 1;
        }

        var count = 0;
        for (var value = 1; value <= Size && count < limit; value++)
        {
            if (!CanPlace(grid, cell, value))
            {
                continue;
            }

            grid[cell] = value;
            count += CountSolutions(grid, limit - count);
            grid[cell] = 0;
        }

        return count;
    }

    public static bool CanPlace(IReadOnlyList<int> grid, int cell, int value)
    {
        var row = cell / Size;
        var column = cell % Size;
        var boxRow = row / BoxSize * BoxSize;
        var boxColumn = column / BoxSize * BoxSize;

        for (var i = 0; i < Size; i++)
        {
            if (grid[row * Size + i] == value
                || grid[i * Size + column] == value
                || grid[(boxRow + i / BoxSize) * Size + boxColumn + i % BoxSize] == value)
            {
                return false;
            }
        }

        return true;
    }

    private static bool FillGrid(int[] grid, Random random)
    {
        var cell = Array.IndexOf(grid, 0);
        if (cell < 0)
        {
            return true;
        }

        foreach (var value in Enumerable.Range(1, Size).OrderBy(_ => random.Next()))
        {
            if (!CanPlace(grid, cell, value))
            {
                continue;
            }

            grid[cell] = value;
            if (FillGrid(grid, random))
            {
                return true;
            }

            grid[cell] = 0;
        }

        return false;
    }
}
