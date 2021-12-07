using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Hparg.Drawable;
using Hparg.Plot;


namespace Hparg
{
    public class Manhattan : APlot<uint>
    {

        private List<Plot.Point<float, float>> _points;
        private uint[] _chpos;

        public Manhattan(uint[] chpos, float[] y, IEnumerable<Color> chcolors, float offset = 50, Shape shape = Shape.Circle, int size = 2, Action<IEnumerable<uint>> callback = null, Point<uint, float>[] additionalPoints = null) :
        base(callback)
        {
            if (additionalPoints == null)
            {
                additionalPoints = Array.Empty<Point<uint, float>>();
            }
            _points = ComputePointsNormalization(chpos, y, chcolors, shape, size, additionalPoints);
            _chpos = chpos;
        }

        public override void AddPoint(float x, float y, Color color, Shape shape = Shape.Circle, int size = 5)
        {
            throw new NotSupportedException("AddPoint can't be called for Manhattan plots");
        }

        internal static List<Plot.Point<float, float>> ComputePointsNormalization(uint[] chpos, float[] y, IEnumerable<Color> chcolors, Shape shape, int size, Plot.Point<uint, float>[] additionalPoints)
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

            float ymin = y.Min();
            float ymax = y.Max();

            int totalSize = _chInfo.Aggregate(0, (acc, val) => acc += val.Value.max - val.Value.min);
            Dictionary<int, double> _chPercent = new();
            foreach (var el in _chInfo)
            {
                _chPercent[el.Key] = (1d - pjumps) * (el.Value.max - el.Value.min) / totalSize;
            }


            List<Plot.Point<float, float>> result = new();

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
                    new Plot.Point<float, float>
                    {
                        X = (float)pi,
                        Y = 1f - (y[i] - ymin) / (ymax - ymin),
                        Color = chcolors.ElementAt((chromosome - 1) % chcolors.Count()),
                        Shape = shape,
                        Size = size
                    }
                );
            }

            foreach (var p in additionalPoints)
            {
                int chromosome = (int)(p.X % 100);
                int position = (int)(p.X / 100);

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
                    new Plot.Point<float, float>
                    {
                        X = (float)pi,
                        Y = 1f - (p.Y - ymin) / (ymax - ymin),
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
            for (int i = 0; i < _points.Count; i++)
            {
                var point = _points[i];

                canvas.DrawPoint(point.X, point.Y, point.Size, point.Shape, point.Color);
            }
        }

        internal override (float X, float Y) ToRelativeSpace(float x, float y)
        {
            throw new NotImplementedException(); // TODO
        }

        internal override IEnumerable<uint> GetPointsInRectangle(float x, float y, float w, float h)
        {
            return _chpos.Where((_, i) => _points[i].X >= x && _points[i].X <= x + w && _points[i].Y >= y && _points[i].Y <= y + h);
        }
    }
}
