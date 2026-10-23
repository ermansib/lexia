using Lexia.BuildingBlocks.Domain;

namespace Lexia.Studio.Domain.Kdp;

/// <summary>The full wrap-around paperback cover: back, spine and front, with bleed on every edge.</summary>
public sealed record CoverDimensions(
    TrimSize Trim,
    int PageCount,
    PaperType Paper,
    decimal SpineWidthIn,
    decimal FullWidthIn,
    decimal FullHeightIn,
    bool SpineTextAllowed)
{
    private const int Precision = 4;

    public static Result<CoverDimensions> Calculate(TrimSize trim, int pageCount, PaperType paper)
    {
        var validation = PageCountRule.Validate(trim, pageCount, paper);
        if (validation.IsFailure)
        {
            return Result.Failure<CoverDimensions>(validation.Error);
        }

        var spine = Math.Round(pageCount * KdpPrintRules.SpineWidthPerPageIn(paper), Precision);
        var width = KdpPrintRules.BleedIn + trim.WidthIn + spine + trim.WidthIn + KdpPrintRules.BleedIn;
        var height = KdpPrintRules.BleedIn + trim.HeightIn + KdpPrintRules.BleedIn;

        return new CoverDimensions(
            trim,
            pageCount,
            paper,
            spine,
            Math.Round(width, Precision),
            Math.Round(height, Precision),
            pageCount >= KdpPrintRules.MinPagesForSpineText);
    }
}

public static class PageCountRule
{
    public static Result Validate(TrimSize trim, int pageCount, PaperType paper)
    {
        if (pageCount < KdpPrintRules.MinPageCount)
        {
            return KdpErrors.PageCountTooLow(pageCount);
        }

        var max = KdpPrintRules.MaxPageCount(paper, trim);
        return pageCount > max ? KdpErrors.PageCountTooHigh(pageCount, max) : Result.Success();
    }
}
