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

public class FontResolver : IFontResolver
{
    private readonly Dictionary<string, string> _fontMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "Arial", "LiberationSans-Regular" },
        { "Arial,Bold", "LiberationSans-Bold" },
        { "Arial,Italic", "LiberationSans-Italic" },
        { "Arial,BoldItalic", "LiberationSans-BoldItalic" },
        { "Helvetica", "LiberationSans-Regular" },
        { "Helvetica,Bold", "LiberationSans-Bold" },
        { "Helvetica,Italic", "LiberationSans-Italic" },
        { "Helvetica,BoldItalic", "LiberationSans-BoldItalic" },
        { "Times New Roman", "LiberationSerif-Regular" },
        { "Times", "LiberationSerif-Regular" },
        { "Courier New", "LiberationMono-Regular" },
        { "Courier", "LiberationMono-Regular" }
    };

    public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        string key = familyName;
        if (isBold && isItalic)
            key += ",BoldItalic";
        else if (isBold)
            key += ",Bold";
        else if (isItalic)
            key += ",Italic";

        if (_fontMapping.TryGetValue(key, out string? fontName))
        {
            return new FontResolverInfo(fontName);
        }

        // Default to Liberation Sans
        string defaultFont = "LiberationSans-Regular";
        if (isBold && isItalic)
            defaultFont = "LiberationSans-BoldItalic";
        else if (isBold)
            defaultFont = "LiberationSans-Bold";
        else if (isItalic)
            defaultFont = "LiberationSans-Italic";

        return new FontResolverInfo(defaultFont);
    }

    public byte[] GetFont(string faceName)
    {
        // Try different font locations
        string[] possiblePaths = {
            $"/usr/share/fonts/truetype/liberation/{faceName}.ttf",
            $"/usr/share/fonts/truetype/liberation2/{faceName}.ttf",
            $"/usr/share/fonts/truetype/dejavu/{faceName}.ttf",
            $"/usr/share/fonts/truetype/msttcorefonts/{faceName}.ttf",
            $"/usr/share/fonts/opentype/urw-base35/{faceName}.otf"
        };

        foreach (var path in possiblePaths)
        {
            if (File.Exists(path))
            {
                return File.ReadAllBytes(path);
            }
        }

        // If not found, return null to use default
        return null!;
    }
}