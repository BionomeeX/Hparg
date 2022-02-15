using Hparg.Drawable;
using Hparg.Plot;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hparg
{
    public class BoxPlot : APlot<float>
    {
        public BoxPlot(IEnumerable<float> data, Action<IEnumerable<float>> callback = null) : base(callback)
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

        internal override void Render(Canvas canvas)
        {
            if (!_data.Any())
            {
                return;
            }
            var dist = Max - Min;

            float ToLocal(float value)
            {
                return (value - Min) / (Max - Min);
            }

            var ordered = _data.OrderBy(x => x);
            var median = ToLocal(Quantile(ordered, .5f));
            var firstQuartile = ToLocal(Quantile(ordered, .25f));
            var thirdQuartile = ToLocal(Quantile(ordered, .75f));
            var minQuartile = ToLocal(Quantile(ordered, .05f));
            var maxQuartile = ToLocal(Quantile(ordered, .95f));

            canvas.DrawLine(.4f, 1f - minQuartile, .6f, 1f - minQuartile, 5, Color.Black);
            canvas.DrawLine(.4f, 1f - maxQuartile, .6f, 1f - maxQuartile, 5, Color.Black);
            canvas.DrawLine(.1f, 1f - firstQuartile, .9f, 1f - firstQuartile, 5, Color.Black);
            canvas.DrawLine(.1f, 1f - thirdQuartile, .9f, 1f - thirdQuartile, 5, Color.Black);
            canvas.DrawLine(.1f, 1f - median, .9f, 1f - median, 5, Color.Black);
            canvas.DrawLine(.1f, 1f - firstQuartile, .1f, 1f - thirdQuartile, 5, Color.Black);
            canvas.DrawLine(.9f, 1f - firstQuartile, .9f, 1f - thirdQuartile, 5, Color.Black);
            canvas.DrawLine(.5f, 1f - minQuartile, .5f, 1f - maxQuartile, 5, Color.Black);

            foreach (var point in _data)
            {
                canvas.DrawPoint((float)_rand.NextDouble() / 10f + .5f - .05f, 1f - ToLocal(point), 3, Shape.Circle, new Color(new Rgba32(150, 255, 0, 0)));
            }
        }

        internal override (float X, float Y) ToRelativeSpace(float x, float y)
        {
            throw new NotImplementedException();
        }

        private Random _rand = new();
    }
}
