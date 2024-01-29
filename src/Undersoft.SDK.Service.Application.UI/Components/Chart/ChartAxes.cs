using System.Text.Json.Serialization;

namespace Undersoft.SDK.Service.Application.Components;

public class ChartAxes
{
    public string? Title { get; set; }

    public string? Color { get; set; }

    public bool Stacked { get; set; }

    public int TicksMin { get; set; } = 0;

    public int TicksMax { get; set; } = 1;

    [JsonIgnore]
    public bool PositionLeft { get; set; } = true;

    public string Position { get => PositionLeft ? "left" : "right"; }
}
