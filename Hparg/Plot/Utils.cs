using Hparg.Drawable;
using System.Globalization;

namespace Hparg.Plot
{
    internal static class Utils
    {

        internal static string FormatNumber(float nb)
        {
            if (nb > 1000 || nb < -1000)
            {
                return $"{nb:0.00E0}";
            }
            return $"{nb:0.00}";
        }

        internal static int GetTextSize(float min, float max)
        {
            var minText = FormatNumber(min);
            var maxText = FormatNumber(max);
            return (int)FontManager.Instance.GetTextSize(minText.Length > maxText.Length ? minText : maxText, 15);
        }
    }
}
