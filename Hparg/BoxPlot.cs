using Hparg.Drawable;
using Hparg.Plot;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Hparg
{
    public class BoxPlot : APlot<float>
    {
        public BoxPlot(IEnumerable<float> data, Action<IEnumerable<float>> callback = null, Metadata? metadata = null) : base(metadata, callback)
        {
            _data = data;
            Min = _data.Any() ? _data.Min() : 0f;
            Max = _data.Any() ? _data.Max() : 0f;
        }

        private readonly IEnumerable<float> _data;

        internal override IEnumerable<float> GetPointsInRectangle(float x, float y, float w, float h)
        {
            return Array.Empty<float>();
        }
        private float Quantile(IOrderedEnumerable<float> _data, float q)
        {
            float count = (_data.Count() - 1) * q;
            if (count == (int)count)
            {
                return _data.ElementAt((int)count);
            }
            return (1f - q) * _data.ElementAt((int)count) + q * _data.ElementAt((int)count + 1);
        }

        internal override void Render(Canvas canvas, Zone drawingZone)
        {
            if (!_data.Any())
            {
                return;
            }
            var dist = DisplayMax - DisplayMin;

            var ordered = _data.OrderBy(x => x);
            var median = ToRelativeSpace(0f, Quantile(ordered, .5f)).Y;
            var firstQuartile = ToRelativeSpace(0f, Quantile(ordered, .25f)).Y;
            var thirdQuartile = ToRelativeSpace(0f, Quantile(ordered, .75f)).Y;
            var minQuartile = ToRelativeSpace(0f, Quantile(ordered, .05f)).Y;
            var maxQuartile = ToRelativeSpace(0f, Quantile(ordered, .95f)).Y;
            var average = ToRelativeSpace(0f, _data.Sum() / _data.Count()).Y;

            var borderLeft = .5f - .25f;
            var borderRight = .5f + .25f;
            var smallBorderLeft = .5f - .15f;
            var smallBorderRight = .5f + .15f;

            // Draw horizonal lines for box plot along with text

            // Average
            canvas.DrawLine(drawingZone, borderLeft, average, borderRight, average, lineSize, Color.Blue);

            // .05
            canvas.DrawLine(drawingZone, smallBorderLeft, minQuartile, smallBorderRight, minQuartile, lineSize, Color.Black);
            // .95
            canvas.DrawLine(drawingZone, smallBorderLeft, maxQuartile, smallBorderRight, maxQuartile, lineSize, Color.Black);

            // First quartile
            canvas.DrawLine(drawingZone, borderLeft, firstQuartile, borderRight, firstQuartile, lineSize, Color.Black);

            // Third quartile
            canvas.DrawLine(drawingZone, borderLeft, thirdQuartile, borderRight, thirdQuartile, lineSize, Color.Black);

            // Median
            canvas.DrawLine(drawingZone, borderLeft, median, borderRight, median, lineSize, Color.Black);

            // Vertical lines
            canvas.DrawLine(drawingZone, borderLeft, firstQuartile, borderLeft, thirdQuartile, lineSize, Color.Black);
            canvas.DrawLine(drawingZone, borderRight, firstQuartile, borderRight, thirdQuartile, lineSize, Color.Black);
            canvas.DrawLine(drawingZone, .5f, minQuartile, .5f, maxQuartile, lineSize, Color.Black);

            foreach (var point in _data)
            {
                canvas.DrawPoint(drawingZone, (float)_rand.NextDouble() / 10f + .5f - .05f, ToRelativeSpace(0f, point).Y, 3, Shape.Circle, new Color(new Rgba32(255, 0, 0, 150)));
            }
        }

        public override (float X, float Y) ToRelativeSpace(float x, float y)
        {
            return (float.NaN, 1f - (y - DisplayMin) / (DisplayMax - DisplayMin));
        }

        private const int lineSize = 3;
        private Random _rand = new();
    }
}
