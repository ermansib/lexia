using Lexia.BuildingBlocks.Domain;

namespace Lexia.Studio.Domain.Puzzles;

/// <summary>The walls still standing around a maze cell.</summary>
[Flags]
public enum Walls
{
    None = 0,
    North = 1,
    East = 2,
    South = 4,
    West = 8,
    All = North | East | South | West,
}

/// <summary>
/// A perfect maze (exactly one path between any two cells). The entrance is the top-left cell
/// and the exit is the bottom-right cell.
/// </summary>
public sealed class Maze
{
    public const int MinSize = 5;
    public const int MaxSize = 60;

    private static readonly (Walls Wall, Walls Opposite, int Row, int Column)[] Moves =
    [
        (Walls.North, Walls.South, -1, 0),
        (Walls.East, Walls.West, 0, 1),
        (Walls.South, Walls.North, 1, 0),
        (Walls.West, Walls.East, 0, -1),
    ];

    private Maze(Walls[,] cells, IReadOnlyList<(int Row, int Column)> solution)
    {
        Cells = cells;
        Solution = solution;
    }

    public Walls[,] Cells { get; }

    public int Rows => Cells.GetLength(0);

    public int Columns => Cells.GetLength(1);

    public IReadOnlyList<(int Row, int Column)> Solution { get; }

    public static int SizeFor(Difficulty difficulty) => difficulty switch
    {
        Difficulty.Easy => 10,
        Difficulty.Medium => 18,
        _ => 28,
    };

    public static Result<Maze> Generate(int rows, int columns, Random random)
    {
        if (rows is < MinSize or > MaxSize || columns is < MinSize or > MaxSize)
        {
            return Result.Failure<Maze>(PuzzleErrors.MazeSizeOutOfRange(MinSize, MaxSize));
        }

        var cells = new Walls[rows, columns];
        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < columns; c++)
            {
                cells[r, c] = Walls.All;
            }
        }

        Carve(cells, random);
        return new Maze(cells, Solve(cells));
    }

    public static bool CanMove(Walls[,] cells, int row, int column, Walls direction) =>
        (cells[row, column] & direction) == 0;

    /// <summary>Iterative depth-first carving, so large mazes cannot overflow the stack.</summary>
    private static void Carve(Walls[,] cells, Random random)
    {
        var rows = cells.GetLength(0);
        var columns = cells.GetLength(1);
        var visited = new bool[rows, columns];
        var stack = new Stack<(int Row, int Column)>();
        stack.Push((0, 0));
        visited[0, 0] = true;

        while (stack.Count > 0)
        {
            var (row, column) = stack.Peek();
            var options = Moves
                .Where(m => InBounds(row + m.Row, column + m.Column, rows, columns) && !visited[row + m.Row, column + m.Column])
                .ToList();

            if (options.Count == 0)
            {
                stack.Pop();
                continue;
            }

            var move = options[random.Next(options.Count)];
            var next = (row + move.Row, column + move.Column);
            cells[row, column] &= ~move.Wall;
            cells[next.Item1, next.Item2] &= ~move.Opposite;
            visited[next.Item1, next.Item2] = true;
            stack.Push(next);
        }
    }

    private static List<(int Row, int Column)> Solve(Walls[,] cells)
    {
        var rows = cells.GetLength(0);
        var columns = cells.GetLength(1);
        var previous = new Dictionary<(int, int), (int, int)>();
        var queue = new Queue<(int Row, int Column)>();
        var goal = (rows - 1, columns - 1);
        queue.Enqueue((0, 0));
        previous[(0, 0)] = (-1, -1);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current == goal)
            {
                break;
            }

            foreach (var move in Moves)
            {
                var next = (current.Row + move.Row, current.Column + move.Column);
                if (CanMove(cells, current.Row, current.Column, move.Wall) && previous.TryAdd(next, current))
                {
                    queue.Enqueue(next);
                }
            }
        }

        List<(int Row, int Column)> path = [];
        for (var step = goal; step != (-1, -1); step = previous[step])
        {
            path.Add(step);
        }

        path.Reverse();
        return path;
    }

    private static bool InBounds(int row, int column, int rows, int columns) =>
        row >= 0 && row < rows && column >= 0 && column < columns;
}
