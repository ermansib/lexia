using Lexia.Studio.Domain.Kdp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace Lexia.Studio.Infrastructure.Pdf;

/// <summary>Adds pages at the right size and works out the live area inside the mirrored margins.</summary>
internal sealed class PageFrame(PdfDocument document, InteriorLayout layout)
{
    private const double PointsPerInch = 72;

    public int PageCount => document.PageCount;

    /// <summary>Adds the next page and returns a canvas plus the box text and art must stay inside.</summary>
    public (XGraphics Graphics, XRect Content) AddPage()
    {
        var page = document.AddPage();
        page.Width = XUnit.FromInch((double)layout.PageWidthIn);
        page.Height = XUnit.FromInch((double)layout.PageHeightIn);

        // Page 1 is a right-hand (recto) page, so its binding edge is on the left.
        var isRecto = document.PageCount % 2 == 1;
        var bleed = layout.HasBleed ? KdpPrintRules.BleedIn : 0m;
        var left = isRecto ? layout.InsideMarginIn : layout.OutsideMarginIn + bleed;
        var top = layout.TopMarginIn + bleed;
        var width = layout.PageWidthIn - layout.InsideMarginIn - layout.OutsideMarginIn - bleed;
        var height = layout.PageHeightIn - layout.TopMarginIn - layout.BottomMarginIn - 2 * bleed;

        var content = new XRect(Points(left), Points(top), Points(width), Points(height));
        return (XGraphics.FromPdfPage(page), content);
    }

    private static double Points(decimal inches) => (double)inches * PointsPerInch;
}
