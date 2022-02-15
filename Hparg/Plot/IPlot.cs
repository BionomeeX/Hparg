using Hparg.Drawable;
using SixLabors.ImageSharp;

namespace Hparg.Plot
{
    public interface IPlot
    {
        public void BeginDragAndDrop(float x, float y);
        public void DragAndDrop(float x, float y);
        public void EndDragAndDrop();
        public Canvas GetRenderData(Canvas cvs);
        public Image GetRenderData(int width, int height);
        public void AddVerticalLine(int x, Color color, int size = 2);
        public void AddHorizontalLine(int y, Color color, int size = 2);
        public float Min { set; get; }
        public float Max { set; get; }
    }
}
