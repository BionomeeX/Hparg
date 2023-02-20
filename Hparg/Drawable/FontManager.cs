using SixLabors.Fonts;
using System.Numerics;

namespace Hparg.Drawable
{
    internal class FontManager
    {
        private static FontManager _instance;

        internal static FontManager Instance
        {
            get
            {
                _instance ??= new();
                return _instance;
            }
        }

        private FontManager()
        {
            if (!SystemFonts.TryGet("Arial", out _targetFont))
            {
                _targetFont = SystemFonts.Families.First();
            }
        }

        public Font GetFont(int size)
            => _targetFont.CreateFont(size, FontStyle.Regular);

        internal Vector2 GetTextSize(string text, int fontSize)
        {
            var measure = TextMeasurer.Measure(text, new TextOptions(GetFont(fontSize)));
            return new(measure.Width, measure.Height);
        }

        private FontFamily _targetFont;
    }
}
