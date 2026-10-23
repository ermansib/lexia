using Lexia.Studio.Domain.Puzzles;

namespace Lexia.Studio.UnitTests.Puzzles;

public sealed class SudokuTests
{
    [Theory]
    [InlineData(Difficulty.Easy, 1)]
    [InlineData(Difficulty.Medium, 2)]
    [InlineData(Difficulty.Hard, 3)]
    public void Puzzles_have_exactly_one_solution(Difficulty difficulty, int seed)
    {
        var sudoku = Sudoku.Generate(difficulty, new Random(seed));

        Sudoku.CountSolutions([.. sudoku.Puzzle], limit: 2).ShouldBe(1);
    }

    [Fact]
    public void The_solution_is_a_valid_grid_that_agrees_with_every_given()
    {
        var sudoku = Sudoku.Generate(Difficulty.Medium, new Random(7));
        var solution = sudoku.Solution.ToArray();

        for (var cell = 0; cell < solution.Length; cell++)
        {
            var value = solution[cell];
            solution[cell] = 0;
            Sudoku.CanPlace(solution, cell, value).ShouldBeTrue();
            solution[cell] = value;

            if (sudoku.Puzzle[cell] != 0)
            {
                sudoku.Puzzle[cell].ShouldBe(value);
            }
        }
    }

    [Fact]
    public void Harder_puzzles_have_fewer_clues()
    {
        var easy = Sudoku.Generate(Difficulty.Easy, new Random(11));
        var hard = Sudoku.Generate(Difficulty.Hard, new Random(11));

        easy.ClueCount.ShouldBe(Sudoku.TargetClues(Difficulty.Easy));
        hard.ClueCount.ShouldBeLessThan(easy.ClueCount);
    }
}
