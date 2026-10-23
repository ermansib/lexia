using FluentValidation;
using Lexia.Books.Application.Abstractions;
using Lexia.Books.Domain.Books;
using Lexia.BuildingBlocks.Application.Abstractions;
using Lexia.BuildingBlocks.Application.Messaging;
using Lexia.BuildingBlocks.Domain;

namespace Lexia.Books.Application.Books;

public sealed record CreateBookCommand(string Title, BookType Type, string? AuthorName) : ICommand<BookDto>;

internal sealed class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(c => c.Title).NotEmpty().MaximumLength(Book.MaxTitleLength);
        RuleFor(c => c.Type).IsInEnum();
        RuleFor(c => c.AuthorName).MaximumLength(Book.MaxTitleLength);
    }
}

internal sealed class CreateBookCommandHandler(
    IBookRepository books,
    IUnitOfWork unitOfWork,
    ICurrentUser user,
    IDateTimeProvider clock) : ICommandHandler<CreateBookCommand, BookDto>
{
    public async Task<Result<BookDto>> Handle(CreateBookCommand command, CancellationToken ct)
    {
        var created = Book.Create(user.Id, command.Title, command.Type, clock.UtcNow);
        if (created.IsFailure)
        {
            return created.Error;
        }

        var book = created.Value;
        if (command.AuthorName is not null)
        {
            book.UpdateDetails(book.Title, null, command.AuthorName, null, null, clock.UtcNow);
        }

        books.Add(book);
        await unitOfWork.SaveChangesAsync(ct);
        return BookDto.From(book);
    }
}

public sealed record UpdateBookDetailsCommand(
    Guid BookId,
    string Title,
    string? Subtitle,
    string? AuthorName,
    string? SeriesName,
    int? SeriesNumber) : ICommand<BookDto>;

internal sealed class UpdateBookDetailsCommandValidator : AbstractValidator<UpdateBookDetailsCommand>
{
    public UpdateBookDetailsCommandValidator()
    {
        RuleFor(c => c.Title).NotEmpty().MaximumLength(Book.MaxTitleLength);
        RuleFor(c => c.Subtitle).MaximumLength(Book.MaxTitleLength);
        RuleFor(c => c.AuthorName).MaximumLength(Book.MaxTitleLength);
        RuleFor(c => c.SeriesName).MaximumLength(Book.MaxTitleLength);
        RuleFor(c => c.SeriesNumber).GreaterThan(0).When(c => c.SeriesNumber.HasValue);
    }
}

internal sealed class UpdateBookDetailsCommandHandler(BookEditor editor) : ICommandHandler<UpdateBookDetailsCommand, BookDto>
{
    public Task<Result<BookDto>> Handle(UpdateBookDetailsCommand c, CancellationToken ct) =>
        editor.EditAsync(c.BookId, (book, now) => book.UpdateDetails(c.Title, c.Subtitle, c.AuthorName, c.SeriesName, c.SeriesNumber, now), ct);
}

public sealed record SetPrintSpecCommand(Guid BookId, string TrimSizeId, int PageCount, PaperType Paper) : ICommand<BookDto>;

internal sealed class SetPrintSpecCommandValidator : AbstractValidator<SetPrintSpecCommand>
{
    private const int MaxTrimSizeIdLength = 20;
    private const int MaxPages = 828;

    public SetPrintSpecCommandValidator()
    {
        RuleFor(c => c.TrimSizeId).NotEmpty().MaximumLength(MaxTrimSizeIdLength);
        RuleFor(c => c.PageCount).InclusiveBetween(1, MaxPages);
        RuleFor(c => c.Paper).IsInEnum();
    }
}

internal sealed class SetPrintSpecCommandHandler(BookEditor editor) : ICommandHandler<SetPrintSpecCommand, BookDto>
{
    public Task<Result<BookDto>> Handle(SetPrintSpecCommand c, CancellationToken ct) =>
        editor.EditAsync(c.BookId, (book, now) =>
        {
            book.SetPrintSpec(c.TrimSizeId, c.PageCount, c.Paper, now);
            return Result.Success();
        }, ct);
}

public sealed record SetListingCommand(Guid BookId, string? Description, IReadOnlyList<string> Keywords) : ICommand<BookDto>;

internal sealed class SetListingCommandValidator : AbstractValidator<SetListingCommand>
{
    public SetListingCommandValidator()
    {
        RuleFor(c => c.Description).MaximumLength(Book.MaxDescriptionLength);
        RuleFor(c => c.Keywords).NotNull();
    }
}

internal sealed class SetListingCommandHandler(BookEditor editor) : ICommandHandler<SetListingCommand, BookDto>
{
    public Task<Result<BookDto>> Handle(SetListingCommand c, CancellationToken ct) =>
        editor.EditAsync(c.BookId, (book, now) => book.SetListing(c.Description, c.Keywords, now), ct);
}

public sealed record MoveBookToStageCommand(Guid BookId, BookStage Stage) : ICommand<BookDto>;

internal sealed class MoveBookToStageCommandHandler(BookEditor editor) : ICommandHandler<MoveBookToStageCommand, BookDto>
{
    public Task<Result<BookDto>> Handle(MoveBookToStageCommand c, CancellationToken ct) =>
        editor.EditAsync(c.BookId, (book, now) => book.MoveToStage(c.Stage, now), ct);
}

public sealed record DeleteBookCommand(Guid BookId) : ICommand;

internal sealed class DeleteBookCommandHandler(IBookRepository books, IUnitOfWork unitOfWork, ICurrentUser user) : ICommandHandler<DeleteBookCommand>
{
    public async Task<Result> Handle(DeleteBookCommand command, CancellationToken ct)
    {
        var book = await books.GetForOwnerAsync(command.BookId, user.Id, ct);
        if (book is null)
        {
            return BookErrors.NotFound(command.BookId);
        }

        books.Remove(book);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}

/// <summary>Loads the signed-in author's book, applies one domain change and saves it.</summary>
internal sealed class BookEditor(IBookRepository books, IUnitOfWork unitOfWork, ICurrentUser user, IDateTimeProvider clock)
{
    public async Task<Result<BookDto>> EditAsync(Guid bookId, Func<Book, DateTimeOffset, Result> change, CancellationToken ct)
    {
        var book = await books.GetForOwnerAsync(bookId, user.Id, ct);
        if (book is null)
        {
            return BookErrors.NotFound(bookId);
        }

        var result = change(book, clock.UtcNow);
        if (result.IsFailure)
        {
            return result.Error;
        }

        await unitOfWork.SaveChangesAsync(ct);
        return BookDto.From(book);
    }
}
