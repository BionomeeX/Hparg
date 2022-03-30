using SixLabors.ImageSharp;

namespace Hparg.Drawable
{
    public class DrawingZone
    {
        public DrawingZone(float x, float y, float w, float h)
            => (_x, _y, _w, _h) = (x, y, w, h);

        public PointF ToLocal(float x, float y)
        {
            return new(x * _w + _x, y * _h + _y);
        }

        internal float Width => _w;
        internal float Height => _h;
        internal float X => _x;
        internal float Y => _y;

        private readonly float _x, _y, _w, _h;
    }
}
