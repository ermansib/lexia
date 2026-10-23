using Lexia.Books.Application.Abstractions;
using Lexia.Books.Domain.Books;
using Microsoft.EntityFrameworkCore;

namespace Lexia.Books.Infrastructure.Persistence;

internal sealed class BookRepository(BooksDbContext db) : IBookRepository
{
    public Task<Book?> GetForOwnerAsync(Guid id, Guid ownerId, CancellationToken ct) =>
        db.Books.FirstOrDefaultAsync(b => b.Id == id && b.OwnerId == ownerId, ct);

    public async Task<(IReadOnlyList<Book> Items, int Total)> ListForOwnerAsync(Guid ownerId, BookListFilter filter, CancellationToken ct)
    {
        var query = db.Books.AsNoTracking().Where(b => b.OwnerId == ownerId);

        if (filter.Stage is { } stage)
        {
            query = query.Where(b => b.Stage == stage);
        }

        if (filter.Type is { } type)
        {
            query = query.Where(b => b.Type == type);
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var pattern = $"%{filter.Search.Trim()}%";
            query = query.Where(b => EF.Functions.ILike(b.Title, pattern) || (b.SeriesName != null && EF.Functions.ILike(b.SeriesName, pattern)));
        }

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(b => b.UpdatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public void Add(Book book) => db.Books.Add(book);

    public void Remove(Book book) => db.Books.Remove(book);
}
