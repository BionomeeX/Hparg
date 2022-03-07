using Hparg.Drawable;
using Hparg.Plot;
using SixLabors.ImageSharp.PixelFormats;

namespace Hparg
{
    public class PlotGroup : IPlot
    {
        public PlotGroup(IPlot[] plots, string? title = null)
        {
            _plots = plots;
            _title = title;
        }

        private IPlot[] _plots;
        public MemoryStream GetRenderData(int width, int height)
        {
            var min = _plots.Select(x => x.Min).Min();
            var max = _plots.Select(x => x.Max).Max();
            var diff = (max - min);
            min -= diff * .1f;
            max += diff * .1f;
            var cvs = new Canvas(width, height, 75, 20, 20, 50, _plots.Length);
            for (int i = 0; i < _plots.Length; i++)
            {
                _plots[i].DisplayMin = min;
                _plots[i].DisplayMax = max;
                _plots[i].GetRenderData(cvs, (i * 3) + 1);
            }

            for (int i = 0; i < _plots.Length; i++)
            {
                foreach (var line in _lines)
                {
                    var y = ToLocal(line.Position);
                    cvs.DrawLine((Zone)((i * 3) + 1), 0f, y, 1f, y, line.Size, line.Color);
                }
            }

            cvs.DrawAxis(DisplayMin, DisplayMax);

            if (_title != null)
            {
                cvs.DrawText(Zone.UpperMarginFull, .5f, .5f, _title, 20);
            }

            return cvs.ToStream();
        }

        public void BeginDragAndDrop(float x, float y)
        { }

        public void DragAndDrop(float x, float y)
        { }

        public void EndDragAndDrop()
        { }

        public Canvas GetRenderData(Canvas cvs, int drawingZone)
        {
            throw new NotImplementedException();
        }

        public void AddVerticalLine(float x, System.Drawing.Color color, int size = 2)
        {
            throw new NotImplementedException();
        }

        public void AddHorizontalLine(float y, System.Drawing.Color color, int size = 2)
        {
            _lines.Add(new() { Position = y, Color = new Rgba32(color.R, color.G, color.B, color.A), Size = size, Orientation = Orientation.Horizontal });
        }

        public float ToLocal(float value)
        {
            return (value - DisplayMin) / (DisplayMax - DisplayMin);
        }

        private readonly List<Line> _lines = new();
        public float Min { get => _plots.Select(x => x.Min).Min(); }
        public float Max { get => _plots.Select(x => x.Max).Max(); }
        public float DisplayMin { get => _plots.Select(x => x.DisplayMin).Min(); set => throw new NotImplementedException(); }
        public float DisplayMax { get => _plots.Select(x => x.DisplayMax).Max(); set => throw new NotImplementedException(); }

        private string? _title;
    }
}
