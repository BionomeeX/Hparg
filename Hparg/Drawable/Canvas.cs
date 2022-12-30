using Hparg.Plot;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Globalization;

namespace Hparg.Drawable
{
    public class Canvas // TODO: Dispose?
    {
        internal Canvas(int width, int height, int offset, int mainSurfaceCount = 1)
            : this(width, height, offset, offset, offset, offset, mainSurfaceCount)
        { }

        internal Canvas(Canvas cvs)
        {
            _maxWidth = cvs._maxWidth;
            _maxHeight = cvs._maxHeight;
            _zones = new(cvs._zones);
            _img = cvs._img.Clone((_) => { });
        }

        internal Canvas(int width, int height, int leftOffset, int rightOffset, int upOffset, int downOffset, int mainSurfaceCount = 1)
        {
            _maxWidth = width;
            _maxHeight = height;
            _zones = new()
            {
                {
                    Zone.LeftMargin,
                    new(
                        0f,
                        GetOffset(upOffset, _maxHeight),
                        GetOffset(leftOffset, _maxWidth),
                        1f - (GetOffset(upOffset, _maxHeight) + GetOffset(downOffset, _maxHeight))
                    )
                },
                {
                    Zone.RightMargin,
                    new(
                        1f - GetOffset(rightOffset, _maxWidth),
                        GetOffset(upOffset, _maxHeight),
                        GetOffset(rightOffset, _maxWidth),
                        1f - (GetOffset(upOffset, _maxHeight) + GetOffset(downOffset, _maxHeight))
                    )
                },
                {
                    Zone.UpperMarginFull,
                    new(
                        GetOffset(leftOffset, _maxWidth),
                        0f,
                        1f - (GetOffset(leftOffset, _maxHeight) + GetOffset(rightOffset, _maxWidth)),
                        GetOffset(upOffset, _maxHeight)
                    )
                },
                {
                    Zone.LowerMarginFull,
                    new(
                        GetOffset(leftOffset, _maxWidth),
                        1f - GetOffset(downOffset, _maxHeight),
                        1f - (GetOffset(leftOffset, _maxHeight) + GetOffset(rightOffset, _maxWidth)),
                        GetOffset(downOffset, _maxHeight)
                    )
                }
            };

            var mx = (1f - (GetOffset(leftOffset, _maxHeight) + GetOffset(rightOffset, _maxWidth))) / mainSurfaceCount;
            for (int i = 0; i < mainSurfaceCount * 3; i += 3)
            {
                // Main zone
                _zones.Add((Zone)(i + 1),
                    new(
                        x: GetOffset(leftOffset, _maxWidth) + mx * (i / 3),
                        y: GetOffset(upOffset, _maxHeight),
                        w: mx,
                        h: 1f - (GetOffset(upOffset, _maxHeight) + GetOffset(downOffset, _maxHeight))
                    )
                );

                // Margins
                _zones.Add(
                    (Zone)i,
                    new(
                        GetOffset(leftOffset, _maxWidth) + mx * (i / 3),
                        0f,
                        mx,
                        GetOffset(upOffset, _maxHeight)
                    )
                );
                _zones.Add(
                    (Zone)(i + 2),
                    new(
                        GetOffset(leftOffset, _maxWidth) + mx * (i / 3),
                        1f - GetOffset(downOffset, _maxHeight),
                        mx,
                        GetOffset(downOffset, _maxHeight)
                    )
                );
            }

            _img = new Image<Rgba32>(width, height);
        }

        private float GetOffset(int o, int max)
            => (float)o / max;

        internal float GetWidth(Zone zone)
            => _zones[zone].Width;
        internal float GetHeight(Zone zone)
            => _zones[zone].Height;

        private PointF Tr(Zone zone, float x, float y)
        {
            var l = _zones[zone].ToLocal(x, y);
            return new PointF(l.X * _maxWidth, l.Y * _maxHeight);
        }

        internal PointF GetSize(Zone zone, float globalWidth, float globalHeight)
        {
            var z = _zones[zone];
            return new PointF(z.Width * globalWidth, z.Height * globalHeight);
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

        internal void DrawText(Zone zone, float x, float y, string text, int size,
            HorizontalAlignment horAlignment = HorizontalAlignment.Center,
            VerticalAlignment verAlignment = VerticalAlignment.Center,
            float rotation = 0f)
        {
            var pos = Tr(zone, x, y);
            _img.Mutate(i => i
            .SetDrawingTransform(Matrix3x2Extensions.CreateRotationDegrees(rotation, pos))
            .DrawText(new TextOptions(FontManager.Instance.GetFont(size))
            {
                HorizontalAlignment = horAlignment,
                VerticalAlignment = verAlignment,
                Origin = pos
            }, text, Color.Black));
        }

        internal void DrawRectangle(Zone zone, float x, float y, float w, float h, int size, Color color, bool doesFill)
        {
            var n = Tr(zone, x + w, y + h) - Tr(zone, x, y);
            var rect = new RectangleF(Tr(zone, x, y), new SizeF(n.X, n.Y));
            if (doesFill)
            {
                _img.Mutate(x => x.Fill(color, rect));
            }
            else
            {
                _img.Mutate(x => x.Draw(new Pen(color, size), rect));
            }
        }

        internal void DrawAxis(float min, float max)
        {
            DrawLine(Zone.LeftMargin, 1f, 0f, 1f, 1f, 2, Color.Black);
            DrawLine(Zone.LowerMarginFull, 0f, 0f, 1f, 0f, 2, Color.Black);
            var relativeMax = max - min;
            for (int i = 0; i <= 10; i++)
            {
                var value = (relativeMax / 10f) * i + min;
                var y = 1f - i / 10f;
                DrawText(Zone.LeftMargin, .9f, y, Utils.FormatNumber(value), 15, HorizontalAlignment.Right);
                //DrawLine(Zone.Main, 0f, y, 1f, y, 1, Color.Gray);
            }
        }

        internal float GetOffset(Zone drawingZone, Direction direction, int pixelOffset)
        {
            var zone = _zones[drawingZone];
            return direction switch
            {
                Direction.Left => zone.X - (float)pixelOffset / _maxWidth,
                Direction.Right => zone.Width + (float)pixelOffset  / _maxWidth,
                Direction.Bottom => zone.Y - (float)pixelOffset / _maxHeight,
                Direction.Top => zone.Height + (float)pixelOffset / _maxHeight,
                _ => throw new NotImplementedException()
            };
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

        internal enum Direction
        {
            Left,
            Right,
            Top,
            Bottom
        }
    }
}
