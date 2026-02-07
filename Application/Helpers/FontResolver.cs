using PdfSharp.Fonts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

public class FontResolver : IFontResolver
{
    // اسم Assembly الخاص بك
    private readonly string _assemblyName = Assembly.GetExecutingAssembly().GetName().Name;

    private static readonly Dictionary<string, string> FontMapping =
        new(StringComparer.OrdinalIgnoreCase)
        {
            // Arabic fonts
            { "NotoSansArabic", "NotoSansArabic-Regular" },
            { "NotoSansArabic,Bold", "NotoSansArabic-Bold" },
            { "NotoSansArabic,Italic", "NotoSansArabic-Regular" },
            { "NotoSansArabic,BoldItalic", "NotoSansArabic-Bold" },
            
            // English/Latin fonts
            { "Arial", "DejaVuSans" },
            { "Arial,Bold", "DejaVuSans-Bold" },
            { "Arial,Italic", "DejaVuSans" },
            { "Arial,BoldItalic", "DejaVuSans-Bold" },

            { "DejaVuSans", "DejaVuSans" },
            { "DejaVuSans,Bold", "DejaVuSans-Bold" },
            
            // Liberation fonts
            { "LiberationSans", "LiberationSans-Regular" },
            { "LiberationSans,Bold", "LiberationSans-Bold" },
            
            // Default fonts for headers/text
            { "HEADER", "NotoSansArabic-Bold" },
            { "TEXT", "NotoSansArabic-Regular" }
        };

    public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        Console.WriteLine($"[FontResolver] Resolving: {familyName}, Bold: {isBold}, Italic: {isItalic}");

        string key = familyName;
        if (isBold && isItalic)
            key += ",BoldItalic";
        else if (isBold)
            key += ",Bold";
        else if (isItalic)
            key += ",Italic";

        if (FontMapping.TryGetValue(key, out var faceName))
        {
            Console.WriteLine($"[FontResolver] Mapped to: {faceName}");
            return new FontResolverInfo(faceName);
        }

        // Fallback
        Console.WriteLine($"[FontResolver] Fallback to: NotoSansArabic-Regular");
        return new FontResolverInfo("NotoSansArabic-Regular");
    }

    public byte[] GetFont(string faceName)
    {
        Console.WriteLine($"[FontResolver] Loading font: {faceName}");

        // أولاً: حاول من Embedded Resources
        try
        {
            var fontBytes = LoadFromEmbeddedResources(faceName);
            if (fontBytes != null && fontBytes.Length > 0)
            {
                Console.WriteLine($"[FontResolver] Successfully loaded from Embedded Resources");
                return fontBytes;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FontResolver] Error loading from Resources: {ex.Message}");
        }

        // ثانياً: حاول من مجلد Fonts في Output Directory
        try
        {
            var fontBytes = LoadFromFontsFolder(faceName);
            if (fontBytes != null && fontBytes.Length > 0)
            {
                Console.WriteLine($"[FontResolver] Successfully loaded from Fonts folder");
                return fontBytes;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FontResolver] Error loading from Fonts folder: {ex.Message}");
        }

        // ثالثاً: Fallback إلى خط بديل
        return GetFallbackFont(faceName);
    }

    private byte[] LoadFromEmbeddedResources(string faceName)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // أسماء Resources المحتملة
        string[] resourceNames =
        {
            $"{_assemblyName}.Fonts.{faceName}.ttf",
            $"{_assemblyName}.Fonts.{faceName}.TTF",
            $"{_assemblyName}.fonts.{faceName}.ttf",
            $"{_assemblyName}.Resources.Fonts.{faceName}.ttf",
            $"{_assemblyName}.Resources.{faceName}.ttf"
        };

        foreach (var resourceName in resourceNames)
        {
            try
            {
                Console.WriteLine($"[FontResolver] Trying resource: {resourceName}");

                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        var buffer = new byte[stream.Length];
                        stream.Read(buffer, 0, buffer.Length);
                        return buffer;
                    }
                }
            }
            catch
            {
                continue;
            }
        }

        return null;
    }

    private byte[] LoadFromFontsFolder(string faceName)
    {
        // المسارات المحتملة لمجلد Fonts
        string[] possiblePaths =
        {
            Path.Combine(AppContext.BaseDirectory, "Fonts", $"{faceName}.ttf"),
            Path.Combine(AppContext.BaseDirectory, "fonts", $"{faceName}.ttf"),
            Path.Combine(AppContext.BaseDirectory, "wwwroot", "Fonts", $"{faceName}.ttf"),
            Path.Combine(AppContext.BaseDirectory, "wwwroot", "fonts", $"{faceName}.ttf"),
            Path.Combine(Directory.GetCurrentDirectory(), "Fonts", $"{faceName}.ttf"),
            Path.Combine(Directory.GetCurrentDirectory(), "fonts", $"{faceName}.ttf")
        };

        foreach (var path in possiblePaths)
        {
            try
            {
                Console.WriteLine($"[FontResolver] Checking path: {path}");

                if (File.Exists(path))
                {
                    return File.ReadAllBytes(path);
                }
            }
            catch
            {
                continue;
            }
        }

        return null;
    }

    private byte[] GetFallbackFont(string faceName)
    {
        Console.WriteLine($"[FontResolver] Using fallback for: {faceName}");

        // خريطة Fallback
        var fallbackMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "NotoSansArabic-Regular", "DejaVuSans" },
            { "NotoSansArabic-Bold", "DejaVuSans-Bold" },
            { "DejaVuSans", "LiberationSans-Regular" },
            { "DejaVuSans-Bold", "LiberationSans-Bold" },
            { "LiberationSans-Regular", "DejaVuSans" },
            { "LiberationSans-Bold", "DejaVuSans-Bold" }
        };

        if (fallbackMap.TryGetValue(faceName, out var fallbackFont))
        {
            // حاول تحميل الـ fallback
            var fontBytes = LoadFromEmbeddedResources(fallbackFont) ??
                           LoadFromFontsFolder(fallbackFont);

            if (fontBytes != null)
                return fontBytes;
        }

        // آخر Fallback: استخدم DejaVuSans
        try
        {
            return LoadFromEmbeddedResources("DejaVuSans") ??
                   LoadFromFontsFolder("DejaVuSans") ??
                   throw new Exception("No fonts available");
        }
        catch
        {
            throw new FileNotFoundException($"Font '{faceName}' not found in embedded resources or Fonts folder");
        }
    }
}