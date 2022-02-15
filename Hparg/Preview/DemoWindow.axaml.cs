using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
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
                color: System.Drawing.Color.Black
            );
            List<float> odds = new(), evens = new();
            for (int i = 0; i < _data.Count; i++)
            {
                if (i % 2 == 0)
                {
                    evens.Add(_data[i]);
                }
                else
                {
                    odds.Add(_data[i]);
                }
            }
            this.FindControl<Graph>("DemoGraph2").Plot = new PlotGroup(
                new[]
                {
                    new BoxPlot(evens),
                    new BoxPlot(odds)
                }
            );
        }

        private Random _rand = new();
        private List<float> _data;
    }
}
