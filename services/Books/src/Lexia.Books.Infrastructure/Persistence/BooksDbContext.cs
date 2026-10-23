using Lexia.Books.Application.Abstractions;
using Lexia.Books.Domain.Books;
using Microsoft.EntityFrameworkCore;

namespace Lexia.Books.Infrastructure.Persistence;

public sealed class BooksDbContext(DbContextOptions<BooksDbContext> options) : DbContext(options), IUnitOfWork
{
    public const string Schema = "books";

    public DbSet<Book> Books => Set<Book>();

    async Task IUnitOfWork.SaveChangesAsync(CancellationToken ct)
    {
        // Domain events are not published yet; the outbox arrives with the first event consumer.
        foreach (var entry in ChangeTracker.Entries<Book>())
        {
            entry.Entity.ClearDomainEvents();
        }

        await SaveChangesAsync(ct);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BooksDbContext).Assembly);
    }
}
