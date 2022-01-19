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
        {
            _data = data;
            Min = _data.Min();
            Max = _data.Max();
        }

        private readonly IEnumerable<float> _data;

        internal override IEnumerable<float> GetPointsInRectangle(float x, float y, float w, float h)
        {
            return Array.Empty<float>();
        }

        private float GetQuarter(IOrderedEnumerable<float> _data, int parts, int index)
        {
            float count = _data.Count() / (float)parts * index;
            if (count == (int)count) // Devide give round value so everything is okay
            {
                return _data.ElementAt((int)count);
            }
            return (_data.ElementAt((int)count) + _data.ElementAt((int)count) + 1) / 2f;
        }

        internal override void Render(Canvas canvas)
        {
            var dist = Max - Min;

            float ToLocal(float value)
            {
                return (value + Min) / (Min + Max);
            }

            var ordered = _data.OrderBy(x => x);
            var median = ToLocal(GetQuarter(ordered, 2, 1));
            var firstQuartile = ToLocal(GetQuarter(ordered, 4, 1));
            var thirdQuartile = ToLocal(GetQuarter(ordered, 4, 3));
            var minQuartile = ToLocal(GetQuarter(ordered, 100, 5));
            var maxQuartile = ToLocal(GetQuarter(ordered, 100, 95));

            canvas.DrawLine(.4f, minQuartile, .6f, minQuartile, 5, Color.Black);
            canvas.DrawLine(.4f, maxQuartile, .6f, maxQuartile, 5, Color.Black);
            canvas.DrawLine(0f, firstQuartile, 1f, firstQuartile, 5, Color.Black);
            canvas.DrawLine(0f, thirdQuartile, 1f, thirdQuartile, 5, Color.Black);
            canvas.DrawLine(0f, median, 1f, median, 5, Color.Black);
            canvas.DrawLine(0f, firstQuartile, 0f, thirdQuartile, 5, Color.Black);
            canvas.DrawLine(1f, firstQuartile, 1f, thirdQuartile, 5, Color.Black);
            canvas.DrawLine(.5f, minQuartile, .5f, maxQuartile, 5, Color.Black);
        }

        internal override (float X, float Y) ToRelativeSpace(float x, float y)
        {
            throw new NotImplementedException();
        }
    }
}
