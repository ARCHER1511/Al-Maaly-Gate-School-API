//using PdfSharp.Fonts;

//namespace Application.Helpers
//{
//    public class FontResolver : IFontResolver
//    {
//        // Look for Fonts folder in project root (go up 3 levels from bin/Debug/net8.0)
//        private static readonly string FontFolder = GetProjectFontFolder();

//        private static string GetProjectFontFolder()
//        {
//            // Try multiple possible locations
//            var possiblePaths = new[]
//            {
//                Path.Combine(AppContext.BaseDirectory, "Fonts"),
//                Path.Combine(Directory.GetCurrentDirectory(), "Fonts"),
//                Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Fonts"), // Go up to project root
//                Path.Combine(Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.FullName, "Fonts")
//            };

//            foreach (var path in possiblePaths)
//            {
//                var fullPath = Path.GetFullPath(path);
//                if (Directory.Exists(fullPath))
//                {
//                    return fullPath;
//                }
//            }

//            // If no Fonts folder found, create one in the output directory
//            var defaultPath = Path.Combine(AppContext.BaseDirectory, "Fonts");
//            if (!Directory.Exists(defaultPath))
//            {
//                Directory.CreateDirectory(defaultPath);
//            }
//            return defaultPath;
//        }

//        public byte[] GetFont(string faceName)
//        {
//            try
//            {
//                var file = faceName switch
//                {
//                    "Regular" => Path.Combine(FontFolder, "LiberationSans-Regular.ttf"),
//                    "Bold" => Path.Combine(FontFolder, "LiberationSans-Bold.ttf"),
//                    "ArabicRegular" => Path.Combine(FontFolder, "arabic-regular.ttf"),
//                    "ArabicBold" => Path.Combine(FontFolder, "arabic-bold.ttf"),
//                    _ => throw new Exception($"Font '{faceName}' not found")
//                };

//                if (!File.Exists(file))
//                {
//                    // Try to find any font file with similar name (case-insensitive)
//                    var fontFiles = Directory.GetFiles(FontFolder, "*.ttf");
//                    var matchingFile = fontFiles.FirstOrDefault(f =>
//                        Path.GetFileNameWithoutExtension(f).Equals(faceName, StringComparison.OrdinalIgnoreCase) ||
//                        Path.GetFileNameWithoutExtension(f).Replace("-", "").Equals(faceName.Replace("-", ""), StringComparison.OrdinalIgnoreCase));

//                    if (matchingFile != null)
//                    {
//                        return File.ReadAllBytes(matchingFile);
//                    }

//                    throw new FileNotFoundException($"Font file not found: {file}. Available fonts: {string.Join(", ", fontFiles.Select(Path.GetFileName))}");
//                }

//                return File.ReadAllBytes(file);
//            }
//            catch (Exception ex)
//            {
//                throw new Exception($"Error loading font '{faceName}' from folder '{FontFolder}': {ex.Message}");
//            }
//        }

//        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
//        {
//            if (familyName.Equals("MyFont", StringComparison.OrdinalIgnoreCase))
//            {
//                return new FontResolverInfo(isBold ? "Bold" : "Regular");
//            }
//            else if (familyName.Equals("ArabicFont", StringComparison.OrdinalIgnoreCase))
//            {
//                return new FontResolverInfo(isBold ? "ArabicBold" : "ArabicRegular");
//            }

//            return null!;
//        }

//        // Helper method to detect if text contains Arabic characters
//        public static bool ContainsArabic(string text)
//        {
//            if (string.IsNullOrEmpty(text))
//                return false;

//            foreach (char c in text)
//            {
//                // Arabic character ranges in Unicode
//                if (c >= 0x0600 && c <= 0x06FF) // Arabic
//                    return true;
//                if (c >= 0x0750 && c <= 0x077F) // Arabic Supplement
//                    return true;
//                if (c >= 0x08A0 && c <= 0x08FF) // Arabic Extended-A
//                    return true;
//                if (c >= 0xFB50 && c <= 0xFDFF) // Arabic Presentation Forms-A
//                    return true;
//                if (c >= 0xFE70 && c <= 0xFEFF) // Arabic Presentation Forms-B
//                    return true;
//            }
//            return false;
//        }
//    }
//}

using PdfSharp.Fonts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class FontResolver : IFontResolver
{
    // First, check if a font file exists
    private bool FontExists(string path)
    {
        try
        {
            return File.Exists(path);
        }
        catch
        {
            return false;
        }
    }

    // Map font faces to actual font files with Arabic support
    private readonly Dictionary<string, string> _fontMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        // Arabic fonts - highest priority
        { "NotoSansArabic", "NotoSansArabic-Regular" },
        { "NotoSansArabic,Bold", "NotoSansArabic-Bold" },
        { "NotoSansArabic,Italic", "NotoSansArabic-Regular" }, // Arabic typically doesn't use italics
        { "NotoSansArabic,BoldItalic", "NotoSansArabic-Bold" },

        { "KacstOne", "KacstOne" },
        { "KacstOne,Bold", "KacstOne" },

        { "DejaVuSans", "DejaVuSans" },
        { "DejaVuSans,Bold", "DejaVuSans-Bold" },
        { "DejaVuSans,Italic", "DejaVuSans-Oblique" },
        { "DejaVuSans,BoldItalic", "DejaVuSans-BoldOblique" },
        
        // Latin fonts as fallback
        { "Arial", "DejaVuSans" }, // DejaVu has better Arabic support than Liberation
        { "Arial,Bold", "DejaVuSans-Bold" },
        { "Arial,Italic", "DejaVuSans-Oblique" },
        { "Arial,BoldItalic", "DejaVuSans-BoldOblique" },

        { "Times New Roman", "DejaVuSerif" },
        { "Times", "DejaVuSerif" },
        { "Courier New", "DejaVuSansMono" },
        { "Courier", "DejaVuSansMono" }
    };

    public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        // Log for debugging
        Console.WriteLine($"ResolveTypeface called: {familyName}, Bold: {isBold}, Italic: {isItalic}");

        // Check if it's likely Arabic text (based on font name hinting)
        bool shouldUseArabicFont = familyName.Contains("Arabic", StringComparison.OrdinalIgnoreCase) ||
                                  familyName.Equals("HEADER", StringComparison.OrdinalIgnoreCase) ||
                                  familyName.Equals("TEXT", StringComparison.OrdinalIgnoreCase) ||
                                  familyName.Contains("Noto", StringComparison.OrdinalIgnoreCase);

        string key;

        if (shouldUseArabicFont)
        {
            // Use Arabic font
            key = "NotoSansArabic";
        }
        else
        {
            // Use the requested font family
            key = familyName;
        }

        if (isBold && isItalic)
            key += ",BoldItalic";
        else if (isBold)
            key += ",Bold";
        else if (isItalic)
            key += ",Italic";

        // Try to get from mapping
        if (_fontMapping.TryGetValue(key, out string? fontName))
        {
            Console.WriteLine($"Mapped {key} to {fontName}");
            return new FontResolverInfo(fontName);
        }

        // Default to Arabic font for better Unicode support
        Console.WriteLine($"Using default Arabic font for: {familyName}");
        return new FontResolverInfo("NotoSansArabic");
    }

    public byte[] GetFont(string faceName)
    {
        Console.WriteLine($"GetFont called for: {faceName}");

        // Define all possible font paths to check
        var fontPaths = new List<string>();

        // Add Arabic font paths first (highest priority)
        fontPaths.AddRange(new[]
        {
            // Noto Sans Arabic (excellent Arabic support)
            $"/usr/share/fonts/truetype/noto/NotoSansArabic-Regular.ttf",
            $"/usr/share/fonts/truetype/noto/NotoSansArabic-Bold.ttf",
            $"/usr/local/share/fonts/NotoSansArabic-Regular.ttf",
            
            // Kacst Arabic fonts
            $"/usr/share/fonts/truetype/kacst/KacstOne.ttf",
            $"/usr/share/fonts/truetype/kacst-one/KacstOne.ttf",
            
            // DejaVu fonts (good Arabic support)
            $"/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf",
            $"/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf",
            $"/usr/share/fonts/truetype/dejavu/DejaVuSans-Oblique.ttf",
            $"/usr/share/fonts/truetype/dejavu/DejaVuSans-BoldOblique.ttf",
            $"/usr/share/fonts/truetype/dejavu/DejaVuSerif.ttf",
            $"/usr/share/fonts/truetype/dejavu/DejaVuSansMono.ttf",
            
            // Arabic Eyes fonts
            $"/usr/share/fonts/truetype/arabeyes/ae_AlMateen.ttf",
            $"/usr/share/fonts/truetype/arabeyes/ae_AlHor.ttf",
            
            // Liberation fonts (fallback, poor Arabic support)
            $"/usr/share/fonts/truetype/liberation/LiberationSans-Regular.ttf",
            $"/usr/share/fonts/truetype/liberation/LiberationSans-Bold.ttf",
            $"/usr/share/fonts/truetype/liberation/LiberationSans-Italic.ttf",
            $"/usr/share/fonts/truetype/liberation/LiberationSans-BoldItalic.ttf"
        });

        // Also check for exact faceName matches
        foreach (var path in fontPaths)
        {
            string fileName = Path.GetFileNameWithoutExtension(path);
            if (fileName.Equals(faceName, StringComparison.OrdinalIgnoreCase) ||
                path.Contains(faceName, StringComparison.OrdinalIgnoreCase))
            {
                if (FontExists(path))
                {
                    Console.WriteLine($"Found font: {path}");
                    return File.ReadAllBytes(path);
                }
            }
        }

        // Fallback: Try to find any font file that contains the faceName
        foreach (var path in fontPaths)
        {
            if (FontExists(path))
            {
                Console.WriteLine($"Using fallback font: {path}");
                return File.ReadAllBytes(path);
            }
        }

        // Last resort: Try to use embedded resource or return null
        Console.WriteLine($"WARNING: No font found for {faceName}");
        return null!;
    }
}