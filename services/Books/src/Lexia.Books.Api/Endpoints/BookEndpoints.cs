using Lexia.Books.Application.Books;
using Lexia.Books.Domain.Books;
using Lexia.BuildingBlocks.Api;
using Lexia.BuildingBlocks.Application.Messaging;

namespace Lexia.Books.Api.Endpoints;

public sealed record UpdateBookDetailsRequest(string Title, string? Subtitle, string? AuthorName, string? SeriesName, int? SeriesNumber);

public sealed record SetPrintSpecRequest(string TrimSizeId, int PageCount, PaperType Paper);

public sealed record SetListingRequest(string? Description, IReadOnlyList<string> Keywords);

public sealed record MoveToStageRequest(BookStage Stage);

/// <summary>An author's book projects and their progress through the guided flow.</summary>
internal static class BookEndpoints
{
    public static IEndpointRouteBuilder MapBookEndpoints(this IEndpointRouteBuilder app)
    {
        var books = app.MapGroup("/api/v1/books").WithTags("Books").RequireAuthorization();

        books.MapGet("/", async (
                [AsParameters] ListMyBooksQuery query,
                IQueryHandler<ListMyBooksQuery, PagedResult<BookDto>> handler,
                CancellationToken ct) => (await handler.Handle(query, ct)).ToHttpResult())
            .WithSummary("Lists the signed-in author's books, newest activity first.");

        books.MapGet("/{id:guid}", async (Guid id, IQueryHandler<GetBookQuery, BookDto> handler, CancellationToken ct) =>
                (await handler.Handle(new GetBookQuery(id), ct)).ToHttpResult())
            .WithSummary("Gets one book.");

        books.MapPost("/", async (CreateBookCommand command, ICommandHandler<CreateBookCommand, BookDto> handler, CancellationToken ct) =>
                (await handler.Handle(command, ct)).ToCreatedResult(b => $"/api/v1/books/{b.Id}"))
            .WithSummary("Starts a new book at the Idea stage.");

        books.MapPut("/{id:guid}/details", async (
                Guid id,
                UpdateBookDetailsRequest r,
                ICommandHandler<UpdateBookDetailsCommand, BookDto> handler,
                CancellationToken ct) =>
                (await handler.Handle(new UpdateBookDetailsCommand(id, r.Title, r.Subtitle, r.AuthorName, r.SeriesName, r.SeriesNumber), ct)).ToHttpResult())
            .WithSummary("Updates title, subtitle, author and series.");

        books.MapPut("/{id:guid}/print-spec", async (
                Guid id,
                SetPrintSpecRequest r,
                ICommandHandler<SetPrintSpecCommand, BookDto> handler,
                CancellationToken ct) =>
                (await handler.Handle(new SetPrintSpecCommand(id, r.TrimSizeId, r.PageCount, r.Paper), ct)).ToHttpResult())
            .WithSummary("Sets trim size, page count and paper.");

        books.MapPut("/{id:guid}/listing", async (
                Guid id,
                SetListingRequest r,
                ICommandHandler<SetListingCommand, BookDto> handler,
                CancellationToken ct) =>
                (await handler.Handle(new SetListingCommand(id, r.Description, r.Keywords), ct)).ToHttpResult())
            .WithSummary("Sets the Amazon description and up to 7 backend keywords.");

        books.MapPut("/{id:guid}/stage", async (
                Guid id,
                MoveToStageRequest r,
                ICommandHandler<MoveBookToStageCommand, BookDto> handler,
                CancellationToken ct) =>
                (await handler.Handle(new MoveBookToStageCommand(id, r.Stage), ct)).ToHttpResult())
            .WithSummary("Moves the book to another step of the guided flow.");

        books.MapDelete("/{id:guid}", async (Guid id, ICommandHandler<DeleteBookCommand> handler, CancellationToken ct) =>
                (await handler.Handle(new DeleteBookCommand(id), ct)).ToHttpResult())
            .WithSummary("Deletes a book.");

        return app;
    }
}
