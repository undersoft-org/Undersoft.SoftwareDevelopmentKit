namespace Undersoft.SDK.Service.Application.Components;

public partial class SubCascader
{
    [Parameter]
    [NotNull]
#if NET6_0_OR_GREATER
    [EditorRequired]
#endif
    public IEnumerable<CascaderItem>? Items { get; set; }

    [Parameter]
    public Func<CascaderItem, Task>? OnClick { get; set; }

    [Parameter]
    public string? SubMenuIcon { get; set; }

    [CascadingParameter]
    [NotNull]
    private List<CascaderItem>? SelectedItems { get; set; }

    private string? GetClassString(string classString, CascaderItem item) => CssBuilder.Default(classString)
        .AddClass("active", SelectedItems.Contains(item))
        .Build();

    protected override void OnParametersSet()
    {
        Items ??= Enumerable.Empty<CascaderItem>();
    }

    private async Task OnClickItem(CascaderItem item)
    {
        if (OnClick != null)
        {
            await OnClick(item);
        }
    }
}
