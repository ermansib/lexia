using Lexia.Books.Domain.Books;

namespace Lexia.Books.UnitTests;

public sealed class BookTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 23, 9, 0, 0, TimeSpan.Zero);
    private static readonly Guid Owner = Guid.NewGuid();

    private static Book NewBook(BookType type = BookType.Journal) => Book.Create(Owner, "  My Journal ", type, Now).Value;

    [Fact]
    public void New_books_start_at_the_idea_stage_and_raise_an_event()
    {
        var book = NewBook();

        book.Title.ShouldBe("My Journal");
        book.Stage.ShouldBe(BookStage.Idea);
        book.DomainEvents.ShouldHaveSingleItem().ShouldBeOfType<BookCreatedDomainEvent>();
    }

    [Fact]
    public void A_title_is_required() =>
        Book.Create(Owner, " ", BookType.Fiction, Now).Error.ShouldBe(BookErrors.TitleRequired);

    [Theory]
    [InlineData(BookType.Sudoku, BookCategory.LowContent)]
    [InlineData(BookType.KidsStory, BookCategory.Illustrated)]
    [InlineData(BookType.Fiction, BookCategory.Written)]
    public void Book_types_map_to_a_category(BookType type, BookCategory category) =>
        NewBook(type).Category.ShouldBe(category);

    [Fact]
    public void Stages_move_forward_one_step_at_a_time_and_back_freely()
    {
        var book = NewBook();

        book.MoveToStage(BookStage.Create, Now).Error.Code.ShouldBe("Book.StageSkipped");
        book.MoveToStage(BookStage.Research, Now).IsSuccess.ShouldBeTrue();
        book.MoveToStage(BookStage.Create, Now).IsSuccess.ShouldBeTrue();
        book.MoveToStage(BookStage.Idea, Now).IsSuccess.ShouldBeTrue();
        book.Stage.ShouldBe(BookStage.Idea);
    }

    [Fact]
    public void A_book_is_only_published_once_its_print_spec_and_listing_are_set()
    {
        var book = NewBook();
        foreach (var stage in new[] { BookStage.Research, BookStage.Create, BookStage.Design, BookStage.Publish })
        {
            book.MoveToStage(stage, Now);
        }

        book.MoveToStage(BookStage.Published, Now).Error.ShouldBe(BookErrors.NotReadyToPublish);

        book.SetPrintSpec("6x9", 120, PaperType.BlackInkCreamPaper, Now);
        book.SetListing("A calm journal.", ["gratitude journal"], Now);
        book.MoveToStage(BookStage.Published, Now).IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void Keywords_follow_kdp_limits()
    {
        var book = NewBook();

        book.SetListing(null, [.. Enumerable.Range(1, 8).Select(i => $"keyword {i}")], Now).Error.Code.ShouldBe("Book.TooManyKeywords");
        book.SetListing(null, [new string('k', 51)], Now).Error.Code.ShouldBe("Book.KeywordTooLong");
        book.SetListing(null, ["Journal", "journal", " ", "diary "], Now).IsSuccess.ShouldBeTrue();
        book.Keywords.ShouldBe(["Journal", "diary"]);
    }

    [Fact]
    public void A_series_number_needs_a_series_name()
    {
        var book = NewBook(BookType.Fiction);

        book.UpdateDetails("Book One", null, null, null, 1, Now).Error.ShouldBe(BookErrors.SeriesNumberWithoutSeries);
        book.UpdateDetails("Book One", null, null, "The Saga", 1, Now).IsSuccess.ShouldBeTrue();
        book.SeriesNumber.ShouldBe(1);
    }
}
