using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Hparg.Drawable;
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

            if (_canvas == null || _canvas._maxWidth != width || _canvas._maxHeight != height)
            {
                UpdateCanvas(width, height);
            }

            var tmpCvs = new Drawable.Canvas(_canvas);
            _plot.DrawSelection(tmpCvs);
            var data = tmpCvs.ToStream();

            Bitmap bmp = new(data);
            context?.DrawImage(bmp, new Rect(0, 0, width, height));
            data.Dispose();
        }

        private void UpdateCanvas(int width, int height)
        {
            _canvas = Plot.GetRenderData(width, height);
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

        private Drawable.Canvas _canvas;
    }
}
