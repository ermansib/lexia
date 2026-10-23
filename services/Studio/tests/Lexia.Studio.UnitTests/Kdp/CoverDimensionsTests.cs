using Lexia.Studio.Domain.Kdp;

namespace Lexia.Studio.UnitTests.Kdp;

public sealed class CoverDimensionsTests
{
    private static readonly TrimSize SixByNine = TrimSize.Find("6x9")!;

    [Theory]
    [InlineData(PaperType.BlackInkWhitePaper, 100, 0.2252)]
    [InlineData(PaperType.BlackInkCreamPaper, 100, 0.25)]
    [InlineData(PaperType.PremiumColorWhitePaper, 100, 0.2347)]
    public void Spine_width_is_page_count_times_paper_thickness(PaperType paper, int pages, double expected)
    {
        var cover = CoverDimensions.Calculate(SixByNine, pages, paper).Value;

        cover.SpineWidthIn.ShouldBe((decimal)expected);
    }

    [Fact]
    public void Full_cover_wraps_back_spine_and_front_with_bleed_on_every_edge()
    {
        var cover = CoverDimensions.Calculate(SixByNine, 120, PaperType.BlackInkCreamPaper).Value;

        cover.FullWidthIn.ShouldBe(0.125m + 6m + 0.3m + 6m + 0.125m);
        cover.FullHeightIn.ShouldBe(9.25m);
    }

    [Theory]
    [InlineData(79, false)]
    [InlineData(80, true)]
    public void Spine_text_needs_at_least_80_pages(int pages, bool allowed) =>
        CoverDimensions.Calculate(SixByNine, pages, PaperType.BlackInkWhitePaper).Value.SpineTextAllowed.ShouldBe(allowed);

    [Fact]
    public void Too_few_pages_is_rejected() =>
        CoverDimensions.Calculate(SixByNine, 23, PaperType.BlackInkWhitePaper).Error.Code.ShouldBe("Kdp.PageCountTooLow");

    [Fact]
    public void Large_trim_sizes_have_a_lower_page_limit()
    {
        var letter = TrimSize.Find("8.5x11")!;

        letter.IsLarge.ShouldBeTrue();
        CoverDimensions.Calculate(letter, 600, PaperType.BlackInkWhitePaper).Error.Code.ShouldBe("Kdp.PageCountTooHigh");
        CoverDimensions.Calculate(SixByNine, 600, PaperType.BlackInkWhitePaper).IsSuccess.ShouldBeTrue();
    }
}
