using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Hparg.Drawable
{
    public class Canvas : ICloneable // TODO: Dispose?
    {
        internal Canvas(int width, int height, int offset, int mainSurfaceCount = 1)
            : this(width, height, offset, offset, offset, offset, mainSurfaceCount)
        { }

        private Canvas()
        { }

        private float GetOffset(int o, int max)
            => (float)o / max;

        internal float GetWidth(Zone zone)
            => _zones[zone].Width;
        internal float GetHeight(Zone zone)
            => _zones[zone].Height;

        internal Canvas(int width, int height, int leftOffset, int rightOffset, int upOffset, int downOffset, int mainSurfaceCount = 1)
        {
            MaxWidth = width;
            MaxHeight = height;
            _zones = new()
            {
                {
                    Zone.LeftMargin,
                    new(
                        0f,
                        GetOffset(upOffset, MaxHeight),
                        GetOffset(leftOffset, MaxWidth),
                        1f - (GetOffset(upOffset, MaxHeight) + GetOffset(downOffset, MaxHeight))
                    )
                },
                {
                    Zone.RightMargin,
                    new(
                        1f - GetOffset(rightOffset, MaxWidth),
                        GetOffset(upOffset, MaxHeight),
                        GetOffset(rightOffset, MaxWidth),
                        1f - (GetOffset(upOffset, MaxHeight) + GetOffset(downOffset, MaxHeight))
                    )
                },
                {
                    Zone.UpperMarginFull,
                    new(
                        GetOffset(leftOffset, MaxWidth),
                        0f,
                        1f - (GetOffset(leftOffset, MaxHeight) + GetOffset(rightOffset, MaxWidth)),
                        GetOffset(upOffset, MaxHeight)
                    )
                },
                {
                    Zone.LowerMarginFull,
                    new(
                        GetOffset(leftOffset, MaxWidth),
                        1f - GetOffset(downOffset, MaxHeight),
                        1f - (GetOffset(leftOffset, MaxHeight) + GetOffset(rightOffset, MaxWidth)),
                        GetOffset(downOffset, MaxHeight)
                    )
                }
            };

            var mx = (1f - (GetOffset(leftOffset, MaxHeight) + GetOffset(rightOffset, MaxWidth))) / mainSurfaceCount;
            for (int i = 0; i < mainSurfaceCount * 3; i += 3)
            {
                // Main zone
                _zones.Add((Zone)(i + 1),
                    new(
                        x: GetOffset(leftOffset, MaxWidth) + mx * (i / 3),
                        y: GetOffset(upOffset, MaxHeight),
                        w: mx,
                        h: 1f - (GetOffset(upOffset, MaxHeight) + GetOffset(downOffset, MaxHeight))
                    )
                );

                // Margins
                _zones.Add(
                    (Zone)i,
                    new(
                        GetOffset(leftOffset, MaxWidth) + mx * (i / 3),
                        0f,
                        mx,
                        GetOffset(upOffset, MaxHeight)
                    )
                );
                _zones.Add(
                    (Zone)(i + 2),
                    new(
                        GetOffset(leftOffset, MaxWidth) + mx * (i / 3),
                        1f - GetOffset(downOffset, MaxHeight),
                        mx,
                        GetOffset(downOffset, MaxHeight)
                    )
                );
            }

            _img = new Image<Rgba32>(width, height);
        }

        private PointF Tr(Zone zone, float x, float y)
        {
            var l = _zones[zone].ToLocal(x, y);
            return new PointF(l.X * MaxWidth, l.Y * MaxHeight);
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
            VerticalAlignment verAlignment = VerticalAlignment.Center)
        {
            if (!SystemFonts.TryGet("Arial", out FontFamily font))
            {
                font = SystemFonts.Families.First();
            }
            _img.Mutate(i => i.DrawText(new TextOptions(font.CreateFont(size, FontStyle.Regular))
            {
                HorizontalAlignment = horAlignment,
                VerticalAlignment = verAlignment,
                Origin = Tr(zone, x, y)
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
                DrawText(Zone.LeftMargin, .9f, 1f - i / 10f, $"{value:0.00}", 15, HorizontalAlignment.Right);
            }
        }

        internal float GetOffset(Zone drawingZone, Direction direction, int pixelOffset)
        {
            var zone = _zones[drawingZone];
            return direction switch
            {
                Direction.Left => zone.X - (float)pixelOffset / MaxWidth,
                Direction.Right => zone.Width + (float)pixelOffset  / MaxWidth,
                Direction.Bottom => zone.Y - (float)pixelOffset / MaxHeight,
                Direction.Top => zone.Height + (float)pixelOffset / MaxHeight,
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

        public object Clone()
        {
            var cvs = new Canvas
            {
                MaxHeight = MaxHeight,
                MaxWidth = MaxWidth,
                _zones = new(_zones),
                _img = _img.Clone((_) => { })
            };
            return cvs;
        }

        public int MaxWidth { private set; get; }
        public int MaxHeight { private set; get; }
        private Dictionary<Zone, DrawingZone> _zones;
        private Image _img;

        internal enum Direction
        {
            Left,
            Right,
            Top,
            Bottom
        }
    }
}
