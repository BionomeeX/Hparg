using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;

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

            _img = new Image<Rgba32>(width, height);
        }

        internal void SetDrawingZone(float xStart, float xStop, float yStart, float yStop)
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

        internal void DrawPoint(float x, float y, int size, Shape shape, Color color)
        {
            RegularPolygon point = shape switch
            {
                Shape.Circle => new RegularPolygon(
                    _drawingZone.X + _drawingZone.Width * x - size / 2,
                    _drawingZone.Y + _drawingZone.Height * y - size / 2,
                    50,
                    size),
                Shape.Diamond => new RegularPolygon(
                    _drawingZone.X + _drawingZone.Width * x - size / 2,
                    _drawingZone.Y + _drawingZone.Height * y - size / 2,
                    4,
                    size),
                _ => throw new NotImplementedException(),
            };
            _img.Mutate(x => x.Fill(color, point));
        }

        internal void DrawLine(float x1, float y1, float x2, float y2, int size, Color color)
        {
            PathBuilder path = new();
            path.AddLine(new PointF(_drawingZone.X + _drawingZone.Width * x1, _drawingZone.Y + _drawingZone.Height * y1),
                new PointF(_drawingZone.X + _drawingZone.Width * x2, _drawingZone.Y + _drawingZone.Height * y2));
            _img.Mutate(x => x.Draw(new Pen(color, size), path.Build()));
        }

        internal void DrawText(float x, float y, string text)
        {
            _img.Mutate(i => i.DrawText(text, SystemFonts.CreateFont("Arial", 16, FontStyle.Regular), Color.Black, new PointF(x, y)));
        }

        internal void DrawRectangle(float x, float y, float w, float h, int size, Color color)
        {
            var rect = new Rectangle(
                (int)(_drawingZone.X + _drawingZone.Width * x),
                (int)(_drawingZone.Y + _drawingZone.Height * y),
                (int)(_drawingZone.Width * w),
                (int)(_drawingZone.Height * h));
            _img.Mutate(x => x.Draw(new Pen(color, size), rect));
        }

        internal void DrawAxis(float min, float max)
        {
            SetDrawingZone(0f, 1f, 0f, 1f);
            DrawLine(0f, 1f, 1f, 1f, 2, Color.Black);
            DrawLine(0f, 0f, 0f, 1f, 2, Color.Black);
            DrawText(0f, 1f, $"{min}");
            DrawText(0f, 0f, $"{max}");
        }

        internal MemoryStream ToStream()
        {
            var stream = new MemoryStream();
            _img.Save(stream, new PngEncoder());
            stream.Position = 0;
            return stream;
        }

        public int _maxWidth, _maxHeight;
        public Rectangle _drawingZone;
        private int _offset;
        private readonly Image _img;
    }
}
