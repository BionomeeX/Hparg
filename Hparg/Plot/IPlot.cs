using Hparg.Drawable;
using System.Drawing;

namespace Hparg.Plot
{
    public interface IPlot
    {
        public void BeginDragAndDrop(float x, float y);
        public void DragAndDrop(float x, float y);
        public void EndDragAndDrop();
        public Canvas GetRenderData(Canvas cvs, int drawingZone);
        public Canvas GetRenderData(int width, int height);
        public void DrawSelection(Canvas cvs);
        public void AddVerticalLine(float x, Color color, int size = 2);
        public void AddHorizontalLine(float y, Color color, int size = 2);
        public (float X, float Y) ToRelativeSpace(float x, float y);
        public float Min { get; }
        public float Max { get; }
        public float DisplayMin { set; get; }
        public float DisplayMax { set; get; }
    }
}
