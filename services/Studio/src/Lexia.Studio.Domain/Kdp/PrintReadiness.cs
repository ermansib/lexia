namespace Lexia.Studio.Domain.Kdp;

public enum IssueSeverity
{
    Warning,
    Error,
}

public sealed record PrintIssue(string Code, IssueSeverity Severity, string Message, string Fix);

/// <summary>What Lexia knows about a manuscript before it is uploaded to KDP.</summary>
public sealed record ManuscriptFacts(
    TrimSize Trim,
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
    bool HasSpineText);

/// <summary>Catches the problems that most often get a paperback rejected by KDP, before upload.</summary>
public static class PrintReadinessChecker
{
    private const decimal SizeToleranceIn = 0.01m;

    public static IReadOnlyList<PrintIssue> Check(ManuscriptFacts facts)
    {
        List<PrintIssue> issues = [];
        CheckPageCount(facts, issues);
        CheckPageSize(facts, issues);
        CheckMargins(facts, issues);
        CheckImages(facts, issues);
        CheckFontsAndSpine(facts, issues);
        return issues;
    }

    public static bool IsReady(IEnumerable<PrintIssue> issues) => issues.All(i => i.Severity != IssueSeverity.Error);

    private static void CheckPageCount(ManuscriptFacts facts, List<PrintIssue> issues)
    {
        var result = PageCountRule.Validate(facts.Trim, facts.PageCount, facts.Paper);
        if (result.IsFailure)
        {
            issues.Add(new(result.Error.Code, IssueSeverity.Error, result.Error.Description,
                "Add or remove pages, or choose a different trim size or paper."));
        }

        if (facts.PageCount % 2 != 0)
        {
            issues.Add(new("Kdp.OddPageCount", IssueSeverity.Warning,
                $"The book has an odd number of pages ({facts.PageCount}). KDP adds a blank page at the end.",
                "Add a final page (notes, about the author) so the last page is yours."));
        }
    }

    private static void CheckPageSize(ManuscriptFacts facts, List<PrintIssue> issues)
    {
        var expected = InteriorLayout.For(facts.Trim, facts.PageCount, facts.HasBleed);
        if (Math.Abs(facts.PageWidthIn - expected.PageWidthIn) > SizeToleranceIn
            || Math.Abs(facts.PageHeightIn - expected.PageHeightIn) > SizeToleranceIn)
        {
            issues.Add(new("Kdp.PageSizeMismatch", IssueSeverity.Error,
                $"Pages are {facts.PageWidthIn}\" x {facts.PageHeightIn}\", but {facts.Trim.Label}{(facts.HasBleed ? " with bleed" : string.Empty)} needs {expected.PageWidthIn}\" x {expected.PageHeightIn}\".",
                facts.HasBleed
                    ? "Add 0.125\" to the width and 0.25\" to the height for bleed."
                    : "Export the PDF at exactly the trim size."));
        }
    }

    private static void CheckMargins(ManuscriptFacts facts, List<PrintIssue> issues)
    {
        var minInside = KdpPrintRules.MinInsideMarginIn(facts.PageCount);
        if (facts.InsideMarginIn < minInside)
        {
            issues.Add(new("Kdp.InsideMarginTooSmall", IssueSeverity.Error,
                $"The inside margin is {facts.InsideMarginIn}\", but a {facts.PageCount}-page book needs at least {minInside}\".",
                $"Set the gutter margin to {minInside}\" or more."));
        }

        var minOutside = KdpPrintRules.MinOutsideMarginIn(facts.HasBleed);
        var smallest = new[] { facts.OutsideMarginIn, facts.TopMarginIn, facts.BottomMarginIn }.Min();
        if (smallest < minOutside)
        {
            issues.Add(new("Kdp.OutsideMarginTooSmall", IssueSeverity.Error,
                $"An outside, top or bottom margin is {smallest}\", but the minimum is {minOutside}\".",
                $"Keep text and important art at least {minOutside}\" from the page edge."));
        }
    }

    private static void CheckImages(ManuscriptFacts facts, List<PrintIssue> issues)
    {
        if (facts.LowestImageDpi is { } dpi && dpi < KdpPrintRules.MinImageDpi)
        {
            issues.Add(new("Kdp.LowImageResolution", IssueSeverity.Warning,
                $"At least one image is {dpi} DPI. KDP recommends {KdpPrintRules.MinImageDpi} DPI and may flag blurry images.",
                "Replace or upscale images to 300 DPI at their printed size."));
        }
    }

    private static void CheckFontsAndSpine(ManuscriptFacts facts, List<PrintIssue> issues)
    {
        if (!facts.AllFontsEmbedded)
        {
            issues.Add(new("Kdp.FontsNotEmbedded", IssueSeverity.Error,
                "Some fonts are not embedded in the PDF.",
                "Re-export the PDF with all fonts embedded (Lexia's exports always embed fonts)."));
        }

        if (facts.HasSpineText && facts.PageCount < KdpPrintRules.MinPagesForSpineText)
        {
            issues.Add(new("Kdp.SpineTextNotAllowed", IssueSeverity.Error,
                $"KDP only allows spine text on books with {KdpPrintRules.MinPagesForSpineText} or more pages.",
                "Remove the spine text, or add pages."));
        }
    }
}
