
/**

class SNP {
    int ChPos;
    float Y;
    Color Color;
    Shape Shape;
    int Size;
}

private Dictionnary<int, (int min, int max)> _chInfo;

public Manhattan(int[] chPos, float[] y, float offset = 50, Shape shape = Shape.Circle, int size = 2, int lineSize = 2)
{
    if (chPos.Length != y.Length)
    {
        throw new ArgumentException("chPos must be of the same length of y", nameof(x));
    }

    // Add all the SNP
    _SNPs.AddRange(Enumerable.Range(0, x.Length).Select(i =>
    {
        return new SNP
        {
            ChPos = chPos[i],
            Y = y[i],
            Color = color,
            Shape = shape,
            Size = size
        };
    }));

    // Calculate the bounds if dynamic, else use the ones given in parameter
    _yMin = new(yMin ?? _SNPs.Min(p => p.Y), yMin == null);
    _yMax = new(yMax ?? _SNPs.Max(p => p.Y), yMax == null);

    // Compute Min and Max per Chromosome:
    _chInfo = new Dictionnary<int, (int min, int max)>();
    foreach(var cp in chPos) {
        UpdateChInfo(cp);
    }

    _lineSize = lineSize;
    _offset = offset;
}


internal void UpdateChInfo(int chPos) {
    int chromosome = chPos % 100;
    int position = int(chPos / 100)

    // if ch exists:
    if ( ! _chInfo.ContainsKey(chromosome) ) {
        _chInfo[chromosome] = (position, position)
    } else {
        // update min // max if necessary
        if ( position < _chInfo[chromosome].min ) {
            _chInfo[chromosome].min = position
        } else if ( position > _chInfo[chromosome].max ) {
            _chInfo[chromosome].max = position
        }
    }
}


public void AddSNP(int chPos, float y, Color color, Shape shape = Shape.Circle, int size = 5)
{
    // Add the point to the graph
    _SNPs.Add(new SNP() { ChPos = chPos, Y = y, Color = color, Shape = shape, Size = size });

    UpdateChInfo(chPos);

    // Recalculate all bounds if set they are set to dynamic
    if (_yMin.IsDynamic)
    {
        _yMin.Value = Math.Min(_yMin.Value, y);
    }
    if (_yMax.IsDynamic)
    {
        _yMax.Value = Math.Max(_yMax.Value, y);
    }
}


internal Bitmap GetRenderData(int width, int height)
{
    var bmp = new Bitmap(width, height);

    if (_SNPs.Count == 0)
    {
        return bmp;
    }

    using Graphics grf = Graphics.FromImage(bmp);
    grf.SmoothingMode = SmoothingMode.HighQuality;
    grf.InterpolationMode = InterpolationMode.HighQualityBicubic;
    (int x, int y)? lastPos = null;

    foreach (var SNP in _SNPs)
    {
        var brush = GetBrush(point);
        var pos = CalculateSNPCoordinate(SNP, width, height);
        switch (point.Shape)
        {
            case Shape.Circle:
                grf.FillEllipse(brush, pos.x - point.Size / 2, pos.y - point.Size / 2, point.Size, point.Size);
                break;

            case Shape.Diamond:
                grf.FillRectangle(brush, pos.x - point.Size / 2, pos.y - point.Size / 2, point.Size, point.Size);
                break;

            default:
                throw new NotImplementedException();
        }

        if (_lineSize > 0 && lastPos.HasValue)
        {
            grf.DrawLine(new Pen(brush, _lineSize),
                new System.Drawing.Point(pos.x, pos.y),
                new System.Drawing.Point(lastPos.Value.x, lastPos.Value.y));
        }
        lastPos = pos;
    }

    foreach (var line in _lines)
    {
        Point point = line.Orientation == Orientation.Horizontal
            ? new() { Color = line.Color, X = 0, Y = line.Position }
            : new() { Color = line.Color, Y = 0, X = line.Position };
        Point otherPoint = line.Orientation == Orientation.Horizontal
            ? new() { Color = line.Color, X = width, Y = line.Position }
            : new() { Color = line.Color, Y = height, X = line.Position };

        var brush = GetBrush(point);
        var pos = CalculateCoordinate(point, width, height);
        var otherPos = CalculateCoordinate(otherPoint, width, height);

        grf.DrawLine(new Pen(brush, line.Size), new System.Drawing.Point(pos.x, pos.y), new System.Drawing.Point(otherPos.x, otherPos.y));
    }

    return bmp;
}


private (int x, int y) CalculateSNPCoordinate(SNP SNP, int width, int height)
{
    int chromosome = SNP.ChPos % 100;
    int position = int(SNP.ChPos / 100);

    double pjumps = 0.2; // <- à modifier via les paramètres

    double rho = (position - _chInfo[chromosome].min) / (_chInfo[chromosome].max - _chInfo[chromosome].min);
    int totalSize = _chInfo.Aggregate(0, (acc, val) => acc += val.max - val.min);
    double pi = rho * double(_chInfo[chromosome].max - _chInfo[chromosome].min) / double(totalSize);

    foreach (var ch in _chInfo.Keys()) {
        if (ch < chromosome) {
            pi += pjumps / double(_chInfo.Count() - 1d) + double(_chInfo[ch].max - _chInfo[ch].min) / double(totalSize);
        }
    }

    int x = (int)((width - 2 * _offset - 1) * pi + _offset);
    int y = (int)((height - 2 * _offset - 1) * (1f - (SNP.Y - _yMin.Value) / (_yMax.Value - _yMin.Value)) + _offset);
    return (x, y);
}

**/
