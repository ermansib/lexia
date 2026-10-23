namespace Lexia.Studio.Domain.Kdp;

/// <summary>Page size and safe margins for an interior PDF that KDP will accept.</summary>
public sealed record InteriorLayout(
    decimal PageWidthIn,
    decimal PageHeightIn,
    decimal InsideMarginIn,
    decimal OutsideMarginIn,
    decimal TopMarginIn,
    decimal BottomMarginIn,
    bool HasBleed)
{
    /// <summary>Lexia keeps a little more than the KDP minimum so nothing sits on the edge of the rules.</summary>
    private const decimal ComfortIn = 0.125m;

    public static InteriorLayout For(TrimSize trim, int pageCount, bool hasBleed)
    {
        var width = hasBleed ? trim.WidthIn + KdpPrintRules.BleedIn : trim.WidthIn;
        var height = hasBleed ? trim.HeightIn + 2 * KdpPrintRules.BleedIn : trim.HeightIn;
        var outside = KdpPrintRules.MinOutsideMarginIn(hasBleed) + ComfortIn;

        return new InteriorLayout(
            width,
            height,
            KdpPrintRules.MinInsideMarginIn(pageCount) + ComfortIn,
            outside,
            outside,
            outside,
            hasBleed);
    }
}
