using PdfSharp.Drawing;

namespace Lexia.Studio.Infrastructure.Pdf;

internal static class Typography
{
    public static readonly XFont Title = new(EmbeddedFontResolver.SansFamily, 26, XFontStyleEx.Bold);
    public static readonly XFont Subtitle = new(EmbeddedFontResolver.SansFamily, 14, XFontStyleEx.Regular);
    public static readonly XFont Heading = new(EmbeddedFontResolver.SansFamily, 16, XFontStyleEx.Bold);
    public static readonly XFont Body = new(EmbeddedFontResolver.SansFamily, 10, XFontStyleEx.Regular);
    public static readonly XFont Small = new(EmbeddedFontResolver.SansFamily, 8, XFontStyleEx.Regular);

    public static readonly XPen Hairline = new(XColors.Black, 0.5);
    public static readonly XPen Rule = new(XColors.Black, 1);
    public static readonly XPen Thick = new(XColors.Black, 2);
    public static readonly XPen Guide = new(XColor.FromGrayScale(0.6), 0.5);

    public static XFont Mono(double size) => new(EmbeddedFontResolver.MonoFamily, size, XFontStyleEx.Regular);

    public static XFont Sans(double size, bool bold = false) =>
        new(EmbeddedFontResolver.SansFamily, size, bold ? XFontStyleEx.Bold : XFontStyleEx.Regular);

    /// <summary>Draws text centered in a band across the content box, and returns the y below it.</summary>
    public static double DrawCentered(XGraphics gfx, string text, XFont font, XRect content, double y)
    {
        var height = gfx.MeasureString(text, font).Height;
        gfx.DrawString(text, font, XBrushes.Black, new XRect(content.X, y, content.Width, height), XStringFormats.TopCenter);
        return y + height;
    }

    /// <summary>Wraps text to the content width and returns the y below the last line.</summary>
    public static double DrawWrapped(XGraphics gfx, string text, XFont font, XRect content, double y)
    {
        var line = string.Empty;
        foreach (var word in text.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            var candidate = line.Length == 0 ? word : $"{line} {word}";
            if (gfx.MeasureString(candidate, font).Width > content.Width && line.Length > 0)
            {
                y = DrawLine(gfx, line, font, content.X, y);
                line = word;
            }
            else
            {
                line = candidate;
            }
        }

        return line.Length > 0 ? DrawLine(gfx, line, font, content.X, y) : y;
    }

    private static double DrawLine(XGraphics gfx, string text, XFont font, double x, double y)
    {
        var height = gfx.MeasureString(text, font).Height;
        gfx.DrawString(text, font, XBrushes.Black, new XRect(x, y, 10_000, height), XStringFormats.TopLeft);
        return y + height * 1.3;
    }
}
