namespace Undersoft.SDK.Service.Application.Components;

public record ColumnVisibleItem
{
    public ColumnVisibleItem(string name, bool visible)
    {
        Name = name;
        Visible = visible;
    }

    public string Name { get; init; }

    public bool Visible { get; set; }
}
