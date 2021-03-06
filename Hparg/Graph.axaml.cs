using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Hparg.Plot;

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
                var pos = e.GetPosition(this);
                Plot?.BeginDragAndDrop((float)(pos.X / Bounds.Width), (float)(pos.Y / Bounds.Height));
            };
            PointerMoved += (sender, e) =>
            {
                if (_isDragAndDrop)
                {
                    var pos = e.GetPosition(this);
                    Plot?.DragAndDrop((float)(pos.X / Bounds.Width), (float)(pos.Y / Bounds.Height));
                    InvalidateVisual();
                }
            };
            PointerReleased += (sender, e) =>
            {
                if (_isDragAndDrop)
                {
                    _isDragAndDrop = false;
                    Plot?.EndDragAndDrop();
                    InvalidateVisual();
                }
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

            Bitmap bmp = new(data);
            context?.DrawImage(bmp, new Rect(0, 0, width, height));
            data.Dispose();
        }
        private IPlot _plot;
        public IPlot Plot
        {
            get => _plot;
            set
            {
                _plot = value;
                InvalidateVisual();
            }
        }
    }
}
