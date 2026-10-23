using Lexia.Studio.Domain.Puzzles;

namespace Lexia.Studio.UnitTests.Puzzles;

public sealed class WordSearchTests
{
    private static readonly string[] Animals = ["cow", "horse", "sheep", "goat", "chicken", "duck", "pig", "tractor", "barn", "hay"];

    [Theory]
    [InlineData(Difficulty.Easy)]
    [InlineData(Difficulty.Medium)]
    [InlineData(Difficulty.Hard)]
    public void Every_placed_word_can_be_read_from_the_grid(Difficulty difficulty)
    {
        var puzzle = WordSearch.Generate("Farm", Animals, 15, difficulty, new Random(1)).Value;

        puzzle.Placements.Count.ShouldBe(Animals.Length);
        foreach (var p in puzzle.Placements)
        {
            var letters = Enumerable.Range(0, p.Word.Length)
                .Select(i => puzzle.Grid[p.Row + i * p.RowStep, p.Column + i * p.ColumnStep]);
            new string([.. letters]).ShouldBe(p.Word);
        }
    }

    [Fact]
    public void Easy_puzzles_only_run_across_or_down()
    {
        var puzzle = WordSearch.Generate("Farm", Animals, 15, Difficulty.Easy, new Random(2)).Value;

        puzzle.Placements.ShouldAllBe(p => (p.RowStep == 0 && p.ColumnStep == 1) || (p.RowStep == 1 && p.ColumnStep == 0));
    }

    [Fact]
    public void Every_cell_is_filled_with_a_capital_letter()
    {
        var puzzle = WordSearch.Generate("Farm", Animals, 12, Difficulty.Hard, new Random(3)).Value;

        puzzle.Grid.Cast<char>().ShouldAllBe(c => char.IsAsciiLetterUpper(c));
    }

    [Fact]
    public void Words_are_cleaned_and_deduplicated()
    {
        var puzzle = WordSearch.Generate("Mixed", ["Ice cream", "ice-cream", "  ", "42"], 12, Difficulty.Easy, new Random(4)).Value;

        puzzle.Placements.Select(p => p.Word).ShouldBe(["ICECREAM"]);
    }

    [Fact]
    public void The_same_seed_gives_the_same_puzzle()
    {
        var first = WordSearch.Generate("Farm", Animals, 15, Difficulty.Hard, new Random(9)).Value;
        var second = WordSearch.Generate("Farm", Animals, 15, Difficulty.Hard, new Random(9)).Value;

        second.Grid.Cast<char>().ShouldBe(first.Grid.Cast<char>());
    }

    [Fact]
    public void A_word_longer_than_the_grid_is_rejected() =>
        WordSearch.Generate("Long", ["supercalifragilistic"], 10, Difficulty.Easy, new Random(5)).Error.Code.ShouldBe("WordSearch.WordTooLong");
}
