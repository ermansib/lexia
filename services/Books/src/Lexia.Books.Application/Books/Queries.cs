using FluentValidation;
using Lexia.Books.Application.Abstractions;
using Lexia.Books.Domain.Books;
using Lexia.BuildingBlocks.Application.Abstractions;
using Lexia.BuildingBlocks.Application.Messaging;
using Lexia.BuildingBlocks.Domain;

namespace Lexia.Books.Application.Books;

public sealed record GetBookQuery(Guid BookId) : IQuery<BookDto>;

internal sealed class GetBookQueryHandler(IBookRepository books, ICurrentUser user) : IQueryHandler<GetBookQuery, BookDto>
{
    public async Task<Result<BookDto>> Handle(GetBookQuery query, CancellationToken ct)
    {
        var book = await books.GetForOwnerAsync(query.BookId, user.Id, ct);
        return book is null ? BookErrors.NotFound(query.BookId) : BookDto.From(book);
    }
}

public sealed record ListMyBooksQuery(BookStage? Stage, BookType? Type, string? Search, int Page = 1, int PageSize = 20)
    : IQuery<PagedResult<BookDto>>;

internal sealed class ListMyBooksQueryValidator : AbstractValidator<ListMyBooksQuery>
{
    public const int MaxPageSize = 100;

    public ListMyBooksQueryValidator()
    {
        RuleFor(q => q.Page).GreaterThanOrEqualTo(1);
        RuleFor(q => q.PageSize).InclusiveBetween(1, MaxPageSize);
        RuleFor(q => q.Search).MaximumLength(Book.MaxTitleLength);
    }
}

internal sealed class ListMyBooksQueryHandler(IBookRepository books, ICurrentUser user) : IQueryHandler<ListMyBooksQuery, PagedResult<BookDto>>
{
    public async Task<Result<PagedResult<BookDto>>> Handle(ListMyBooksQuery query, CancellationToken ct)
    {
        var (items, total) = await books.ListForOwnerAsync(
            user.Id,
            new BookListFilter(query.Stage, query.Type, query.Search, query.Page, query.PageSize),
            ct);

        return new PagedResult<BookDto>([.. items.Select(BookDto.From)], query.Page, query.PageSize, total);
    }
}
