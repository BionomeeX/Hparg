using System;
using System.Drawing;

namespace Hparg
{
    public class Plot
    {
        public void AddPoint(float x, float y, Color color, Shape shape = Shape.Circle, int size = 5)
        { }

        public void AddPoints(float[] x, float[] y, Color color, Shape shape = Shape.Circle, int size = 5)
        { }

        public void Clear()
        { }

        internal int[][] GetRenderData(int width, int height)
        {
            return Array.Empty<int[]>();
        }
    }
}
