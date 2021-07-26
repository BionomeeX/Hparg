using System;
using System.Collections.Generic;
using System.Linq;

namespace Hparg
{
    public class Plot
    {
        /// <summary>
        /// Create a new plot
        /// </summary>
        /// <param name="x">Values for the X coordinate</param>
        /// <param name="y">Values for the Y coordinate</param>
        /// <param name="color">Color to render the points in</param>
        /// <param name="xMin">Start point of the graph for the width, null if dynamic</param>
        /// <param name="xMax">End point of the graph for the width, null if dynamic</param>
        /// <param name="yMin">Start point of the graph for the height, null if dynamic</param>
        /// <param name="yMax">End point of the graph for the height, null if dynamic</param>
        /// <param name="offset">Offset for the start/end points</param>
        /// <param name="shape">Shape of the points</param>
        /// <param name="size">Size of the points</param>
        public Plot(float[] x, float[] y, System.Drawing.Color color, float? xMin = null, float? xMax = null, float? yMin = null, float? yMax = null,
            float offset = 5, Shape shape = Shape.Circle, int size = 5)
        {
            if (x.Length != y.Length)
            {
                throw new ArgumentException("x must be of the same length of y", nameof(x));
            }

            // Calculate the bounds if dynamic, else use the ones given in parameter
            _xMin = new(xMin ?? _points.Min(p => p.X), xMin == null);
            _xMax = new(xMin ?? _points.Max(p => p.X), xMin == null);
            _yMin = new(xMin ?? _points.Min(p => p.Y), xMin == null);
            _yMax = new(xMin ?? _points.Max(p => p.Y), xMin == null);
            _offset = offset;

            // Add all the points
            _points.AddRange(Enumerable.Range(0, x.Length).Select(i =>
            {
                return new Point
                {
                    X = x[i],
                    Y = y[i],
                    Color = color,
                    Shape = shape,
                    Size = size
                };
            }));
        }
        public void AddPoint(float x, float y, System.Drawing.Color color, Shape shape = Shape.Circle, int size = 5)
        {
            // Recalculate all bounds if set they are set to dynamic
            if (_xMin.IsDynamic)
            {
                _xMin.Value = _points.Min(p => p.X);
            }
            if (_xMax.IsDynamic)
            {
                _xMax.Value = _points.Max(p => p.X);
            }
            if (_yMin.IsDynamic)
            {
                _yMin.Value = _points.Min(p => p.Y);
            }
            if (_yMax.IsDynamic)
            {
                _xMax.Value = _points.Max(p => p.Y);
            }

            // Add the point to the graph
            _points.Add(new Point() { X = x, Y = y, Color = color, Shape = shape, Size = size });
        }

        internal int[][] GetRenderData(int width, int height)
        {
            if (_points.Count == 0)
            {
                return Array.Empty<int[]>();
            }

            float xRatio = width / _xMax.Value;
            float yRatio = height / _yMax.Value;

            int[][] data = new int[height][];
            for (int y = 0; y < height; y++)
            {
                data[y] = new int[width];
            }

            foreach (var point in _points)
            {
                int x = (int)((point.X * xRatio) - _xMin.Value);
                int y = (int)((point.Y * yRatio) - _yMin.Value);
                data[y][x] = point.Color.ToArgb();
            }

            return data;
        }

        private readonly List<Point> _points = new();
        private readonly DynamicLimit _xMin, _xMax, _yMin, _yMax;
        private readonly float _offset;
    }
}
