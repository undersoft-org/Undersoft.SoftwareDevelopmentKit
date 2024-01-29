namespace Undersoft.SDK.Service.Application.Components;

public partial class Collapse
{
    private static string? GetButtonClassString(CollapseItem item) => CssBuilder.Default("accordion-button")
        .AddClass("collapsed", item.IsCollapsed)
        .Build();

    private static string? GetClassString(bool collpased) => CssBuilder.Default("accordion-collapse collapse")
        .AddClass("show", !collpased)
        .Build();

    private string? ClassString => CssBuilder.Default("accordion")
        .AddClass("is-accordion", IsAccordion)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private static string? GetItemClassString(CollapseItem item) => CssBuilder.Default("accordion-item")
        .AddClass(item.Class, !string.IsNullOrEmpty(item.Class))
        .Build();

    private string GetTargetId(CollapseItem item) => ComponentIdGenerator.Generate(item);

    private string? GetTargetIdString(CollapseItem item) => $"#{GetTargetId(item)}";

    private string? ParentIdString => IsAccordion ? $"#{Id}" : null;

    protected List<CollapseItem> Children { get; } = new(10);

    [Parameter]
    public bool IsAccordion { get; set; }

    [Parameter]
    public RenderFragment? CollapseItems { get; set; }

    [Parameter]
    public Func<CollapseItem, Task>? OnCollapseChanged { get; set; }

    private async Task OnClickItem(CollapseItem item)
    {
        item.SetCollapsed(!item.IsCollapsed);
        if (OnCollapseChanged != null)
        {
            await OnCollapseChanged(item);
        }
    }

    internal void AddItem(CollapseItem item) => Children.Add(item);

    internal void RemoveItem(CollapseItem item) => Children.Remove(item);
}
