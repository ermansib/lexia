using Lexia.BuildingBlocks.Domain;

namespace Lexia.Books.Domain.Books;

/// <summary>An author's book project, carried through the guided flow from idea to publication.</summary>
public sealed class Book : AggregateRoot
{
    public const int MaxTitleLength = 200;
    public const int MaxKeywords = 7;
    public const int MaxKeywordLength = 50;
    public const int MaxDescriptionLength = 4000;

    private List<string> _keywords = [];

    private Book(Guid id, Guid ownerId, string title, BookType type, DateTimeOffset now)
        : base(id)
    {
        OwnerId = ownerId;
        Title = title;
        Type = type;
        Stage = BookStage.Idea;
        CreatedAt = now;
        UpdatedAt = now;
    }

    // Used by EF Core.
    private Book()
    {
        Title = string.Empty;
    }

    public Guid OwnerId { get; private set; }

    public string Title { get; private set; }

    public string? Subtitle { get; private set; }

    public string? AuthorName { get; private set; }

    public BookType Type { get; private set; }

    public BookCategory Category => Type.Category();

    public BookStage Stage { get; private set; }

    public string? SeriesName { get; private set; }

    public int? SeriesNumber { get; private set; }

    public string? TrimSizeId { get; private set; }

    public int? PageCount { get; private set; }

    public PaperType? Paper { get; private set; }

    public string? Description { get; private set; }

    public IReadOnlyList<string> Keywords => _keywords.AsReadOnly();

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static Result<Book> Create(Guid ownerId, string title, BookType type, DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result.Failure<Book>(BookErrors.TitleRequired);
        }

        var book = new Book(Guid.CreateVersion7(now), ownerId, title.Trim(), type, now);
        book.Raise(new BookCreatedDomainEvent(book.Id, ownerId, type, now));
        return book;
    }

    public Result UpdateDetails(string title, string? subtitle, string? authorName, string? seriesName, int? seriesNumber, DateTimeOffset now)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return BookErrors.TitleRequired;
        }

        if (seriesNumber is not null && string.IsNullOrWhiteSpace(seriesName))
        {
            return BookErrors.SeriesNumberWithoutSeries;
        }

        Title = title.Trim();
        Subtitle = Clean(subtitle);
        AuthorName = Clean(authorName);
        SeriesName = Clean(seriesName);
        SeriesNumber = SeriesName is null ? null : seriesNumber;
        UpdatedAt = now;
        return Result.Success();
    }

    public void SetPrintSpec(string trimSizeId, int pageCount, PaperType paper, DateTimeOffset now)
    {
        TrimSizeId = trimSizeId;
        PageCount = pageCount;
        Paper = paper;
        UpdatedAt = now;
    }

    public Result SetListing(string? description, IEnumerable<string> keywords, DateTimeOffset now)
    {
        List<string> cleaned = [.. keywords.Select(k => k.Trim()).Where(k => k.Length > 0).Distinct(StringComparer.OrdinalIgnoreCase)];
        if (cleaned.Count > MaxKeywords)
        {
            return BookErrors.TooManyKeywords(MaxKeywords);
        }

        var tooLong = cleaned.FirstOrDefault(k => k.Length > MaxKeywordLength);
        if (tooLong is not null)
        {
            return BookErrors.KeywordTooLong(tooLong, MaxKeywordLength);
        }

        Description = Clean(description);
        _keywords = cleaned;
        UpdatedAt = now;
        return Result.Success();
    }

    /// <summary>
    /// Authors may go back to any earlier step, but forward only one step at a time, and a book
    /// is only marked published once it has everything KDP asks for.
    /// </summary>
    public Result MoveToStage(BookStage target, DateTimeOffset now)
    {
        if (target == Stage)
        {
            return Result.Success();
        }

        if (target > Stage + 1)
        {
            return BookErrors.StageSkipped(Stage, target);
        }

        if (target == BookStage.Published && !IsReadyToPublish)
        {
            return BookErrors.NotReadyToPublish;
        }

        var from = Stage;
        Stage = target;
        UpdatedAt = now;
        Raise(new BookStageChangedDomainEvent(Id, from, target, now));
        return Result.Success();
    }

    public bool IsReadyToPublish =>
        TrimSizeId is not null && PageCount is not null && Paper is not null
        && Description is not null && _keywords.Count > 0;

    private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
