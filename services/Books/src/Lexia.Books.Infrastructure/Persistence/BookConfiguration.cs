using Lexia.Books.Domain.Books;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lexia.Books.Infrastructure.Persistence;

internal sealed class BookConfiguration : IEntityTypeConfiguration<Book>
{
    private const int EnumLength = 40;
    private const int TrimSizeIdLength = 20;

    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("books");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).ValueGeneratedNever();

        builder.Property(b => b.Title).HasMaxLength(Book.MaxTitleLength).IsRequired();
        builder.Property(b => b.Subtitle).HasMaxLength(Book.MaxTitleLength);
        builder.Property(b => b.AuthorName).HasMaxLength(Book.MaxTitleLength);
        builder.Property(b => b.SeriesName).HasMaxLength(Book.MaxTitleLength);
        builder.Property(b => b.TrimSizeId).HasMaxLength(TrimSizeIdLength);
        builder.Property(b => b.Description).HasMaxLength(Book.MaxDescriptionLength);

        builder.Property(b => b.Type).HasConversion<string>().HasMaxLength(EnumLength);
        builder.Property(b => b.Stage).HasConversion<string>().HasMaxLength(EnumLength);
        builder.Property(b => b.Paper).HasConversion<string>().HasMaxLength(EnumLength);

        // Stored as a PostgreSQL text[] column.
        builder.Property<List<string>>("_keywords").HasColumnName("keywords");
        builder.Ignore(b => b.Keywords);
        builder.Ignore(b => b.Category);
        builder.Ignore(b => b.IsReadyToPublish);
        builder.Ignore(b => b.DomainEvents);

        builder.HasIndex(b => new { b.OwnerId, b.UpdatedAt });
    }
}
