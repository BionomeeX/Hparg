using Hparg.Drawable;
using Hparg.Plot;
using System.Drawing;
using System.Linq;

namespace Hparg
{
    public class PlotGroup : IPlot
    {
        public PlotGroup(params IPlot[] plots)
        {
            _plots = plots;
        }

        private IPlot[] _plots;
        public Bitmap GetRenderData(int width, int height)
        {
            Canvas cvs = new(width, height, 20);
            var min = _plots.Select(x => x.Min).Min();
            var max = _plots.Select(x => x.Max).Max();
            for (int i = 0; i < _plots.Length; i++)
            {
                _plots[i].Min = min;
                _plots[i].Max = max;
                cvs.SetDrawingZone(i / (float)_plots.Length, (i + 1) / (float)_plots.Length, 0f, 1f);
                _plots[i].GetRenderData(cvs);
            }
            return cvs.GetBitmap();
        }

        public void BeginDragAndDrop(float x, float y)
        {
            throw new System.NotImplementedException();
        }

        public void DragAndDrop(float x, float y)
        {
            throw new System.NotImplementedException();
        }

        public void EndDragAndDrop()
        {
            throw new System.NotImplementedException();
        }

        public Canvas GetRenderData(Canvas cvs)
        {
            throw new System.NotImplementedException();
        }

        public void AddVerticalLine(int x, Color color, int size = 2)
        {
            throw new System.NotImplementedException();
        }

        public void AddHorizontalLine(int y, Color color, int size = 2)
        {
            throw new System.NotImplementedException();
        }

        public float Min { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public float Max { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    }
}
