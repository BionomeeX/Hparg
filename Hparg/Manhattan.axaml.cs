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
    public partial class Manhattan : Window
    {
        public Manhattan()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void Render(DrawingContext context)
        {
            int width = (int)Bounds.Width;
            int height = (int)Bounds.Height;

            var format = PixelFormat.Bgra8888; // Blue freen red alpha
            using var bmp = new WriteableBitmap(new PixelSize(width, height), new Vector(96, 96), format, AlphaFormat.Unpremul);
            using var bmpLock = bmp.Lock();

            var data = Plot.GetRenderData(width, height);

            // 2D to 1D array
            var newArray = new List<int>();
            foreach (var line in data)
            {
                newArray.AddRange(line);
            }

            Marshal.Copy(newArray.ToArray(), 0, bmpLock.Address, newArray.Count);
            context?.DrawImage(bmp, new Rect(0, 0, width, height));
        }

        public Plot Plot { get; } = new();
    }
}
