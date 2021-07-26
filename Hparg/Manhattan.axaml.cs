using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Hparg
{
    public partial class Manhattan : UserControl
    {
        public Manhattan()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void Render(DrawingContext context)
        {
            if (Plot == null)
            {
                return;
            }

            int width = (int)Bounds.Width;
            int height = (int)Bounds.Height;

            var data = Plot.GetRenderData(width, height);

            // 2D to 1D array
            var newArray = new List<int>();
            foreach (var line in data)
            {
                newArray.AddRange(line);
            }

            var format = PixelFormat.Bgra8888; // Blue green red alpha
            using var bmp = new WriteableBitmap(new PixelSize(width, height), new Vector(96, 96), format, AlphaFormat.Unpremul);
            using var bmpLock = bmp.Lock();

            Marshal.Copy(newArray.ToArray(), 0, bmpLock.Address, newArray.Count);
            context?.DrawImage(bmp, new Rect(0, 0, width, height));
        }

        private Plot _plot;
        public Plot Plot
        {
            set
            {
                _plot = value;
                InvalidateVisual(); // TODO: Need to do that when calling AddPoint too
            }
            get
            {
                return _plot;
            }
        }
    }
}
