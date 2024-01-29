namespace Undersoft.SDK.Service.Application.Components;

public partial class Table<TItem>
{
    [Parameter]
    public bool IsPagination { get; set; }

    [Parameter]
    public bool ShowTopPagination { get; set; }

    [Parameter]
    public bool ShowLineNo { get; set; }

    [Parameter]
    [NotNull]
    public string? LineNoText { get; set; }

    [Parameter]
    [NotNull]
    public IEnumerable<int>? PageItemsSource { get; set; }

    [Parameter]
    public Func<QueryPageOptions, Task<QueryData<TItem>>>? OnQueryAsync { get; set; }

    protected int TotalCount { get; set; }

    protected int PageCount { get; set; }

    protected int PageIndex { get; set; } = 1;

    [Parameter]
    public int PageItems { get; set; }

    [Parameter]
    public bool ShowGotoNavigator { get; set; } = true;

    [Parameter]
    public string? GotoNavigatorLabelText { get; set; }

    [Parameter]
    public RenderFragment? GotoTemplate { get; set; }

    [Parameter]
    public bool ShowPageInfo { get; set; } = true;

    [Parameter]
    public string? PageInfoText { get; set; }

    [Parameter]
    public RenderFragment? PageInfoTemplate { get; set; }

    protected int StartIndex { get; set; }

    [NotNull]
    protected RenderFragment? InternalPageInfoTemplate => builder =>
    {
        if (PageInfoTemplate != null)
        {
            builder.AddContent(0, PageInfoTemplate);
        }
        else if (!string.IsNullOrEmpty(PageInfoText))
        {
            builder.OpenElement(1, "div");
            builder.AddAttribute(2, "class", "page-info");
            builder.AddContent(3, PageInfoText);
            builder.CloseElement();
        }
        else
        {
            builder.AddContent(4, RenderPageInfo);
        }
    };

    protected async Task OnPageLinkClick(int pageIndex)
    {
        if (pageIndex != PageIndex)
        {
            PageIndex = pageIndex;

            SelectedRows.Clear();

            await QueryAsync(false);

            await OnSelectedRowsChanged();
        }
    }

    protected async Task OnPageItemsValueChanged(int pageItems)
    {
        if (PageItems != pageItems)
        {
            PageIndex = 1;
            PageItems = pageItems;
            await QueryAsync();
        }
    }

    private List<SelectedItem>? _pageItemsSource;

    protected List<SelectedItem> GetPageItemsSource()
    {
        _pageItemsSource ??= PageItemsSource.Select(i => new SelectedItem($"{i}", Localizer["PageItemsText", i].Value)).ToList();
        return _pageItemsSource;
    }
}
