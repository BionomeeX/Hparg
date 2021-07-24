using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

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

        public Plot Plot { get; } = new();
    }
}
