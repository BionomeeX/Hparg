using Hparg.Drawable;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;

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
        private protected APlot(Metadata? metadata, Action<IEnumerable<T>> callback)
        {
            _metadata = metadata;
            _callback = callback;
        }

        public void AddVerticalLine(int x, System.Drawing.Color color, int size = 2)
        {
            _lines.Add(new() { Position = x, Color = new Rgba32(color.R, color.G, color.B, color.A), Size = size, Orientation = Orientation.Vertical });
        }

        public void AddHorizontalLine(int y, System.Drawing.Color color, int size = 2)
        {
            _lines.Add(new() { Position = y, Color = new Rgba32(color.R, color.G, color.B, color.A), Size = size, Orientation = Orientation.Horizontal });
        }

        internal abstract (float X, float Y) ToRelativeSpace(float x, float y);
        internal abstract IEnumerable<T> GetPointsInRectangle(float x, float y, float w, float h);
        private float _min = float.MaxValue;
        private float _max = float.MinValue;
        protected float Min
        {
            set
            {
                if (value < DisplayMin)
                {
                    DisplayMin = value;
                }
                _min = value;
            }
            get => _min;
        }
        protected float Max
        {
            set
            {
                if (value > DisplayMax)
                {
                    DisplayMax = value;
                }
                _max = value;
            }
            get => _max;
        }
        protected float DisplayMin { private set; get; }
        protected float DisplayMax { private set; get; }
        float IPlot.Min { get => Min; }
        float IPlot.Max { get => Max; }
        float IPlot.DisplayMin { set => DisplayMin = value; get => DisplayMin; }
        float IPlot.DisplayMax { set => DisplayMax = value; get => DisplayMax; }

        internal abstract void Render(Canvas canvas, Zone drawingZone);

        public Canvas GetRenderData(Canvas cvs, int drawingZone)
        {
            var zone = (Zone)drawingZone;
            Render(cvs, zone);

            foreach (var line in _lines)
            {
                if (line.Orientation == Orientation.Vertical)
                {
                    var (X, _) = ToRelativeSpace(line.Position, 0);
                    cvs.DrawLine(zone, X, 0f, X, 1f, line.Size, line.Color);
                }
                else
                {
                    var (_, Y) = ToRelativeSpace(0, line.Position);
                    cvs.DrawLine(zone, 0f, Y, 1f, Y, line.Size, line.Color);
                }
            }

            if (_dragAndDropSelection.HasValue)
            {
                var xMin = (float)Math.Min(_dragAndDropSelection.Value.start.X, _dragAndDropSelection.Value.end.X);
                var yMin = (float)Math.Min(_dragAndDropSelection.Value.start.Y, _dragAndDropSelection.Value.end.Y);
                var xMax = (float)Math.Max(_dragAndDropSelection.Value.start.X, _dragAndDropSelection.Value.end.X);
                var yMax = (float)Math.Max(_dragAndDropSelection.Value.start.Y, _dragAndDropSelection.Value.end.Y);
                cvs.DrawRectangle(Zone.Main, xMin, yMin, xMax - xMin, yMax - yMin, 1, Color.Red);
            }

            if (_metadata != null)
            {
                cvs.DrawText(
                    zone: (Zone)(drawingZone + 1),
                    x: .5f,
                    y: .5f,
                    text: _metadata.Title,
                    size: 16
                );
            }

            return cvs;
        }

        /// <summary>
        /// Get all the data to render on screen
        /// </summary>
        /// <param name="width">Width of the window</param>
        /// <param name="height">Height of the window</param>
        /// <returns>Bitmap containing the points to render</returns>
        public MemoryStream GetRenderData(int width, int height)
        {
            var cvs = new Canvas(width, height, 75, 20, 20, 20);
            cvs.DrawAxis(DisplayMin, DisplayMax);
            return GetRenderData(cvs, (int)Zone.Main).ToStream();
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

        protected Metadata? _metadata;
    }
}
