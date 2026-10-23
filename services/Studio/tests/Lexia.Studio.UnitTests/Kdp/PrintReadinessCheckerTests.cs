using Lexia.Studio.Domain.Kdp;

namespace Lexia.Studio.UnitTests.Kdp;

public sealed class PrintReadinessCheckerTests
{
    private static readonly TrimSize SixByNine = TrimSize.Find("6x9")!;

    private static ManuscriptFacts GoodManuscript(int pages = 200) => new(
        SixByNine, PaperType.BlackInkWhitePaper, pages, HasBleed: false,
        PageWidthIn: 6m, PageHeightIn: 9m,
        InsideMarginIn: 0.75m, OutsideMarginIn: 0.5m, TopMarginIn: 0.5m, BottomMarginIn: 0.5m,
        LowestImageDpi: 300, AllFontsEmbedded: true, HasSpineText: true);

    [Fact]
    public void A_manuscript_that_follows_the_rules_has_no_issues()
    {
        var issues = PrintReadinessChecker.Check(GoodManuscript());

        issues.ShouldBeEmpty();
        PrintReadinessChecker.IsReady(issues).ShouldBeTrue();
    }

    [Fact]
    public void Gutter_must_grow_with_page_count()
    {
        var issues = PrintReadinessChecker.Check(GoodManuscript(pages: 400) with { InsideMarginIn = 0.5m });

        issues.ShouldContain(i => i.Code == "Kdp.InsideMarginTooSmall" && i.Severity == IssueSeverity.Error);
    }

    [Fact]
    public void Bleed_pages_must_be_larger_than_the_trim()
    {
        var issues = PrintReadinessChecker.Check(GoodManuscript() with { HasBleed = true });

        issues.ShouldContain(i => i.Code == "Kdp.PageSizeMismatch");
    }

    [Fact]
    public void Bleed_pages_at_the_right_size_pass()
    {
        var issues = PrintReadinessChecker.Check(GoodManuscript() with { HasBleed = true, PageWidthIn = 6.125m, PageHeightIn = 9.25m });

        issues.ShouldNotContain(i => i.Code == "Kdp.PageSizeMismatch");
    }

    [Fact]
    public void Problems_that_cause_rejection_are_errors_and_cosmetic_ones_are_warnings()
    {
        var issues = PrintReadinessChecker.Check(GoodManuscript(pages: 61) with
        {
            AllFontsEmbedded = false,
            LowestImageDpi = 150,
            OutsideMarginIn = 0.1m,
        });

        issues.Single(i => i.Code == "Kdp.FontsNotEmbedded").Severity.ShouldBe(IssueSeverity.Error);
        issues.Single(i => i.Code == "Kdp.SpineTextNotAllowed").Severity.ShouldBe(IssueSeverity.Error);
        issues.Single(i => i.Code == "Kdp.OutsideMarginTooSmall").Severity.ShouldBe(IssueSeverity.Error);
        issues.Single(i => i.Code == "Kdp.LowImageResolution").Severity.ShouldBe(IssueSeverity.Warning);
        issues.Single(i => i.Code == "Kdp.OddPageCount").Severity.ShouldBe(IssueSeverity.Warning);
        PrintReadinessChecker.IsReady(issues).ShouldBeFalse();
    }
}
