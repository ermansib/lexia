using FluentValidation;
using Lexia.BuildingBlocks.Application.Messaging;
using Lexia.BuildingBlocks.Domain;
using Lexia.Studio.Domain.Kdp;

namespace Lexia.Studio.Application.Covers;

public sealed record CoverDimensionsDto(
    string TrimSizeId,
    int PageCount,
    PaperType Paper,
    decimal SpineWidthIn,
    decimal FullWidthIn,
    decimal FullHeightIn,
    decimal BleedIn,
    decimal SafeZoneIn,
    bool SpineTextAllowed,
    int RequiredDpi,
    int FullWidthPx,
    int FullHeightPx);

public sealed record CalculateCoverQuery(string TrimSizeId, int PageCount, PaperType Paper) : IQuery<CoverDimensionsDto>;

internal sealed class CalculateCoverQueryValidator : AbstractValidator<CalculateCoverQuery>
{
    public CalculateCoverQueryValidator()
    {
        RuleFor(q => q.TrimSizeId).NotEmpty();
        RuleFor(q => q.PageCount).GreaterThan(0);
        RuleFor(q => q.Paper).IsInEnum();
    }
}

internal sealed class CalculateCoverQueryHandler : IQueryHandler<CalculateCoverQuery, CoverDimensionsDto>
{
    public Task<Result<CoverDimensionsDto>> Handle(CalculateCoverQuery query, CancellationToken ct) =>
        Task.FromResult(Calculate(query));

    private static Result<CoverDimensionsDto> Calculate(CalculateCoverQuery query)
    {
        var trim = TrimSize.Find(query.TrimSizeId);
        if (trim is null)
        {
            return KdpErrors.UnknownTrimSize(query.TrimSizeId);
        }

        var result = CoverDimensions.Calculate(trim, query.PageCount, query.Paper);
        if (result.IsFailure)
        {
            return result.Error;
        }

        var cover = result.Value;
        return new CoverDimensionsDto(
            trim.Id,
            cover.PageCount,
            cover.Paper,
            cover.SpineWidthIn,
            cover.FullWidthIn,
            cover.FullHeightIn,
            KdpPrintRules.BleedIn,
            KdpPrintRules.CoverSafeZoneIn,
            cover.SpineTextAllowed,
            KdpPrintRules.MinImageDpi,
            (int)Math.Ceiling(cover.FullWidthIn * KdpPrintRules.MinImageDpi),
            (int)Math.Ceiling(cover.FullHeightIn * KdpPrintRules.MinImageDpi));
    }
}
