using SixLabors.Fonts;

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

        internal float GetTextSize(string text, int fontSize)
            => TextMeasurer.Measure(text, new TextOptions(GetFont(fontSize))).Width;

        private FontFamily _targetFont;
    }
}
