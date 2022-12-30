using Hparg.Drawable;

namespace Hparg.Plot
{
    internal static class Utils
    {

        internal static string FormatNumber(float nb)
        {
            if (nb > 1000f || nb < -1000 || (nb > -0.01f && nb < 0.01f && nb != 0f))
            {
                return $"{nb:0.00E+0}";
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
