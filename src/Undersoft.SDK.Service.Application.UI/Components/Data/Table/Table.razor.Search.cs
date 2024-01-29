using Microsoft.AspNetCore.Components.Web;

namespace Undersoft.SDK.Service.Application.Components;

public partial class Table<TItem>
{
    protected string? AdvanceSearchClass => CssBuilder.Default("btn btn-secondary")
        .AddClass("btn-info", IsAdvanceSearch)
        .Build();

    protected bool IsAdvanceSearch { get; set; }

    [Parameter]
    public RenderFragment<TItem>? SearchTemplate { get; set; }

    [Parameter]
    public TItem SearchModel { get; set; } = new TItem();

    [Parameter]
    public ITableSearchModel? CustomerSearchModel { get; set; }

    [Parameter]
    public RenderFragment<ITableSearchModel>? CustomerSearchTemplate { get; set; }

    [Parameter]
    public bool ShowSearch { get; set; }

    [Parameter]
    public bool CollapsedTopSearch { get; set; }

    [Parameter]
    public bool ShowSearchText { get; set; } = true;

    [Parameter]
    public bool ShowSearchTextTooltip { get; set; } = true;

    [Parameter]
    public bool ShowResetButton { get; set; } = true;

    [Parameter]
    public bool ShowSearchButton { get; set; } = true;

    [Parameter]
    public bool ShowAdvancedSearch { get; set; } = true;

    [Parameter]
    public string? SearchText { get; set; }

    [Parameter]
    public SearchMode SearchMode { get; set; }

    [Parameter]
    public int SearchDialogItemsPerRow { get; set; } = 2;

    [Parameter]
    public RowType SearchDialogRowType { get; set; } = RowType.Inline;

    [Parameter]
    public Alignment SearchDialogLabelAlign { get; set; }

    [Parameter]
    public Func<TItem, Task>? OnResetSearchAsync { get; set; }

    protected async Task ResetSearchClick()
    {
        await ToggleLoading(true);
        if (CustomerSearchModel != null)
        {
            CustomerSearchModel.Reset();
        }
        else if (OnResetSearchAsync != null)
        {
            await OnResetSearchAsync(SearchModel);
        }
        else if (SearchTemplate == null)
        {
            Utility.Reset(SearchModel);
        }

        PageIndex = 1;
        await QueryAsync();
        await ToggleLoading(false);
    }

    protected async Task SearchClick()
    {
        PageIndex = 1;
        await QueryAsync();
    }

    [Parameter]
    public Size SearchDialogSize { get; set; } = Size.ExtraExtraLarge;

    [Parameter]
    public bool SearchDialogIsDraggable { get; set; }

    [Parameter]
    public bool SearchDialogShowMaximizeButton { get; set; } = true;

    protected async Task ShowSearchDialog()
    {
        if (CustomerSearchModel != null && CustomerSearchTemplate != null)
        {
            await DialogService.ShowSearchDialog(CreateCustomerModelDialog());
        }
        else
        {
            await DialogService.ShowSearchDialog(CreateModelDialog());
        }

        SearchDialogOption<TItem> CreateModelDialog() => new()
        {
            Class = "modal-dialog-table",
            IsScrolling = ScrollingDialogContent,
            Title = SearchModalTitle,
            Model = SearchModel,
            DialogBodyTemplate = SearchTemplate,
            OnResetSearchClick = ResetSearchClick,
            OnSearchClick = SearchClick,
            RowType = SearchDialogRowType,
            ItemsPerRow = SearchDialogItemsPerRow,
            LabelAlign = SearchDialogLabelAlign,
            Size = SearchDialogSize,
            Items = Columns.Where(i => i.Searchable),
            IsDraggable = SearchDialogIsDraggable,
            ShowMaximizeButton = SearchDialogShowMaximizeButton,
            ShowUnsetGroupItemsOnTop = ShowUnsetGroupItemsOnTop
        };

        SearchDialogOption<ITableSearchModel> CreateCustomerModelDialog() => new()
        {
            IsScrolling = ScrollingDialogContent,
            Title = SearchModalTitle,
            Model = CustomerSearchModel,
            DialogBodyTemplate = CustomerSearchTemplate,
            OnResetSearchClick = ResetSearchClick,
            OnSearchClick = SearchClick,
            RowType = SearchDialogRowType,
            ItemsPerRow = SearchDialogItemsPerRow,
            Size = SearchDialogSize,
            LabelAlign = SearchDialogLabelAlign
        };
    }

    protected IEnumerable<IFilterAction> GetCustomerSearchs()
    {
        var searchs = new List<IFilterAction>();
        if (CustomerSearchModel != null)
        {
            searchs.AddRange(CustomerSearchModel.GetSearchs());
        }
        return searchs;
    }

    protected List<IFilterAction> GetAdvanceSearchs()
    {
        var searchs = new List<IFilterAction>();
        if (ShowAdvancedSearch && CustomerSearchModel == null && SearchModel != null)
        {
            var searchColumns = Columns.Where(i => i.Searchable);
            foreach (var property in SearchModel.GetType().GetProperties().Where(i => searchColumns.Any(col => col.GetFieldName() == i.Name)))
            {
                var v = property.GetValue(SearchModel);
                if (v != null && v.ToString() != string.Empty)
                {
                    searchs.Add(new SearchFilterAction(property.Name, v, FilterAction.Equal));
                }
            }
        }
        return searchs;
    }

    protected List<IFilterAction> GetSearchs() => Columns.Where(col => col.Searchable).ToSearchs(SearchText);

    private async Task OnSearchKeyup(KeyboardEventArgs args)
    {
        if (args.Key == "Enter")
        {
            await SearchClick();
        }
        else if (args.Key == "Escape")
        {
            await ClearSearchClick();
        }
    }

    protected async Task ClearSearchClick()
    {
        SearchText = null;
        await ResetSearchClick();
    }

    private IEnumerable<ITableColumn> GetSearchColumns() => Columns.Where(c => c.Searchable);
}
