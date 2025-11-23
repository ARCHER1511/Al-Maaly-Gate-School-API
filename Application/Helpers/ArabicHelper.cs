using BidiReshapeSharp;

namespace Application.Helpers
{
    public static class ArabicHelper
    {
        public static string ShapeArabic(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            // Call the static method directly on BidiReshape
            return BidiReshape.ProcessString(text);
        }
    }
}
