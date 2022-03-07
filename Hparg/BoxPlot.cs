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

            float ToLocal(float value)
            {
                return (value - DisplayMin) / (DisplayMax - DisplayMin);
            }

            var ordered = _data.OrderBy(x => x);
            var median = ToLocal(Quantile(ordered, .5f));
            var firstQuartile = ToLocal(Quantile(ordered, .25f));
            var thirdQuartile = ToLocal(Quantile(ordered, .75f));
            var minQuartile = ToLocal(Quantile(ordered, .05f));
            var maxQuartile = ToLocal(Quantile(ordered, .95f));

            var textSize = 11;
            var textOffset = .02f;

            var borderLeft = .5f - .25f;
            var borderRight = .5f + .25f;
            var smallBorderLeft = .5f - .15f;
            var smallBorderRight = .5f + .15f;

            // Draw horizonal lines for box plot along with text

            // .05
            canvas.DrawLine(drawingZone, smallBorderLeft, 1f - minQuartile, smallBorderRight, 1f - minQuartile, lineSize, Color.Black);
            // .95
            canvas.DrawLine(drawingZone, smallBorderLeft, 1f - maxQuartile, smallBorderRight, 1f - maxQuartile, lineSize, Color.Black);

            // First quartile
            canvas.DrawLine(drawingZone, borderLeft, 1f - firstQuartile, borderRight, 1f - firstQuartile, lineSize, Color.Black);

            // Third quartile
            canvas.DrawLine(drawingZone, borderLeft, 1f - thirdQuartile, borderRight, 1f - thirdQuartile, lineSize, Color.Black);

            // Median
            canvas.DrawLine(drawingZone, borderLeft, 1f - median, borderRight, 1f - median, lineSize, Color.Black);

            // Vertical lines
            canvas.DrawLine(drawingZone, borderLeft, 1f - firstQuartile, borderLeft, 1f - thirdQuartile, lineSize, Color.Black);
            canvas.DrawLine(drawingZone, borderRight, 1f - firstQuartile, borderRight, 1f - thirdQuartile, lineSize, Color.Black);
            canvas.DrawLine(drawingZone, .5f, 1f - minQuartile, .5f, 1f - maxQuartile, lineSize, Color.Black);

            foreach (var point in _data)
            {
                canvas.DrawPoint(drawingZone, (float)_rand.NextDouble() / 10f + .5f - .05f, 1f - ToLocal(point), 3, Shape.Circle, new Color(new Rgba32(255, 0, 0, 150)));
            }
        }

        internal override (float X, float Y) ToRelativeSpace(float x, float y)
        {
            throw new NotImplementedException();
        }

        private const int lineSize = 3;
        private Random _rand = new();
    }
}
