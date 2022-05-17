using Hparg.Drawable;
using Hparg.Plot;
using SixLabors.ImageSharp;
using System.Numerics;

namespace Hparg
{
    public class StackedColumnChart : APlot<Vector2>
    {
        public StackedColumnChart(float[][] data, Metadata? metadata = null, Action<IEnumerable<Vector2>> callback = null) : base(metadata, callback)
        {
            _data = data;
            Min = 0f;
            Max = 1f;
        }

        public override (float X, float Y) ToRelativeSpace(float x, float y)
        {
            throw new NotImplementedException();
        }

        internal override IEnumerable<Vector2> GetPointsInRectangle(float x, float y, float w, float h)
        {
            return Array.Empty<Vector2>();
        }

        internal override void Render(Canvas canvas, Zone drawingZone)
        {
            for (int x = 0; x < _data.Length; x++)
            {
                var sum = _data[x].Sum();
                var last = 0f;
                for (int y = 0; y < _data[x].Length; y++)
                {
                    var curr = _data[x][y] / sum;
                    canvas.DrawRectangle(drawingZone,
                        x: (float)x / _data.Length + .1f,
                        y: last,
                        w: 1f / _data.Length - .1f,
                        h: curr,
                        2,
                        _colors[y % _colors.Length],
                        doesFill: true
                    );
                    last += curr;
                }
                ;
            }
        }

        private Color[] _colors = new[]
        {
            Color.Red, Color.Blue, Color.Green, Color.Magenta, Color.Cyan, Color.Yellow
        };
        private float[][] _data;
    }
}
