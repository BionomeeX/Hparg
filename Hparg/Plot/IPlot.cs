using Hparg.Drawable;
using System.Drawing;
using System.IO;

namespace Hparg.Plot
{
    public interface IPlot
    {
        public void BeginDragAndDrop(float x, float y);
        public void DragAndDrop(float x, float y);
        public void EndDragAndDrop();
        public Canvas GetRenderData(Canvas cvs, int drawingZone);
        public MemoryStream GetRenderData(int width, int height);
        public void AddVerticalLine(int x, Color color, int size = 2);
        public void AddHorizontalLine(int y, Color color, int size = 2);
        public float Min { get; }
        public float Max { get; }
        public float DisplayMin { set; get; }
        public float DisplayMax { set; get; }
    }
}
