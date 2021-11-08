using Hparg.Plot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Hparg.Drawable
{
    public class Canvas : IDisposable
    {
        internal Canvas(int width, int height)
        {
            (Width, Height, _bmp, _grf) = (width, height, new(width, height), Graphics.FromImage(_bmp));
            _grf.SmoothingMode = SmoothingMode.HighQuality;
            _grf.InterpolationMode = InterpolationMode.HighQualityBicubic;
        }

        public void DrawPoint(float x, float y, int size, Shape shape, Color color)
        {
            var brush = GetBrush(color);
            switch (shape)
            {
                case Shape.Circle:
                    _grf.FillEllipse(brush, x - size / 2, y - size / 2, size, size);
                    break;

                case Shape.Diamond:
                    _grf.FillRectangle(brush, x - size / 2, y - size / 2, size, size);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        public void DrawLine(int x1, int y1, int x2, int y2, int size, Color color)
        {
            _grf.DrawLine(new Pen(GetBrush(color), size),
                new Point(x1, y1),
                new Point(x2, y2));
        }

        public void DrawRectangle(Rectangle rect, int size, Color color)
        {
            _grf.DrawRectangle(new(GetBrush(color), size), rect);
        }

        public void Dispose()
        {
            _grf.Dispose();
            _bmp.Dispose();
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

        public int Width { private init; get; }
        public int Height { private init; get; }
        private Bitmap _bmp;
        private Graphics _grf;

        private readonly Dictionary<Color, Brush> brushes = new();
    }
}
