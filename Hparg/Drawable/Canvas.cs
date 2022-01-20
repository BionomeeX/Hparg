using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Hparg.Drawable
{
    public class Canvas // TODO: Dispose?
    {
        internal Canvas(int width, int height, int offset)
        {
            _offset = offset;
            _maxWidth = width - 2 * _offset;
            _maxHeight = height - 2 * _offset;
            SetDrawingZone(0f, 1f, 0f, 1f);

            _bmp = new(width, height);
            _grf = Graphics.FromImage(_bmp);
            _grf.SmoothingMode = SmoothingMode.HighQuality;
            _grf.InterpolationMode = InterpolationMode.HighQualityBicubic;
        }

        public void SetDrawingZone(float xStart, float xStop, float yStart, float yStop)
        {
            var x = _maxWidth;
            var y = _maxHeight;
            _drawingZone = new(
                x: (int)(x * xStart) + _offset,
                y: (int)(y * yStart) + _offset,
                width: (int)(x * (xStop - xStart)) - _offset,
                height: (int)(y * (yStop - yStart)) - _offset
            );
        }

        public void DrawPoint(float x, float y, int size, Shape shape, Color color)
        {
            var brush = GetBrush(color);
            switch (shape)
            {
                case Shape.Circle:
                    _grf.FillEllipse(brush,
                        _drawingZone.X + _drawingZone.Width * x - size / 2,
                        _drawingZone.Y + _drawingZone.Height * y - size / 2,
                        size, size);
                    break;

                case Shape.Diamond:
                    _grf.FillRectangle(brush,
                        _drawingZone.X + _drawingZone.Width * x - size / 2,
                        _drawingZone.Y + _drawingZone.Height * y - size / 2,
                        size, size);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        public void DrawLine(float x1, float y1, float x2, float y2, int size, Color color)
        {
            _grf.DrawLine(new Pen(GetBrush(color), size),
                new Point((int)(_drawingZone.X + _drawingZone.Width * x1), (int)(_drawingZone.Y + _drawingZone.Height * y1)),
                new Point((int)(_drawingZone.X + _drawingZone.Width * x2), (int)(_drawingZone.Y + _drawingZone.Height * y2)));
        }

        public void DrawText(float x, float y, string text)
        {
            _grf.DrawString(text, new Font("Arial", 10), GetBrush(Color.Black),
                new Rectangle((int)(_drawingZone.X + _drawingZone.Width * x), (int)(_drawingZone.Y + _drawingZone.Height * y), 100, 100));
        }

        public void DrawRectangle(float x, float y, float w, float h, int size, Color color)
        {
            _grf.DrawRectangle(new(GetBrush(color), size),
                new(
                    (int)(_drawingZone.X + _drawingZone.Width * x),
                    (int)(_drawingZone.Y + _drawingZone.Height * y),
                    (int)(_drawingZone.Width * w),
                    (int)(_drawingZone.Height * h)
                )
            );
        }

        /// <summary>
        /// Get the current brush given a point
        /// Allow to store brushes so we don't recreate them everytimes
        /// </summary>
        private Brush GetBrush(Color color)
        {
            if (!brushes.ContainsKey(color))
            {
                var brush = new SolidBrush(color);
                brushes.Add(color, brush);
                return brush;
            }
            return brushes[color];
        }

        internal Bitmap GetBitmap()
            => _bmp;

        public int _maxWidth, _maxHeight;
        public Rectangle _drawingZone;
        private int _offset;
        private readonly Bitmap _bmp;
        private readonly Graphics _grf;

        private readonly Dictionary<Color, Brush> brushes = new();
    }
}
