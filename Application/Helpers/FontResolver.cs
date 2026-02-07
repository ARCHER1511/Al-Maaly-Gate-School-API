using PdfSharp.Fonts;

public class FontResolver : IFontResolver
{
    private static readonly Dictionary<string, string> Map =
        new(StringComparer.OrdinalIgnoreCase)
        {
            { "NotoSansArabic", "NotoSansArabic-Regular" },
            { "NotoSansArabic,Bold", "NotoSansArabic-Bold" },
            { "Arial", "DejaVuSans" },
            { "Arial,Bold", "DejaVuSans-Bold" }
        };

    public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        string key = familyName;
        if (isBold) key += ",Bold";

        if (Map.TryGetValue(key, out var face))
            return new FontResolverInfo(face);

        return new FontResolverInfo("DejaVuSans");
    }

    public byte[] GetFont(string faceName)
    {
        string[] paths =
        {
            "/usr/share/fonts/truetype/noto/NotoSansArabic-Regular.ttf",
            "/usr/share/fonts/truetype/noto/NotoSansArabic-Bold.ttf",
            "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf",
            "/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf"
        };

        foreach (var p in paths)
            if (File.Exists(p) && p.Contains(faceName))
                return File.ReadAllBytes(p);

        throw new FileNotFoundException($"Font not found: {faceName}");
    }
}
