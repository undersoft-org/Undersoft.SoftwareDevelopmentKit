namespace Undersoft.SDK.Service.Application.Components;

public partial class RibbonTab
{
    [Parameter]
    public bool ShowFloatButton { get; set; }

    [Parameter]
    public Func<bool, Task>? OnFloatChanged { get; set; }

    [Parameter]
    public string? RibbonArrowUpIcon { get; set; }

    [Parameter]
    public string? RibbonArrowDownIcon { get; set; }

    [Parameter]
    public string? RibbonArrowPinIcon { get; set; }

    private bool IsFloat { get; set; }

    private string? ArrowIconClassString => CssBuilder.Default()
        .AddClass(RibbonArrowUpIcon, !IsFloat)
        .AddClass(RibbonArrowDownIcon, IsFloat && !IsExpand)
        .AddClass(RibbonArrowPinIcon, IsFloat && IsExpand)
        .Build();

    [Parameter]
    [NotNull]
#if NET6_0_OR_GREATER
    [EditorRequired]
#endif
    public IEnumerable<RibbonTabItem>? Items { get; set; }

    [Parameter]
    public Func<RibbonTabItem, Task>? OnItemClickAsync { get; set; }

    [Parameter]
    public Func<RibbonTabItem, Task>? OnMenuClickAsync { get; set; }

    [Parameter]
    public RenderFragment? RightButtonsTemplate { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public bool IsBorder { get; set; } = true;

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    private bool IsExpand { get; set; }

    private string? HeaderClassString => CssBuilder.Default("ribbon-tab")
        .AddClass("is-float", IsFloat)
        .AddClass("is-expand", IsFloat && IsExpand)
        .AddClass("border", IsBorder)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private static string? GetClassString(RibbonTabItem item) => CssBuilder.Default()
        .AddClass("active", item.IsActive)
        .Build();

    protected override Task InvokeInitAsync() => InvokeVoidAsync("init", Id, Interop, nameof(SetExpand));

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        RibbonArrowUpIcon ??= IconTheme.GetIconByKey(ComponentIcons.RibbonTabArrowUpIcon);
        RibbonArrowDownIcon ??= IconTheme.GetIconByKey(ComponentIcons.RibbonTabArrowDownIcon);
        RibbonArrowPinIcon ??= IconTheme.GetIconByKey(ComponentIcons.RibbonTabArrowPinIcon);

        Items ??= Enumerable.Empty<RibbonTabItem>();
        if (!Items.Any(i => i.IsActive))
        {
            var item = Items.FirstOrDefault();
            if (item != null)
            {
                item.IsActive = true;
            }
        }
    }

    [JSInvokable]
    public void SetExpand()
    {
        IsExpand = false;
        StateHasChanged();
    }

    private async Task OnClick(RibbonTabItem item)
    {
        if (OnItemClickAsync != null)
        {
            await OnItemClickAsync(item);
        }
    }

    private async Task OnClickTabItemAsync(TabItem item)
    {
        var tab = Items.FirstOrDefault(i => i.IsActive);
        if (tab != null)
        {
            tab.IsActive = false;
        }
        tab = Items.First(i => i.Text == item.Text);
        tab.IsActive = true;
        if (OnMenuClickAsync != null)
        {
            await OnMenuClickAsync(tab);
        }
        if (IsFloat)
        {
            IsExpand = true;
            StateHasChanged();
        }
    }

    private async Task OnToggleFloat()
    {
        IsFloat = !IsFloat;
        if (!IsFloat)
        {
            IsExpand = false;
        }
        if (OnFloatChanged != null)
        {
            await OnFloatChanged(IsFloat);
        }
    }

    private static RenderFragment? RenderTemplate(RibbonTabItem item) => item.Component?.Render() ?? item.Template;
}
