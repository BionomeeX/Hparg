﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace Hparg.Plot
{
    public abstract class APlot
    {
        /// <summary>
        /// Create a new plot
        /// </summary>
        /// <param name="x">Values for the X coordinate</param>
        /// <param name="y">Values for the Y coordinate</param>
        /// <param name="color">Color to render the points in</param>
        /// <param name="offset">Offset for the start/end points</param>
        /// <param name="shape">Shape of the points</param>
        /// <param name="size">Size of the points</param>
        private protected APlot(IEnumerable<Point> points, float offset = 50, int lineSize = 2)
        {
            _points.AddRange(points);

            _lineSize = lineSize;
            _offset = offset;
        }
        public virtual void AddPoint(float x, float y, Color color, Shape shape = Shape.Circle, int size = 5)
        {
            // Add the point to the graph
            _points.Add(new Point() { X = x, Y = y, Color = color, Shape = shape, Size = size });
        }

        public void AddVerticalLine(int x, Color color, int size = 2)
        {
            _lines.Add(new() { Position = x, Color = color, Size = size, Orientation = Orientation.Vertical });
        }

        public void AddHorizontalLine(int y, Color color, int size = 2)
        {
            _lines.Add(new() { Position = y, Color = color, Size = size, Orientation = Orientation.Horizontal });
        }

        /// <summary>
        /// Calculate the local coordinate of a point
        /// </summary>
        /// <param name="point">Point in global coordinate</param>
        /// <param name="width">Width of the window</param>
        /// <param name="height">Height of the window</param>
        /// <returns>Tuple containing the X and Y position of the point in local coordinate</returns>
        internal abstract (int x, int y) CalculateCoordinate(Point point, int width, int height);
        /// <summary>
        /// Get all the data to render on screen
        /// </summary>
        /// <param name="width">Width of the window</param>
        /// <param name="height">Height of the window</param>
        /// <returns>Bitmap containing the points to render</returns>
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
            (int x, int y)? lastPos = null;

            foreach (var point in _points)
            {
                var brush = GetBrush(point);
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

            foreach (var line in _lines)
            {
                Point point = line.Orientation == Orientation.Horizontal
                    ? new() { Color = line.Color, X = 0, Y = line.Position }
                    : new() { Color = line.Color, Y = 0, X = line.Position };
                Point otherPoint = line.Orientation == Orientation.Horizontal
                    ? new() { Color = line.Color, X = width, Y = line.Position }
                    : new() { Color = line.Color, Y = height, X = line.Position };

                var brush = GetBrush(point);
                var pos = CalculateCoordinate(point, width, height);
                var otherPos = CalculateCoordinate(otherPoint, width, height);

                grf.DrawLine(new Pen(brush, line.Size), new System.Drawing.Point(pos.x, pos.y), new System.Drawing.Point(otherPos.x, otherPos.y));
            }

            return bmp;
        }

        /// <summary>
        /// Get the current brush given a point
        /// Allow to store brushes so we don't recreate them everytimes
        /// </summary>
        private Brush GetBrush(Point point)
        {
            if (!brushes.ContainsKey(point.Color))
            {
                var brush = new SolidBrush(point.Color);
                brushes.Add(point.Color, brush);
                return brush;
            }
            return brushes[point.Color];
        }

        /// <summary>
        /// List of all points to display
        /// </summary>
        private protected readonly List<Point> _points = new();
        /// <summary>
        /// List of all points to display
        /// </summary>
        private readonly List<Line> _lines = new();
        protected readonly float _offset;
        private readonly int _lineSize;

        private readonly Dictionary<Color, Brush> brushes = new();
    }
}
