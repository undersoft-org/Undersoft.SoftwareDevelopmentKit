using System.Text.Json.Serialization;

namespace Undersoft.SDK.Service.Application.Components;

public class ChartOptions
{
    public string? Title { get; set; }

    public ChartAxes X { get; } = new ChartAxes();

    public ChartAxes Y { get; } = new ChartAxes();

    public ChartAxes Y2 { get; } = new ChartAxes();

    public bool? Responsive { get; set; }

    public bool? MaintainAspectRatio { get; set; }

    public int? AspectRatio { get; set; }

    public int? ResizeDelay { get; set; }

    public string? Height { get; set; }

    public string? Width { get; set; }

    public double BorderWidth { get; set; } = 3;

    public bool ShowLegend { get; set; } = true;

    [JsonConverter(typeof(ChartLegendPositionConverter))]
    public ChartLegendPosition LegendPosition { get; set; } = ChartLegendPosition.Top;

    public bool? ShowXLine { get; set; }

    public bool? ShowYLine { get; set; }

    public bool? ShowXScales { get; set; }

    public bool? ShowYScales { get; set; }

    public Dictionary<string, string> Colors { get; set; } = new Dictionary<string, string>()
    {
        { "blue", "rgb(54, 162, 235)" },
        { "green", "rgb(75, 192, 192)" },
        { "red", "rgb(255, 99, 132)" },
        { "orange", "rgb(255, 159, 64)" },
        { "yellow", "rgb(255, 205, 86)" },
        { "tomato", "rgb(255, 99, 71)" },
        { "pink", "rgb(255, 192, 203)" },
        { "violet", "rgb(238, 130, 238)" }
    };
}
