using Hparg.Drawable;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;

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
        private protected APlot(Action<IEnumerable<Vector2>> callback)
        {
            _callback = callback;
        }
        public abstract void AddPoint(float x, float y, Color color, Shape shape = Shape.Circle, int size = 5);

        public void AddVerticalLine(int x, Color color, int size = 2)
        {
            _lines.Add(new() { Position = x, Color = color, Size = size, Orientation = Orientation.Vertical });
        }

        public void AddHorizontalLine(int y, Color color, int size = 2)
        {
            _lines.Add(new() { Position = y, Color = color, Size = size, Orientation = Orientation.Horizontal });
        }

        internal abstract void Render(Canvas canvas);
        /// <summary>
        /// Get all the data to render on screen
        /// </summary>
        /// <param name="width">Width of the window</param>
        /// <param name="height">Height of the window</param>
        /// <returns>Bitmap containing the points to render</returns>
        internal Bitmap GetRenderData(int width, int height)
        {
            var cvs = new Canvas(width, height);

            Render(cvs);

            if (_dragAndDropSelection.HasValue)
            {
                var xMin = (int)Math.Min(_dragAndDropSelection.Value.start.X, _dragAndDropSelection.Value.end.X);
                var yMin = (int)Math.Min(_dragAndDropSelection.Value.start.Y, _dragAndDropSelection.Value.end.Y);
                var xMax = (int)Math.Max(_dragAndDropSelection.Value.start.X, _dragAndDropSelection.Value.end.X);
                var yMax = (int)Math.Max(_dragAndDropSelection.Value.start.Y, _dragAndDropSelection.Value.end.Y);
                cvs.DrawRectangle(new Rectangle(xMin, yMin, xMax - xMin, yMax - yMin), 1, Color.Red);
            }

            return cvs.GetBitmap();
        }

        private IEnumerable<Vector2> GetPointsInRectangle(Vector2 topLeft, Vector2 bottomRight, float width, float height)
            => Array.Empty<Vector2>();
            /*=> _points
                .Where(p => {
                    var nP = CalculateCoordinate(p, (int)width, (int)height);
                    var rP = new Vector2(nP.x / width, nP.y / height);
                    return rP.X >= topLeft.X && rP.X <= bottomRight.X && rP.Y >= topLeft.Y && rP.Y <= bottomRight.Y;
                })
                .Select(p => new Vector2(p.X, p.Y));*/

        public void BeginDragAndDrop(Avalonia.Point p)
        {
            _dragAndDropSelection = (p, p);
        }

        public void DragAndDrop(Avalonia.Point p)
        {
            _dragAndDropSelection = (_dragAndDropSelection!.Value.start, p);
        }

        public void EndDragAndDrop(double width, double height)
        {
            var xMin = Math.Min(_dragAndDropSelection!.Value.start.X, _dragAndDropSelection.Value.end.X);
            var yMin = Math.Min(_dragAndDropSelection.Value.start.Y, _dragAndDropSelection.Value.end.Y);
            var xMax = Math.Max(_dragAndDropSelection.Value.start.X, _dragAndDropSelection.Value.end.X);
            var yMax = Math.Max(_dragAndDropSelection.Value.start.Y, _dragAndDropSelection.Value.end.Y);
            var points = GetPointsInRectangle(
                new Vector2((float)(xMin / width), (float)(yMin / height)),
                new Vector2((float)(xMax / width), (float)(yMax / height)),
                (int)width, (int)height);
            _callback?.Invoke(points);

            _dragAndDropSelection = null;
        }

        /// <summary>
        /// List of all points to display
        /// </summary>
        private readonly List<Line> _lines = new();
        protected readonly float _offset;
        private readonly int _lineSize;

        private (Avalonia.Point start, Avalonia.Point end)? _dragAndDropSelection;

        private readonly Action<IEnumerable<Vector2>> _callback;
    }
}
