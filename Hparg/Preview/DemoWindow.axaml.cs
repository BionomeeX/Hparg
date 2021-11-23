using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Drawing;
using System.Linq;

namespace Hparg
{
    public partial class DemoWindow : Window
    {
        public DemoWindow()
        {
            AvaloniaXamlLoader.Load(this);
#if DEBUG
            this.AttachDevTools();
#endif
            Random _rand = new();
            this.FindControl<Graph>("DemoGraph").Plot = new Scatter(
                x: Enumerable.Range(0, 10).Select(x => (float)x).ToArray(),
                y: Enumerable.Range(0, 10).Select(_ => (float)_rand.NextDouble() * 10f).ToArray(),
                color: Color.Black
            );
        }
    }
}
