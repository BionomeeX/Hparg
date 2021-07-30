﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        public Plot(float[] x, float[] y, Color color, float? xMin = null, float? xMax = null, float? yMin = null, float? yMax = null,
            float offset = 50, Shape shape = Shape.Circle, int size = 2, int lineSize = 2)
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
            _xMax = new(xMax ?? _points.Max(p => p.X), xMax == null);
            _yMin = new(yMin ?? _points.Min(p => p.Y), yMin == null);
            _yMax = new(yMax ?? _points.Max(p => p.Y), yMax == null);

            if (float.IsNaN(_xMin.Value) || float.IsNaN(_yMin.Value))
            {
                throw new ArgumentException("x and y can't contains NaN values");
            }

            _lineSize = lineSize;
            _offset = offset;
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

        internal Bitmap GetRenderData(int width, int height)
        {
            var bmp = new Bitmap(width, height);

            if (_points.Count == 0)
            {
                return bmp;
            }

            using Graphics grf = Graphics.FromImage(bmp);
            grf.SmoothingMode = SmoothingMode.HighQuality;
            grf.InterpolationMode = InterpolationMode.HighQualityBicubic;

            Dictionary<Color, Brush> brushes = new();
            (int x, int y)? lastPos = null;

            foreach (var point in _points)
            {
                Brush brush;
                if (!brushes.ContainsKey(point.Color))
                {
                    brush = new SolidBrush(point.Color);
                    brushes.Add(point.Color, brush);
                }
                else
                {
                    brush = brushes[point.Color];
                }

                var pos = CalculateCoordinate(point, width, height);
                switch (point.Shape)
                {
                    case Shape.Circle:
                        grf.FillEllipse(brush, pos.x - point.Size / 2, pos.y - point.Size / 2, point.Size, point.Size);
                        break;

                    case Shape.Diamond:
                        grf.FillRectangle(brush, pos.x - point.Size / 2, pos.y - point.Size / 2, point.Size, point.Size);
                        break;

                    default:
                        throw new NotImplementedException();
                }

                if (_lineSize > 0 && lastPos.HasValue)
                {
                    grf.DrawLine(new Pen(brush, _lineSize),
                        new System.Drawing.Point(pos.x, pos.y),
                        new System.Drawing.Point(lastPos.Value.x, lastPos.Value.y));
                }
                lastPos = pos;
            }

            return bmp;
        }

        private (int x, int y) CalculateCoordinate(Point point, int width, int height)
        {
            int x = (int)((width - 2 * _offset - 1) * (point.X - _xMin.Value) / (_xMax.Value - _xMin.Value) + _offset);
            int y = (int)((height - 2 * _offset - 1) * (1f - (point.Y - _yMin.Value) / (_yMax.Value - _yMin.Value)) + _offset );
            return (x, y);
        }

        private readonly List<Point> _points = new();
        private readonly DynamicBoundary _xMin, _xMax, _yMin, _yMax;
        private readonly float _offset;
        private readonly int _lineSize;
    }
}
