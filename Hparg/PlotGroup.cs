using Hparg.Drawable;
using Hparg.Plot;
using System.Drawing;

namespace Hparg
{
    public class PlotGroup
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
    }
}
