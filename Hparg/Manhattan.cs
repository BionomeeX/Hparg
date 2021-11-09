using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Hparg.Drawable;
using Hparg.Plot;


namespace Hparg
{
    public class Manhattan : APlot
    {
        public Manhattan(uint[] chpos, float[] y, IEnumerable<Color> chcolors, float offset = 50, Shape shape = Shape.Circle, int size = 2, Action<IEnumerable<Vector2>> callback = null, Plot.Point<uint>[] additionalPoints = null) :
        base(callback)
        { }

        public override void AddPoint(float x, float y, Color color, Shape shape = Shape.Circle, int size = 5)
        {
            throw new NotSupportedException("AddPoint can't be called for Manhattan plots");
        }

        internal static List<Plot.Point<float>> ComputePointsNormalization(uint[] chpos, float[] y, IEnumerable<Color> chcolors, Shape shape, int size, Plot.Point<uint>[] additionalPoints)
        {
            Dictionary<int, (int min, int max)> _chInfo = new();
            double pjumps = 0.05; // <- à modifier via les paramètres

            foreach (var pos in chpos)
            {
                int chromosome = (int)(pos % 100);
                int position = (int)(pos / 100);

                // if ch exists:
                if (!_chInfo.ContainsKey(chromosome))
                {
                    _chInfo[chromosome] = (position, position);
                }
                else
                {
                    // update min // max if necessary
                    if (position < _chInfo[chromosome].min)
                    {
                        _chInfo[chromosome] = (position, _chInfo[chromosome].max);
                    }
                    else if (position > _chInfo[chromosome].max)
                    {
                        _chInfo[chromosome] = (_chInfo[chromosome].min, position);
                    }
                }
            }

            int totalSize = _chInfo.Aggregate(0, (acc, val) => acc += val.Value.max - val.Value.min);
            Dictionary<int, double> _chPercent = new();
            foreach(var el in _chInfo)
            {
                _chPercent[el.Key] = (1d - pjumps) * (el.Value.max - el.Value.min) / totalSize;
            }


            List<Plot.Point<float>> result = new();

            // for each snp, compute x position

            for (int i = 0; i < chpos.Length; ++i)
            {
                int chromosome = (int)(chpos[i] % 100);
                int position = (int)(chpos[i] / 100);

                double rho = (double)(position - _chInfo[chromosome].min) / (_chInfo[chromosome].max - _chInfo[chromosome].min);
                double pi = rho * _chPercent[chromosome];
                foreach (var ch in _chInfo.Keys)
                {
                    if (ch < chromosome)
                    {
                        pi += pjumps / (double)(_chInfo.Count - 1d) + _chPercent[ch];
                    }
                }

                result.Add(
                    new Plot.Point<float>{
                        X = (float)pi,
                        Y = y[i],
                        Color = chcolors.ElementAt((chromosome - 1) % chcolors.Count()),
                        Shape = shape,
                        Size = size
                    }
                );
            }

            foreach (var p in additionalPoints)
            {
                int chromosome = (int)(p.Y % 100);
                int position = (int)(p.Y / 100);

                double rho = (double)(position - _chInfo[chromosome].min) / (_chInfo[chromosome].max - _chInfo[chromosome].min);
                double pi = rho * _chPercent[chromosome];
                foreach (var ch in _chInfo.Keys)
                {
                    if (ch < chromosome)
                    {
                        pi += pjumps / (double)(_chInfo.Count - 1d) + _chPercent[ch];
                    }
                }

                result.Add(
                    new Plot.Point<float>
                    {
                        X = (float)pi,
                        Y = p.X,
                        Color = p.Color,
                        Shape = p.Shape,
                        Size = p.Size
                    }
                );
            }

            return result;
        }
        internal override void Render(Canvas canvas)
        {
            // TODO
        }

        internal override (float X, float Y) ToRelativeSpace(float x, float y)
        {
            throw new NotImplementedException(); // TODO
        }
    }
}
