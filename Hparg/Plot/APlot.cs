using Hparg.Drawable;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

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

        public void AddVerticalLine(float x, System.Drawing.Color color, int size = 2)
        {
            _lines.Add(new() { Position = x, Color = new Rgba32(color.R, color.G, color.B, color.A), Size = size, Orientation = Orientation.Vertical });
        }

        public void AddHorizontalLine(float y, System.Drawing.Color color, int size = 2)
        {
            _lines.Add(new() { Position = y, Color = new Rgba32(color.R, color.G, color.B, color.A), Size = size, Orientation = Orientation.Horizontal });
        }

        public abstract (float X, float Y) ToRelativeSpace(float x, float y);
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
            _lastCanvas = cvs;

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

        public void DrawSelection(Canvas cvs)
        {
            if (_dragAndDropSelection.HasValue)
            {
                (float XMin, float YMin, float XMax, float YMax) = ToLocalRect(cvs);
                cvs.DrawRectangle(Zone.Main, XMin, YMin, XMax - XMin, YMax - YMin, 1, Color.Red, false);
            }
        }

        /// <summary>
        /// Get all the data to render on screen
        /// </summary>
        /// <param name="width">Width of the window</param>
        /// <param name="height">Height of the window</param>
        /// <returns>Bitmap containing the points to render</returns>
        public Canvas GetRenderData(int width, int height)
        {
            var cvs = new Canvas(width, height, 75, 20, 20, 20);
            cvs.DrawAxis(DisplayMin, DisplayMax);
            return GetRenderData(cvs, (int)Zone.Main);
        }

        private (float XMin, float YMin, float XMax, float YMax) ToLocalRect(Canvas cvs)
        {
            var width = cvs.GetWidth(Zone.Main);
            var leftWidth = cvs.GetWidth(Zone.LeftMargin);
            var height = cvs.GetHeight(Zone.Main);
            var topHeight = cvs.GetHeight(Zone.UpperMargin);
            var xMin = (float)Math.Min(_dragAndDropSelection.Value.start.X, _dragAndDropSelection.Value.end.X);
            var yMin = (float)Math.Min(_dragAndDropSelection.Value.start.Y, _dragAndDropSelection.Value.end.Y);
            var xMax = (float)Math.Max(_dragAndDropSelection.Value.start.X, _dragAndDropSelection.Value.end.X);
            var yMax = (float)Math.Max(_dragAndDropSelection.Value.start.Y, _dragAndDropSelection.Value.end.Y);

            xMin = (xMin - leftWidth) / width;
            xMax = (xMax - leftWidth) / width;
            yMin = (yMin - topHeight) / height;
            yMax = (yMax - topHeight) / height;

            return (xMin, yMin, xMax, yMax);
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
            (float XMin, float YMin, float XMax, float YMax) = ToLocalRect(_lastCanvas);
            var points = GetPointsInRectangle(XMin, YMin, XMax - XMin, YMax - YMin);
            _callback?.Invoke(points);

            _dragAndDropSelection = null;
        }

        private Canvas _lastCanvas;

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
