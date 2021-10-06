using Hparg.Plot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace Hparg
{
    public class Scatter : APlot
    {
        public Scatter(float[] x, float[] y, Color color, float? xMin = null, float? xMax = null, float? yMin = null, float? yMax = null,
            float offset = 50, Shape shape = Shape.Circle, int size = 2, int lineSize = 2, Action<IEnumerable<Vector2>> callback = null)
            : base(Enumerable.Range(0, x.Length).Select(i =>
            {
                return new Plot.Point<float>
                {
                    X = x[i],
                    Y = y[i],
                    Color = color,
                    Shape = shape,
                    Size = size
                };
            }), offset, lineSize, callback)
        {
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

        public override void AddPoint(float x, float y, Color color, Shape shape = Shape.Circle, int size = 5)
        {
            base.AddPoint(x, y, color, shape, size);

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

        internal override (int x, int y) CalculateCoordinate(Plot.Point<float> point, int width, int height)
        {
            var dX = _xMax.Value - _xMin.Value;
            var dY = _yMax.Value - _yMin.Value;
            int x = dX == 0 ? 0 : (int)((width - 2 * _offset - 1) * (point.X - _xMin.Value) / dX + _offset);
            int y = dY == 0 ? 0 : (int)((height - 2 * _offset - 1) * (1f - (point.Y - _yMin.Value) / dY) + _offset);
            return (x, y);
        }

        private readonly DynamicBoundary _xMin, _xMax, _yMin, _yMax;
    }
}
