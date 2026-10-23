using Lexia.BuildingBlocks.Domain;

namespace Lexia.Books.Domain.Books;

public static class BookErrors
{
    public static Error NotFound(Guid id) => Error.NotFound("Book.NotFound", $"No book with id '{id}' was found.");

    public static readonly Error TitleRequired = Error.Validation("Book.TitleRequired", "A book needs a title.");

    public static Error TooManyKeywords(int max) =>
        Error.Validation("Book.TooManyKeywords", $"KDP allows at most {max} keyword boxes.");

    public static Error KeywordTooLong(string keyword, int max) =>
        Error.Validation("Book.KeywordTooLong", $"'{keyword}' is longer than KDP's {max}-character keyword limit.");

    public static Error StageSkipped(BookStage from, BookStage to) =>
        Error.Conflict("Book.StageSkipped", $"A book in {from} can move forward one step at a time, not straight to {to}.");

    public static readonly Error NotReadyToPublish = Error.Conflict(
        "Book.NotReadyToPublish",
        "Set the trim size, page count, paper, description and at least one keyword before marking the book published.");

    public static readonly Error SeriesNumberWithoutSeries =
        Error.Validation("Book.SeriesNumberWithoutSeries", "A series number needs a series name.");
}
