using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

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
            _data = Enumerable.Range(0, 10).Select(_ => (float)_rand.NextDouble() * 10f).ToList();
            RenderGraph();
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(3000);
                    _data.Add((float)_rand.NextDouble() * 10f);
                    Dispatcher.UIThread.Post(() =>
                    {
                        RenderGraph();
                    });
                }
            });
        }

        private void RenderGraph()
        {
            this.FindControl<Graph>("DemoGraph").Plot = new Scatter(
                x: Enumerable.Range(0, _data.Count).Select(x => (float)x).ToArray(),
                y: _data.ToArray(),
                color: Color.Black
            );
            this.FindControl<Graph>("DemoGraph2").Plot = new BoxPlot(_data);
        }

        private Random _rand = new();
        private List<float> _data;
    }
}
