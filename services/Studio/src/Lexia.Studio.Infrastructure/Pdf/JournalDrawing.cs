using Lexia.Studio.Application.Abstractions;
using PdfSharp.Drawing;

namespace Lexia.Studio.Infrastructure.Pdf;

/// <summary>Draws ruled, dotted and grid journal pages.</summary>
internal static class JournalDrawing
{
    private const double LineSpacing = 0.3 * 72;
    private const double GridSpacing = 0.25 * 72;
    private const double DotRadius = 0.7;
    private static readonly XBrush DotBrush = new XSolidBrush(XColor.FromGrayScale(0.55));

    public static void Page(XGraphics gfx, XRect content, JournalStyle style)
    {
        switch (style)
        {
            case JournalStyle.Lined:
                Lines(gfx, content);
                break;
            case JournalStyle.Dotted:
                Dots(gfx, content);
                break;
            case JournalStyle.Grid:
                Grid(gfx, content);
                break;
            default:
                break;
        }
    }

    public static void Lines(XGraphics gfx, XRect area)
    {
        for (var y = area.Top + LineSpacing; y <= area.Bottom; y += LineSpacing)
        {
            gfx.DrawLine(Typography.Guide, area.Left, y, area.Right, y);
        }
    }

    private static void Dots(XGraphics gfx, XRect area)
    {
        for (var y = area.Top; y <= area.Bottom; y += GridSpacing)
        {
            for (var x = area.Left; x <= area.Right; x += GridSpacing)
            {
                gfx.DrawEllipse(DotBrush, x - DotRadius, y - DotRadius, DotRadius * 2, DotRadius * 2);
            }
        }
    }

    private static void Grid(XGraphics gfx, XRect area)
    {
        var columns = (int)(area.Width / GridSpacing);
        var rows = (int)(area.Height / GridSpacing);
        var right = area.Left + columns * GridSpacing;
        var bottom = area.Top + rows * GridSpacing;

        for (var c = 0; c <= columns; c++)
        {
            gfx.DrawLine(Typography.Guide, area.Left + c * GridSpacing, area.Top, area.Left + c * GridSpacing, bottom);
        }

        for (var r = 0; r <= rows; r++)
        {
            gfx.DrawLine(Typography.Guide, area.Left, area.Top + r * GridSpacing, right, area.Top + r * GridSpacing);
        }
    }
}
