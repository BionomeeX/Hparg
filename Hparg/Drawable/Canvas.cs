using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;

namespace Hparg.Drawable
{
    public class Canvas // TODO: Dispose?
    {
        internal Canvas(int width, int height, int offset)
        {
            _maxWidth = width;
            _maxHeight = height;
            var wOffset = (_maxWidth - (float)offset) / _maxWidth;
            var hOffset = (_maxHeight - (float)offset) / _maxHeight;
            _zones = new()
            {
                { Zone.Main,        new(wOffset, hOffset, 1f - 2f * wOffset, 1f - 2f * hOffset) },
                { Zone.LeftMargin,  new(0f, 0f, wOffset, 1f)},
                { Zone.UpperMargin, new(0f, 0f, hOffset, 1f)},
                { Zone.RightMargin, new(1f - wOffset, 0f, wOffset, 1f)},
                { Zone.LowerMargin, new(0f, 1f - hOffset, 1f, hOffset)}
            };

            _img = new Image<Rgba32>(width, height);
        }

        private PointF Tr(Zone zone, float x, float y)
        {
            var l = _zones[zone].ToLocal(x, y);
            return new PointF(l.X * _maxWidth, l.Y * _maxHeight);
        }

        internal void DrawPoint(Zone zone, float x, float y, int size, Shape shape, Color color)
        {
            RegularPolygon point = shape switch
            {
                Shape.Circle => new RegularPolygon(
                    Tr(zone, x, y),
                    50,
                    size),
                Shape.Diamond => new RegularPolygon(
                    Tr(zone, x, y),
                    4,
                    size),
                _ => throw new NotImplementedException(),
            };
            _img.Mutate(x => x.Fill(color, point));
        }

        internal void DrawLine(Zone zone, float x1, float y1, float x2, float y2, int size, Color color)
        {
            PathBuilder path = new();
            path.AddLine(Tr(zone, x1, y1), Tr(zone, x2, y2));
            _img.Mutate(x => x.Draw(new Pen(color, size), path.Build()));
        }

        internal void DrawText(Zone zone, float x, float y, string text)
        {
            _img.Mutate(i => i.DrawText(text, SystemFonts.CreateFont("Arial", 16, FontStyle.Regular), Color.Black, Tr(zone, x, y)));
        }

        internal void DrawRectangle(Zone zone, float x, float y, float w, float h, int size, Color color)
        {
            var n = Tr(zone, x + w, y + h) - Tr(zone, x, y);
            var rect = new RectangleF(Tr(zone, x, y), new SizeF(n.X, n.Y));
            _img.Mutate(x => x.Draw(new Pen(color, size), rect));
        }

        internal void DrawAxis(float min, float max)
        {
            DrawLine(Zone.Main, 0f, 1f, 1f, 1f, 2, Color.Black);
            DrawLine(Zone.Main, 0f, 0f, 0f, 1f, 2, Color.Black);
            DrawText(Zone.Main, 0f, 1f, $"{min}");
            DrawText(Zone.Main, 0f, 0f, $"{max}");
        }

        internal MemoryStream ToStream()
        {
            var stream = new MemoryStream();
            _img.Save(stream, new PngEncoder());
            stream.Position = 0;
            return stream;
        }

        public int _maxWidth, _maxHeight;
        private Dictionary<Zone, DrawingZone> _zones;
        private readonly Image _img;
    }
}
