using Lexia.BuildingBlocks.Api;
using Lexia.BuildingBlocks.Application.Messaging;
using Lexia.Studio.Application.Abstractions;
using Lexia.Studio.Application.Covers;
using Lexia.Studio.Application.Journals;
using Lexia.Studio.Application.PrintReadiness;
using Lexia.Studio.Application.PuzzleBooks;
using Lexia.Studio.Application.TrimSizes;
using Lexia.Studio.Domain.Kdp;

namespace Lexia.Studio.Api.Endpoints;

/// <summary>KDP tools: trim sizes, cover calculator, print-readiness checker and interior generators.</summary>
internal static class StudioEndpoints
{
    public static IEndpointRouteBuilder MapStudioEndpoints(this IEndpointRouteBuilder app)
    {
        var kdp = app.MapGroup("/api/v1/studio/kdp").WithTags("KDP tools");

        // The calculators are free tools, so they work without signing in.
        kdp.MapGet("/trim-sizes", async (IQueryHandler<ListTrimSizesQuery, IReadOnlyList<TrimSizeDto>> handler, CancellationToken ct) =>
                (await handler.Handle(new ListTrimSizesQuery(), ct)).ToHttpResult())
            .WithSummary("Lists every KDP paperback trim size.");

        kdp.MapGet("/cover", async (
                string trimSizeId,
                int pageCount,
                PaperType paper,
                IQueryHandler<CalculateCoverQuery, CoverDimensionsDto> handler,
                CancellationToken ct) =>
                (await handler.Handle(new CalculateCoverQuery(trimSizeId, pageCount, paper), ct)).ToHttpResult())
            .WithSummary("Calculates full-wrap cover size and spine width.");

        kdp.MapPost("/print-check", async (
                CheckPrintReadinessQuery query,
                IQueryHandler<CheckPrintReadinessQuery, PrintReadinessReportDto> handler,
                CancellationToken ct) =>
                (await handler.Handle(query, ct)).ToHttpResult())
            .WithSummary("Checks a manuscript against the KDP rules that most often cause rejections.");

        var interiors = app.MapGroup("/api/v1/studio/interiors").WithTags("Interiors").RequireAuthorization();

        interiors.MapPost("/puzzle-books", async (
                GeneratePuzzleBookCommand command,
                ICommandHandler<GeneratePuzzleBookCommand, GeneratedFile> handler,
                CancellationToken ct) =>
                ToFile(await handler.Handle(command, ct)))
            .WithSummary("Generates a print-ready word search, sudoku or maze book interior.");

        interiors.MapPost("/journals", async (
                GenerateJournalCommand command,
                ICommandHandler<GenerateJournalCommand, GeneratedFile> handler,
                CancellationToken ct) =>
                ToFile(await handler.Handle(command, ct)))
            .WithSummary("Generates a print-ready lined, dotted, grid or blank journal interior.");

        return app;
    }

    private static IResult ToFile(BuildingBlocks.Domain.Result<GeneratedFile> result) =>
        result.IsSuccess
            ? Results.File(result.Value.Content, result.Value.ContentType, result.Value.FileName)
            : result.Error.ToProblem();
}
