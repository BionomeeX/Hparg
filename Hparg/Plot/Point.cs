using System.Drawing;

namespace Hparg.Plot
{
    public class Point<Tx, Ty>
    {
        public Tx X { init; get; }
        public Ty Y { init; get; }
        public Color Color { init; get; }
        public Shape Shape { init; get; }
        public int Size { init; get; }

        public override string ToString()
        {
            return $"Point ({X};{Y})";
        }
    }
}
