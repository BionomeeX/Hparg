using Hparg.Drawable;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Hparg.Plot
{
    public abstract class APlot<T> : IPlot
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
        private protected APlot(Action<IEnumerable<T>> callback)
        {
            _callback = callback;
        }

        public void AddVerticalLine(int x, Color color, int size = 2)
        {
            _lines.Add(new() { Position = x, Color = color, Size = size, Orientation = Orientation.Vertical });
        }

        public void AddHorizontalLine(int y, Color color, int size = 2)
        {
            _lines.Add(new() { Position = y, Color = color, Size = size, Orientation = Orientation.Horizontal });
        }

        internal abstract (float X, float Y) ToRelativeSpace(float x, float y);
        internal abstract IEnumerable<T> GetPointsInRectangle(float x, float y, float w, float h);
        protected float Min { set; get; }
        protected float Max { set; get; }
        float IPlot.Min { set => Min = value; get => Min; }
        float IPlot.Max { set => Max = value; get => Max; }

        internal abstract void Render(Canvas canvas);

        public Canvas GetRenderData(Canvas cvs)
        {
            Render(cvs);

            foreach (var line in _lines)
            {
                if (line.Orientation == Orientation.Vertical)
                {
                    var (X, _) = ToRelativeSpace(line.Position, 0);
                    cvs.DrawLine(X, 0f, X, 1f, line.Size, line.Color);
                }
                else
                {
                    var (_, Y) = ToRelativeSpace(0, line.Position);
                    cvs.DrawLine(0f, Y, 1f, Y, line.Size, line.Color);
                }
            }

            if (_dragAndDropSelection.HasValue)
            {
                var xMin = (float)Math.Min(_dragAndDropSelection.Value.start.X, _dragAndDropSelection.Value.end.X);
                var yMin = (float)Math.Min(_dragAndDropSelection.Value.start.Y, _dragAndDropSelection.Value.end.Y);
                var xMax = (float)Math.Max(_dragAndDropSelection.Value.start.X, _dragAndDropSelection.Value.end.X);
                var yMax = (float)Math.Max(_dragAndDropSelection.Value.start.Y, _dragAndDropSelection.Value.end.Y);
                cvs.DrawRectangle(xMin, yMin, xMax - xMin, yMax - yMin, 1, Color.Red);
            }

            return cvs;
        }

        /// <summary>
        /// Get all the data to render on screen
        /// </summary>
        /// <param name="width">Width of the window</param>
        /// <param name="height">Height of the window</param>
        /// <returns>Bitmap containing the points to render</returns>
        public Bitmap GetRenderData(int width, int height)
        {
            var cvs = new Canvas(width, height, 20);
            cvs.DrawAxis(Min, Max);
            return GetRenderData(cvs).GetBitmap();
        }

        public void BeginDragAndDrop(float x, float y)
        {
            _dragAndDropSelection = ((x, y), (x, y));
        }

        public void DragAndDrop(float x, float y)
        {
            _dragAndDropSelection = (_dragAndDropSelection!.Value.start, (x, y));
        }

        public void EndDragAndDrop()
        {
            var xMin = Math.Min(_dragAndDropSelection!.Value.start.X, _dragAndDropSelection.Value.end.X);
            var yMin = Math.Min(_dragAndDropSelection.Value.start.Y, _dragAndDropSelection.Value.end.Y);
            var xMax = Math.Max(_dragAndDropSelection.Value.start.X, _dragAndDropSelection.Value.end.X);
            var yMax = Math.Max(_dragAndDropSelection.Value.start.Y, _dragAndDropSelection.Value.end.Y);
            var points = GetPointsInRectangle(xMin, yMin, xMax - xMin, yMax - yMin);
            _callback?.Invoke(points);

            _dragAndDropSelection = null;
        }

        /// <summary>
        /// List of all points to display
        /// </summary>
        private readonly List<Line> _lines = new();
        protected readonly float _offset;

        private ((float X, float Y) start, (float X, float Y) end)? _dragAndDropSelection;

        private readonly Action<IEnumerable<T>> _callback;
    }
}
