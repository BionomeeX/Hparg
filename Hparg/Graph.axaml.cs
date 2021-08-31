using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Hparg.Plot;
using System.Drawing.Imaging;
using System.IO;

namespace Hparg
{
    public partial class Graph : UserControl
    {
        public Graph()
        {
            InitializeComponent();

            PointerPressed += (sender, e) =>
            {
                _isDragAndDrop = true;
                Plot?.BeginDragAndDrop(e.GetPosition(this));
            };
            PointerMoved += (sender, e) =>
            {
                if (_isDragAndDrop)
                {
                    Plot?.DragAndDrop(e.GetPosition(this));
                    InvalidateVisual();
                }
            };
            PointerReleased += (sender, e) =>
            {
                _isDragAndDrop = false;
                Plot?.EndDragAndDrop(Bounds.Width, Bounds.Height);
                InvalidateVisual();
            };
        }

        private bool _isDragAndDrop;

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
            data.Dispose();
        }

        private APlot _plot;
        public APlot Plot
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
