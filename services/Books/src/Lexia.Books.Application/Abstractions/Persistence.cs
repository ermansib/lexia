using Lexia.Books.Domain.Books;

namespace Lexia.Books.Application.Abstractions;

/// <summary>Books are always loaded for one owner, so one author can never read another's books.</summary>
public interface IBookRepository
{
    Task<Book?> GetForOwnerAsync(Guid id, Guid ownerId, CancellationToken ct);

    Task<(IReadOnlyList<Book> Items, int Total)> ListForOwnerAsync(Guid ownerId, BookListFilter filter, CancellationToken ct);

    void Add(Book book);

    void Remove(Book book);
}

public sealed record BookListFilter(BookStage? Stage, BookType? Type, string? Search, int Page, int PageSize);

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken ct);
}
