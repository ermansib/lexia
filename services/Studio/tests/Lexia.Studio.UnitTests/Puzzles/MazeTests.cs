using Lexia.Studio.Domain.Puzzles;

namespace Lexia.Studio.UnitTests.Puzzles;

public sealed class MazeTests
{
    private static readonly Dictionary<(int, int), Walls> StepWall = new()
    {
        [(-1, 0)] = Walls.North,
        [(0, 1)] = Walls.East,
        [(1, 0)] = Walls.South,
        [(0, -1)] = Walls.West,
    };

    [Fact]
    public void Solution_runs_from_entrance_to_exit_without_crossing_a_wall()
    {
        var maze = Maze.Generate(18, 18, new Random(3)).Value;

        maze.Solution[0].ShouldBe((0, 0));
        maze.Solution[^1].ShouldBe((17, 17));
        foreach (var (from, to) in maze.Solution.Zip(maze.Solution.Skip(1)))
        {
            var wall = StepWall[(to.Row - from.Row, to.Column - from.Column)];
            Maze.CanMove(maze.Cells, from.Row, from.Column, wall).ShouldBeTrue();
        }
    }

    [Fact]
    public void Every_cell_is_reachable_so_the_maze_is_perfect()
    {
        var maze = Maze.Generate(12, 20, new Random(4)).Value;

        // A perfect maze is a spanning tree: cells - 1 open passages.
        var openings = 0;
        for (var r = 0; r < maze.Rows; r++)
        {
            for (var c = 0; c < maze.Columns; c++)
            {
                openings += Maze.CanMove(maze.Cells, r, c, Walls.East) ? 1 : 0;
                openings += Maze.CanMove(maze.Cells, r, c, Walls.South) ? 1 : 0;
            }
        }

        openings.ShouldBe(maze.Rows * maze.Columns - 1);
    }

    [Theory]
    [InlineData(4)]
    [InlineData(61)]
    public void Size_must_be_in_range(int size) =>
        Maze.Generate(size, size, new Random(1)).Error.Code.ShouldBe("Maze.SizeOutOfRange");
}
