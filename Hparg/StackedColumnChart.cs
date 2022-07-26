using Hparg.Drawable;
using Hparg.Plot;
using SixLabors.ImageSharp;
using System.Numerics;

namespace Hparg
{
    public class StackedColumnChart : APlot<Vector2>
    {
        public StackedColumnChart(int[] data, Metadata? metadata = null, Action<IEnumerable<Vector2>> callback = null) : base(metadata, callback)
        {
            _data = data;
            Min = 0f;
            Max = data.Length;
        }

        public override (float X, float Y) ToRelativeSpace(float x, float y)
        {
            return (float.NaN, (y - DisplayMin) / (DisplayMax - DisplayMin));
        }

        internal override IEnumerable<Vector2> GetPointsInRectangle(float x, float y, float w, float h)
        {
            return Array.Empty<Vector2>();
        }

        internal override void Render(Canvas canvas, Zone drawingZone)
        {
            var categories = _data.Distinct().OrderBy(x => x);

            var curr = DisplayMax * 0.8f;
            int index = 0;

            foreach (var c in categories)
            {
                var height = _data.Count(x => x == c) * 0.9f;
                canvas.DrawRectangle(drawingZone,
                        x: .2f,
                        y: ToRelativeSpace(0f, curr - height).Y,
                        w: .8f,
                        h: ToRelativeSpace(0f, height).Y,
                        2,
                        _colors[index % _colors.Length],
                        doesFill: true
                    );
                curr -= height;
                index++;
            }
        }

        private Color[] _colors = new[]
        {
            Color.Red, Color.Blue, Color.Green, Color.Magenta, Color.Cyan, Color.Yellow
        };
        private int[] _data;
    }
}
