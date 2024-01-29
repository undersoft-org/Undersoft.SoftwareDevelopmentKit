namespace Undersoft.SDK.Service.Application.Components;

public sealed partial class DropdownWidget
{
    private string? ClassString => CssBuilder.Default("widget")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public IEnumerable<DropdownWidgetItem>? Items { get; set; }

    private List<DropdownWidgetItem> Childs { get; } = new List<DropdownWidgetItem>(20);

    internal void Add(DropdownWidgetItem item)
    {
        Childs.Add(item);
    }

    private IEnumerable<DropdownWidgetItem> GetItems() => Items == null ? Childs : Childs.Concat(Items);
}
