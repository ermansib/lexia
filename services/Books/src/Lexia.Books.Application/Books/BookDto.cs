using Lexia.Books.Domain.Books;

namespace Lexia.Books.Application.Books;

public sealed record BookDto(
    Guid Id,
    string Title,
    string? Subtitle,
    string? AuthorName,
    BookType Type,
    BookCategory Category,
    BookStage Stage,
    string? SeriesName,
    int? SeriesNumber,
    string? TrimSizeId,
    int? PageCount,
    PaperType? Paper,
    string? Description,
    IReadOnlyList<string> Keywords,
    bool IsReadyToPublish,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt)
{
    public static BookDto From(Book book) => new(
        book.Id,
        book.Title,
        book.Subtitle,
        book.AuthorName,
        book.Type,
        book.Category,
        book.Stage,
        book.SeriesName,
        book.SeriesNumber,
        book.TrimSizeId,
        book.PageCount,
        book.Paper,
        book.Description,
        book.Keywords,
        book.IsReadyToPublish,
        book.CreatedAt,
        book.UpdatedAt);
}

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int Total);
