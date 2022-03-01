using Hparg.Drawable;
using Hparg.Plot;
using System.IO;
using System.Linq;

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
            throw new System.NotImplementedException();
        }

        public void AddVerticalLine(int x, System.Drawing.Color color, int size = 2)
        {
            throw new System.NotImplementedException();
        }

        public void AddHorizontalLine(int y, System.Drawing.Color color, int size = 2)
        {
            throw new System.NotImplementedException();
        }

        public float Min { get => _plots.Select(x => x.Min).Min(); }
        public float Max { get => _plots.Select(x => x.Max).Max(); }
        public float DisplayMin { get => _plots.Select(x => x.DisplayMin).Min(); set => throw new System.NotImplementedException(); }
        public float DisplayMax { get => _plots.Select(x => x.DisplayMax).Max(); set => throw new System.NotImplementedException(); }

        private string? _title;
    }
}
