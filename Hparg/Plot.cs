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
            float offset = 50, Shape shape = Shape.Circle, int size = 2)
        {
            if (x.Length != y.Length)
            {
                throw new ArgumentException("x must be of the same length of y", nameof(x));
            }

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

            // Calculate the bounds if dynamic, else use the ones given in parameter
            _xMin = new(xMin ?? _points.Min(p => p.X), xMin == null);
            _xMax = new(xMin ?? _points.Max(p => p.X), xMin == null);
            _yMin = new(xMin ?? _points.Min(p => p.Y), xMin == null);
            _yMax = new(xMin ?? _points.Max(p => p.Y), xMin == null);
            _offset = offset;
            _shapeSize = size;
        }
        public void AddPoint(float x, float y, System.Drawing.Color color, Shape shape = Shape.Circle, int size = 5)
        {
            // Add the point to the graph
            _points.Add(new Point() { X = x, Y = y, Color = color, Shape = shape, Size = size });

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
                int x = (int)((width - 2 * _offset - 1) * (point.X - _xMin.Value) / (_xMax.Value - _xMin.Value));
                int y = (int)((height - 2 * _offset - 1) * (point.Y - _yMin.Value) / (_yMax.Value - _yMin.Value));
                switch (point.Shape)
                {
                    case Shape.Circle:
                        for (int line = -point.Size; line <= point.Size; ++line)
                        {
                            for (int col = -point.Size; col <= point.Size; ++col)
                            {
                                if (col * col + line * line <= point.Size * point.Size)
                                {
                                    if (x + col >= 0 && y + line >= 0 && x + col < width && y + line < height)
                                    {
                                        data[(int)(y + line + _offset)][(int)(x + col + _offset)] = point.Color.ToArgb();
                                    }
                                }
                            }
                        }
                        break;
                    case Shape.Diamond:
                        for (int line = -point.Size; line <= point.Size; ++line)
                        {
                            for (int col = -point.Size; col <= point.Size; ++col)
                            {
                                if (Math.Abs(col) + Math.Abs(line) <= point.Size)
                                {
                                    if (x + col >= 0 && y + line >= 0 && x + col < width && y + line < height)
                                    {
                                        data[(int)(y + line + _offset)][(int)(x + col + _offset)] = point.Color.ToArgb();
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            return data;
        }

        private readonly List<Point> _points = new();
        private readonly DynamicBoundary _xMin, _xMax, _yMin, _yMax;
        private readonly float _offset;
        private readonly int _shapeSize;
    }
}
