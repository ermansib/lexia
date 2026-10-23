using FluentValidation;
using Lexia.BuildingBlocks.Application.Messaging;
using Lexia.BuildingBlocks.Domain;
using Lexia.Studio.Domain.Kdp;

namespace Lexia.Studio.Application.PrintReadiness;

public sealed record PrintIssueDto(string Code, IssueSeverity Severity, string Message, string Fix);

public sealed record PrintReadinessReportDto(bool IsReady, int ErrorCount, int WarningCount, IReadOnlyList<PrintIssueDto> Issues);

public sealed record CheckPrintReadinessQuery(
    string TrimSizeId,
    PaperType Paper,
    int PageCount,
    bool HasBleed,
    decimal PageWidthIn,
    decimal PageHeightIn,
    decimal InsideMarginIn,
    decimal OutsideMarginIn,
    decimal TopMarginIn,
    decimal BottomMarginIn,
    int? LowestImageDpi,
    bool AllFontsEmbedded,
    bool HasSpineText) : IQuery<PrintReadinessReportDto>;

internal sealed class CheckPrintReadinessQueryValidator : AbstractValidator<CheckPrintReadinessQuery>
{
    public CheckPrintReadinessQueryValidator()
    {
        RuleFor(q => q.TrimSizeId).NotEmpty();
        RuleFor(q => q.Paper).IsInEnum();
        RuleFor(q => q.PageCount).GreaterThan(0);
        RuleFor(q => q.PageWidthIn).GreaterThan(0);
        RuleFor(q => q.PageHeightIn).GreaterThan(0);
        RuleFor(q => q.InsideMarginIn).GreaterThanOrEqualTo(0);
        RuleFor(q => q.OutsideMarginIn).GreaterThanOrEqualTo(0);
        RuleFor(q => q.TopMarginIn).GreaterThanOrEqualTo(0);
        RuleFor(q => q.BottomMarginIn).GreaterThanOrEqualTo(0);
        RuleFor(q => q.LowestImageDpi).GreaterThan(0).When(q => q.LowestImageDpi.HasValue);
    }
}

internal sealed class CheckPrintReadinessQueryHandler : IQueryHandler<CheckPrintReadinessQuery, PrintReadinessReportDto>
{
    public Task<Result<PrintReadinessReportDto>> Handle(CheckPrintReadinessQuery query, CancellationToken ct)
    {
        var trim = TrimSize.Find(query.TrimSizeId);
        if (trim is null)
        {
            return Task.FromResult(Result.Failure<PrintReadinessReportDto>(KdpErrors.UnknownTrimSize(query.TrimSizeId)));
        }

        var issues = PrintReadinessChecker.Check(new ManuscriptFacts(
            trim,
            query.Paper,
            query.PageCount,
            query.HasBleed,
            query.PageWidthIn,
            query.PageHeightIn,
            query.InsideMarginIn,
            query.OutsideMarginIn,
            query.TopMarginIn,
            query.BottomMarginIn,
            query.LowestImageDpi,
            query.AllFontsEmbedded,
            query.HasSpineText));

        var report = new PrintReadinessReportDto(
            PrintReadinessChecker.IsReady(issues),
            issues.Count(i => i.Severity == IssueSeverity.Error),
            issues.Count(i => i.Severity == IssueSeverity.Warning),
            [.. issues.Select(i => new PrintIssueDto(i.Code, i.Severity, i.Message, i.Fix))]);

        return Task.FromResult(Result.Success(report));
    }
}
