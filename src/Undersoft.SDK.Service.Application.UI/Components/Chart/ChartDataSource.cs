namespace Undersoft.SDK.Service.Application.Components;

public class ChartDataSource
{
    public IEnumerable<string>? Labels { get; set; }

    public List<ChartDataset> Data { get; } = new List<ChartDataset>();

    public ChartOptions Options { get; } = new ChartOptions();

    public string? Type { get; set; }
}
