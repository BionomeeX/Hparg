using System.Drawing;

namespace Hparg.Plot
{
    public interface IPlot
    {
        public void BeginDragAndDrop(float x, float y);
        public void DragAndDrop(float x, float y);
        public void EndDragAndDrop();
        public Bitmap GetRenderData(int width, int height);
        public abstract void AddPoint(float x, float y, Color color, Shape shape = Shape.Circle, int size = 5);
        public void AddVerticalLine(int x, Color color, int size = 2);
        public void AddHorizontalLine(int y, Color color, int size = 2);
    }
}
