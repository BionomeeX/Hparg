using Hparg.Drawable;
using Hparg.Plot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Hparg
{
    public class BoxPlot : APlot<float>
    {
        public BoxPlot(IEnumerable<float> data, Action<IEnumerable<float>> callback = null) : base(callback)
            => _data = data;

        private IEnumerable<float> _data;

        internal override IEnumerable<float> GetPointsInRectangle(float x, float y, float w, float h)
        {
            return Array.Empty<float>();
        }

        internal override void Render(Canvas canvas)
        {
            var min = _data.Min();
            var max = _data.Max();
            var dist = max - min;

            float ToLocal(float value)
            {
                return (value + min) / (min + max);
            }

            var ordered = _data.OrderBy(x => x);
            var median = ToLocal(ordered.ElementAt(ordered.Count() / 2));
            var firstQuartile = ToLocal(ordered.ElementAt(ordered.Count() / 4));
            var thirdQuartile = ToLocal(ordered.ElementAt(ordered.Count() / 4 * 3));

            canvas.DrawLine(.4f, 0f, .6f, 0f, 5, Color.Black);
            canvas.DrawLine(.4f, 1f, .6f, 1f, 5, Color.Black);
            canvas.DrawLine(0f, firstQuartile, 1f, firstQuartile, 5, Color.Black);
            canvas.DrawLine(0f, thirdQuartile, 1f, thirdQuartile, 5, Color.Black);
            canvas.DrawLine(0f, median, 1f, median, 5, Color.Black);
            canvas.DrawLine(0f, firstQuartile, 0f, thirdQuartile, 5, Color.Black);
            canvas.DrawLine(1f, firstQuartile, 1f, thirdQuartile, 5, Color.Black);
            canvas.DrawLine(.5f, 0f, .5f, 1f, 5, Color.Black);
        }

        internal override (float X, float Y) ToRelativeSpace(float x, float y)
        {
            throw new NotImplementedException();
        }

        internal override (float, float) GetMinMax() => (_data.Min(), _data.Max());
    }
}
