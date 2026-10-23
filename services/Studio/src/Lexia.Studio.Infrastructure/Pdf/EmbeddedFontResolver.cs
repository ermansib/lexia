using PdfSharp.Fonts;

namespace Lexia.Studio.Infrastructure.Pdf;

/// <summary>
/// Serves the DejaVu fonts bundled with this assembly, so PDFs render the same on every host
/// and every font is embedded, which KDP requires.
/// </summary>
internal sealed class EmbeddedFontResolver : IFontResolver
{
    public const string SansFamily = "Lexia Sans";
    public const string MonoFamily = "Lexia Mono";

    private const string Regular = "DejaVuSans";
    private const string Bold = "DejaVuSans-Bold";
    private const string Mono = "DejaVuSansMono";

    private static readonly Lock InstallLock = new();

    /// <summary>PDFsharp's font resolver is process-wide, so install it once.</summary>
    public static void EnsureInstalled()
    {
        lock (InstallLock)
        {
            if (GlobalFontSettings.FontResolver is not EmbeddedFontResolver)
            {
                GlobalFontSettings.FontResolver = new EmbeddedFontResolver();
            }
        }
    }

    public FontResolverInfo? ResolveTypeface(string familyName, bool isBold, bool isItalic) => familyName switch
    {
        MonoFamily => new FontResolverInfo(Mono),
        _ => new FontResolverInfo(isBold ? Bold : Regular),
    };

    public byte[]? GetFont(string faceName)
    {
        var assembly = typeof(EmbeddedFontResolver).Assembly;
        var resource = assembly.GetManifestResourceNames().Single(n => n.EndsWith($".{faceName}.ttf", StringComparison.Ordinal));
        using var stream = assembly.GetManifestResourceStream(resource)!;
        using var buffer = new MemoryStream();
        stream.CopyTo(buffer);
        return buffer.ToArray();
    }
}
