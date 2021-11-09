using Hparg.Drawable;
using Hparg.Plot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Hparg
{
    public class Scatter : APlot
    {
        public Scatter(float[] x, float[] y, System.Drawing.Color color, float? xMin = null, float? xMax = null, float? yMin = null, float? yMax = null,
            float offset = 50, Shape shape = Shape.Circle, int size = 2, int lineSize = 2, Action<IEnumerable<Vector2>> callback = null)
            : base(callback)
        {
            _points = Enumerable.Range(0, x.Length).Select(i =>
            {
                return new Point<float>
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
            _yMin = new(yMin ?? _points.Min(p => p.Y), yMin == null);
            _yMax = new(yMax ?? _points.Max(p => p.Y), yMax == null);

            if (float.IsNaN(_xMin.Value) || float.IsNaN(_yMin.Value))
            {
                throw new ArgumentException("x and y can't contains NaN values");
            }
        }

        public override void AddPoint(float x, float y, System.Drawing.Color color, Shape shape = Shape.Circle, int size = 5)
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
            if (_yMin.IsDynamic)
            {
                _yMin.Value = Math.Min(_yMin.Value, y);
            }
            if (_yMax.IsDynamic)
            {
                _yMax.Value = Math.Max(_yMax.Value, y);
            }
        }

        private (float x, float y) GetCoordinate(float oX, float oY)
        {
            var dX = _xMax.Value - _xMin.Value;
            var dY = _yMax.Value - _yMin.Value;
            var x = dX == 0 ? 0f : (oX - _xMin.Value) / dX + _offset;
            var y = dY == 0 ? 0f : (1f - (oY - _yMin.Value) / dY) + _offset;

            return (x, y);
        }

        internal override void Render(Canvas canvas)
        {
            for (int i = 0; i < _points.Count; i++)
            {
                var point = _points[i];

                (float x, float y) = GetCoordinate(point.X, point.Y);
                canvas.DrawPoint(x, y, point.Size, point.Shape, point.Color);

                if (i < _points.Count - 1)
                {
                    var next = _points[i + 1];
                    (float nX, float nY) = GetCoordinate(next.X, next.Y);
                    canvas.DrawLine(x, y, nX, nY, point.Size, point.Color);
                }
            }
        }

        internal override (float X, float Y) ToRelativeSpace(float x, float y)
        {
            return GetCoordinate(x, y);
        }

        private readonly DynamicBoundary _xMin, _xMax, _yMin, _yMax;
        private readonly List<Point<float>> _points;
    }
}
