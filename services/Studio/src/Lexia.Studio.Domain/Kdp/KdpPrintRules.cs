namespace Lexia.Studio.Domain.Kdp;

/// <summary>
/// KDP paperback print rules, taken from KDP's published paperback guidelines.
/// KDP changes these from time to time, so keep every rule in this one place.
/// </summary>
public static class KdpPrintRules
{
    public const decimal BleedIn = 0.125m;
    public const int MinPageCount = 24;
    public const int MinPagesForSpineText = 80;
    public const decimal SpineTextClearanceIn = 0.0625m;
    public const decimal CoverSafeZoneIn = 0.125m;
    public const int MinImageDpi = 300;
    public const decimal MinOutsideMarginNoBleedIn = 0.25m;
    public const decimal MinOutsideMarginWithBleedIn = 0.375m;

    /// <summary>Spine thickness added by each page, in inches.</summary>
    public static decimal SpineWidthPerPageIn(PaperType paper) => paper switch
    {
        PaperType.BlackInkWhitePaper => 0.002252m,
        PaperType.BlackInkCreamPaper => 0.0025m,
        PaperType.StandardColorWhitePaper => 0.002252m,
        PaperType.PremiumColorWhitePaper => 0.002347m,
        _ => throw new ArgumentOutOfRangeException(nameof(paper), paper, null),
    };

    public static int MaxPageCount(PaperType paper, TrimSize trim) => (paper, trim.IsLarge) switch
    {
        (PaperType.BlackInkWhitePaper, false) => 828,
        (PaperType.BlackInkWhitePaper, true) => 590,
        (PaperType.BlackInkCreamPaper, false) => 776,
        (PaperType.BlackInkCreamPaper, true) => 550,
        (PaperType.StandardColorWhitePaper, _) => 600,
        (PaperType.PremiumColorWhitePaper, false) => 828,
        (PaperType.PremiumColorWhitePaper, true) => 590,
        _ => throw new ArgumentOutOfRangeException(nameof(paper), paper, null),
    };

    /// <summary>The minimum inside (gutter) margin grows with page count so text does not disappear into the binding.</summary>
    public static decimal MinInsideMarginIn(int pageCount) => pageCount switch
    {
        <= 150 => 0.375m,
        <= 300 => 0.5m,
        <= 500 => 0.625m,
        <= 700 => 0.75m,
        _ => 0.875m,
    };

    public static decimal MinOutsideMarginIn(bool hasBleed) =>
        hasBleed ? MinOutsideMarginWithBleedIn : MinOutsideMarginNoBleedIn;
}
