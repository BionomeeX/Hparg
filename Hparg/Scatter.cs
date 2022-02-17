using Hparg.Drawable;
using Hparg.Plot;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Hparg
{
    public class Scatter : APlot<Vector2>
    {
        public Scatter(float[] x, float[] y, System.Drawing.Color color, float? xMin = null, float? xMax = null, float? yMin = null, float? yMax = null,
            Shape shape = Shape.Circle, int size = 2, int lineSize = 2, Action<IEnumerable<Vector2>> callback = null)
            : base(null, callback)
        {
            _points = Enumerable.Range(0, x.Length).Select(i =>
            {
                return new Point<float, float>
                {
                    X = x[i],
                    Y = y[i],
                    Color = color,
                    Shape = shape,
                    Size = size
                };
            }).ToList();

            // Calculate the bounds if dynamic, else use the ones given in parameter
            _xMin = new(xMin ?? _points.Min(p => p.X), xMin == null);
            _xMax = new(xMax ?? _points.Max(p => p.X), xMax == null);
            Min = _points.Min(p => p.Y);
            Max = _points.Max(p => p.Y);

            if (float.IsNaN(_xMin.Value) || float.IsNaN(Min))
            {
                throw new ArgumentException("x and y can't contains NaN values");
            }
        }

        public void AddPoint(float x, float y, System.Drawing.Color color, Shape shape = Shape.Circle, int size = 5)
        {
            _points.Add(new()
            {
                X = x,
                Y = y,
                Color = color,
                Shape = shape,
                Size = size
            });

            // Recalculate all bounds if set they are set to dynamic
            if (_xMin.IsDynamic)
            {
                _xMin.Value = Math.Min(_xMin.Value, x);
            }
            if (_xMax.IsDynamic)
            {
                _xMax.Value = Math.Max(_xMax.Value, x);
            }
            Min = Math.Min(Min, y);
            Max = Math.Max(Max, y);
        }

        private (float x, float y) GetCoordinate(float oX, float oY)
        {
            var dX = _xMax.Value - _xMin.Value;
            var dY = Max - Min;
            var x = dX == 0 ? 0f : (oX - _xMin.Value) / dX;
            var y = dY == 0 ? 0f : (1f - (oY - Min) / dY);

            return (x, y);
        }

        internal override void Render(Canvas canvas, Zone drawingZone)
        {
            for (int i = 0; i < _points.Count; i++)
            {
                var point = _points[i];

                (float x, float y) = GetCoordinate(point.X, point.Y);
                canvas.DrawPoint(drawingZone, x, y, point.Size, point.Shape, new Rgba32(point.Color.R, point.Color.G, point.Color.B, point.Color.A));

                if (i < _points.Count - 1)
                {
                    var next = _points[i + 1];
                    (float nX, float nY) = GetCoordinate(next.X, next.Y);
                    canvas.DrawLine(drawingZone, x, y, nX, nY, point.Size, new Rgba32(point.Color.R, point.Color.G, point.Color.B, point.Color.A));
                }
            }
        }

        internal override (float X, float Y) ToRelativeSpace(float x, float y)
        {
            return GetCoordinate(x, y);
        }

        internal override IEnumerable<Vector2> GetPointsInRectangle(float x, float y, float w, float h)
        {
            return _points
                .Select(p => GetCoordinate(p.X, p.Y))
                .Where(p => p.x >= x && p.x <= x + w && p.y >= y && p.y <= y + h)
                .Select(p => new Vector2(p.x, p.y));
        }

        private readonly DynamicBoundary _xMin, _xMax;
        private readonly List<Point<float, float>> _points;
    }
}
