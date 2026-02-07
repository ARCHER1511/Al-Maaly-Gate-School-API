using BidiReshapeSharp;
using System.Linq;

namespace Application.Helpers
{
    public static class ArabicHelper
    {
        public static string ShapeArabic(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            // Only process if Arabic exists
            if (!ContainsArabic(text))
                return text;

            return BidiReshape.ProcessString(text);
        }

        private static bool ContainsArabic(string text)
        {
            return text.Any(c => c >= 0x0600 && c <= 0x06FF);
        }
    }
}
