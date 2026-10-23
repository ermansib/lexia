using Lexia.Studio.Domain.Kdp;

namespace Lexia.Studio.Application.Abstractions;

/// <summary>
/// How many pages a generated interior will have once it is padded to KDP's minimum
/// and to an even count, and the layout that page count needs.
/// </summary>
public sealed record BookPagePlan(int ContentPages, int PaddingPages, InteriorLayout Layout)
{
    public int TotalPages => ContentPages + PaddingPages;

    public static BookPagePlan For(TrimSize trim, int contentPages, bool hasBleed = false)
    {
        var total = Math.Max(contentPages, KdpPrintRules.MinPageCount);
        if (total % 2 != 0)
        {
            total++;
        }

        return new BookPagePlan(contentPages, total - contentPages, InteriorLayout.For(trim, total, hasBleed));
    }
}
