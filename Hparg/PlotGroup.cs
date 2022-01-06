using Hparg.Drawable;
using Hparg.Plot;
using System.Drawing;

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
            for (int i = 0; i < _plots.Length; i++)
            {
                cvs.SetDrawingZone(i / _plots.Length, (i + 1) / _plots.Length, 0f, 1f);
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
    }
}
