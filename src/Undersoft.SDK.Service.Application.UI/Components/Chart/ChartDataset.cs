using System.Text.Json.Serialization;

namespace Undersoft.SDK.Service.Application.Components;

public class ChartDataset
{
    public string? Label { get; set; }

    public IEnumerable<object>? Data { get; set; }

    public bool Fill { get; set; }

    public float Tension { get; set; } = 0.4f;

    [JsonIgnore]
    public bool IsAxisY2 { get; set; }

    public string? YAxisID { get => IsAxisY2 ? "y2" : "y"; }
}
