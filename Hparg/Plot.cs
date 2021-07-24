using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Hparg
{
    public class Plot
    {
        public void AddPoint(float x, float y, Color color, Shape shape = Shape.Circle, int size = 5)
        {
            _points.Add(new Point() { X = x, Y = y, Color = color, Shape = shape, Size = size });
        }

        public void AddPoints(float[] x, float[] y, Color color, Shape shape = Shape.Circle, int size = 5)
        {
            if (x.Length != y.Length)
            {
                throw new ArgumentException("x must be of the same length of y", nameof(x));
            }
            for (int i = 0; i < x.Length; i++)
            {
                AddPoint(x[i], y[i], color, shape, size);
            }
        }

        public void Clear()
        {
            _points.Clear();
        }

        internal int[][] GetRenderData(int width, int height)
        {
            if (_points.Count == 0)
            {
                return Array.Empty<int[]>();
            }
            float xMin = _points.Min(x => x.X);
            float xMax = _points.Max(x => x.X);
            float yMin = _points.Min(y => y.Y);
            float yMax = _points.Max(y => y.Y);
            if (xMin < 0)
            {
                xMax += xMin;
                xMin = 0;
            }
            if (yMin < 0)
            {
                yMax += yMin;
                yMin = 0;
            }

            float xRatio = width / (xMax - xMin);
            float yRatio = height / (yMax - yMin);

            int[][] data = new int[height][];
            for (int y = 0; y < height; y++)
            {
                data[y] = new int[width];
            }

            foreach (var point in _points)
            {
                int x = (int)((point.X * xRatio) - xMin);
                int y = (int)((point.Y * yRatio) - yMin);
                data[y][x] = point.Color.ToArgb();
            }

            return data;
        }

        private readonly List<Point> _points = new();
    }
}
