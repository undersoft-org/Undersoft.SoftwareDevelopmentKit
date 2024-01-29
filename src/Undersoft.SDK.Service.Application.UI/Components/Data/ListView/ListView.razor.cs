namespace Undersoft.SDK.Service.Application.Components;

public partial class ListView<TItem> : Component
{
    private string? ClassString => CssBuilder.Default("listview")
        .AddClass("is-vertical", IsVertical)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? BodyClassString => CssBuilder.Default("listview-body")
        .AddClass("is-group", GroupName != null)
        .Build();

    [Parameter]
    public RenderFragment? HeaderTemplate { get; set; }

    [Parameter]
#if NET6_0_OR_GREATER
    [EditorRequired]
#endif
    public RenderFragment<TItem>? BodyTemplate { get; set; }

    [Parameter]
    public RenderFragment? FooterTemplate { get; set; }

    [Parameter]
    public IEnumerable<TItem>? Items { get; set; }

    [Parameter]
    public bool Pageable { get; set; }

    [Parameter]
    public Func<TItem, object?>? GroupName { get; set; }

    [Parameter]
    public bool Collapsable { get; set; }

    [Parameter]
    public bool IsAccordion { get; set; }

    [Parameter]
    public Func<CollapseItem, Task>? OnCollapseChanged { get; set; }

    [Parameter]
    public Func<object?, bool>? CollapsedGroupCallback { get; set; }

    [Parameter]
    public Func<QueryPageOptions, Task<QueryData<TItem>>>? OnQueryAsync { get; set; }

    [Parameter]
    public Func<TItem, Task>? OnListViewItemClick { get; set; }

    [Parameter]
    public bool IsVertical { get; set; }

    [Parameter]
    public int PageItems { get; set; } = 20;

    private int PageIndex { get; set; }

    protected int TotalCount { get; set; }

    protected IEnumerable<TItem> Rows => Items ?? Enumerable.Empty<TItem>();

    protected override async Task OnParametersSetAsync()
    {
        if (Items == null)
        {
            await QueryData();
        }
    }

    private bool IsCollapsed(int index, object? groupKey) => CollapsedGroupCallback?.Invoke(groupKey) ?? index > 0;

    protected Task OnPageLinkClick(int pageIndex) => QueryAsync(pageIndex);

    public async Task QueryAsync(int pageIndex = 1)
    {
        PageIndex = pageIndex;
        await QueryData();
        StateHasChanged();
    }

    protected async Task QueryData()
    {
        QueryData<TItem>? queryData = null;
        if (OnQueryAsync != null)
        {
            queryData = await OnQueryAsync(new QueryPageOptions()
            {
                PageIndex = PageIndex,
                PageItems = PageItems,
            });
        }
        if (queryData != null)
        {
            Items = queryData.Items;
            TotalCount = queryData.TotalCount;
        }
    }

    private int PageCount => (int)Math.Ceiling(TotalCount * 1.0 / PageItems);

    protected async Task OnClick(TItem item)
    {
        if (OnListViewItemClick != null)
        {
            await OnListViewItemClick(item);
        }
    }
}
