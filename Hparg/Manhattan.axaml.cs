using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
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

            using MemoryStream stream = new();
            data.Save(stream, ImageFormat.Png);
            stream.Position = 0;
            Bitmap bmp = new(stream);
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
