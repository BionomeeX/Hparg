using Hparg.Drawable;
using Hparg.Plot;
using System.IO;
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
        public MemoryStream GetRenderData(int width, int height)
        {
            var min = _plots.Select(x => x.Min).Min();
            var max = _plots.Select(x => x.Max).Max();
            var cvs = new Canvas(width, height, 20, _plots.Length);
            for (int i = 0; i < _plots.Length; i++)
            {
                _plots[i].Min = min;
                _plots[i].Max = max;
                _plots[i].GetRenderData(cvs, i);
            }

            cvs.DrawAxis(Min, Max);

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

        public float Min { get => _plots.Select(x => x.Min).Min(); set => throw new System.NotImplementedException(); }
        public float Max { get => _plots.Select(x => x.Max).Max(); set => throw new System.NotImplementedException(); }
    }
}
