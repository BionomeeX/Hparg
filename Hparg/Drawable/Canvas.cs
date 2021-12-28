﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Hparg.Drawable
{
    public class Canvas // TODO: Dispose?
    {
        internal Canvas(int width, int height)
        {
            _width = width - 2 * _offset;
            _height = height - 2 * _offset;
            _bmp = new(width, height);
            _grf = Graphics.FromImage(_bmp);
            _grf.SmoothingMode = SmoothingMode.HighQuality;
            _grf.InterpolationMode = InterpolationMode.HighQualityBicubic;
        }

        public void DrawPoint(float x, float y, int size, Shape shape, Color color)
        {
            var brush = GetBrush(color);
            switch (shape)
            {
                case Shape.Circle:
                    _grf.FillEllipse(brush, _offset + x * _width - size / 2, _offset + y * _height - size / 2, size, size);
                    break;

                case Shape.Diamond:
                    _grf.FillRectangle(brush, _offset + x * _width - size / 2, _offset + y * _height - size / 2, size, size);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        public void DrawLine(float x1, float y1, float x2, float y2, int size, Color color)
        {
            _grf.DrawLine(new Pen(GetBrush(color), size),
                new Point(_offset + (int)(x1 * _width), _offset + (int)(y1 * _height)),
                new Point(_offset + (int)(x2 * _width), _offset + (int)(y2 * _height)));
        }

        public void DrawRectangle(float x, float y, float w, float h, int size, Color color)
        {
            _grf.DrawRectangle(new(GetBrush(color), size), new(_offset + (int)(x * _width), _offset + (int)(y * _height), (int)(w * _width), (int)(h * _height)));
        }

        /// <summary>
        /// Get the current brush given a point
        /// Allow to store brushes so we don't recreate them everytimes
        /// </summary>
        private Brush GetBrush(Color color)
        {
            if (!brushes.ContainsKey(color))
            {
                var brush = new SolidBrush(color);
                brushes.Add(color, brush);
                return brush;
            }
            return brushes[color];
        }

        internal Bitmap GetBitmap()
            => _bmp;

        public int _width;
        public int _height;
        private readonly Bitmap _bmp;
        private readonly Graphics _grf;

        private readonly Dictionary<Color, Brush> brushes = new();

        private const int _offset = 20;
    }
}
