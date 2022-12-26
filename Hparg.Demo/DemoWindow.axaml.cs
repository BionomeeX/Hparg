using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace Hparg.Demo
{
    public partial class DemoWindow : Window
    {
        private const float max = 3f;
        public DemoWindow()
        {
            AvaloniaXamlLoader.Load(this);
#if DEBUG
            this.AttachDevTools();
#endif
            _data = Enumerable.Range(0, 20).Select(_ => (float)_rand.NextDouble() * 10f).ToList();
            RenderGraph();

            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(3000);
                    _data.Add((float)_rand.NextDouble() * 2f);
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
            var pg = new PlotGroup(
                new[]
                {
                    new BoxPlot(evens, metadata: new Metadata() { Title = "Evens" }),
                    new BoxPlot(odds, metadata: new Metadata() { Title = "Odds" })
                },
                title: "Values"
            );
            pg.AddHorizontalLine(_data.Sum() / _data.Count, System.Drawing.Color.Blue);
            this.FindControl<Graph>("DemoGraph2").Plot = pg;
            this.FindControl<Graph>("DemoGraph3").Plot = new PlotGroup(new Plot.IPlot[]
            {
                new StackedColumnChart(evens.Select(x => (int)MathF.Round(x)).ToArray()),
                new StackedColumnChart(odds.Select(x => (int)MathF.Round(x)).ToArray())
            });
        }

        private Random _rand = new();
        private List<float> _data;
    }
}
