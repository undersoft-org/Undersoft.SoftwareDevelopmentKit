using Microsoft.AspNetCore.Components.Forms;

namespace Undersoft.SDK.Service.Application.Components;

public partial class Table<TItem>
{
    [NotNull]
    protected string? EditModalTitleString { get; set; }

    [Parameter]
    public List<TItem> SelectedRows { get; set; } = new List<TItem>();

    [Parameter]
    public EventCallback<List<TItem>> SelectedRowsChanged { get; set; }

    [Parameter]
    public InsertRowMode InsertRowMode { get; set; } = InsertRowMode.Last;

    private bool IsLoading { get; set; }

    protected TableRenderMode ActiveRenderMode => RenderMode switch
    {
        TableRenderMode.Auto => ScreenSize < RenderModeResponsiveWidth ? TableRenderMode.CardView : TableRenderMode.Table,
        _ => RenderMode
    };

    protected BreakPoint ScreenSize { get; set; }

    [Parameter]
    public EditMode EditMode { get; set; }

    [Parameter]
    public TableRenderMode RenderMode { get; set; }

    [Parameter]
    public BreakPoint RenderModeResponsiveWidth { get; set; } = BreakPoint.Medium;

    [Parameter]
    public bool ScrollingDialogContent { get; set; }

    [Parameter]
    public bool IsKeyboard { get; set; } = true;

    [Parameter]
    public Func<TItem, string?>? SetRowClassFormatter { get; set; }

    [Parameter]
    public Func<TItem, Task>? OnAfterSaveAsync { get; set; }

    [Parameter]
    [NotNull]
    public string? EditModalTitle { get; set; }

    [Parameter]
    [NotNull]
    public string? AddModalTitle { get; set; }

    [Parameter]
    [NotNull]
    public TItem? EditModel { get; set; }

    [Parameter]
    public RenderFragment<TItem>? EditTemplate { get; set; }

    [Parameter]
    public RenderFragment<TItem>? BeforeRowButtonTemplate { get; set; }

    [Parameter]
    public RenderFragment<TItem>? RowButtonTemplate { get; set; }

    [Parameter]
    [NotNull]
    public string? ColumnButtonTemplateHeaderText { get; set; }

    [Parameter]
    public bool ClickToSelect { get; set; }

    [Parameter]
    public bool DoubleClickToEdit { get; set; }

    [Parameter]
    public bool AutoGenerateColumns { get; set; }

    [Parameter]
    public bool ShowLoading { get; set; }

    [NotNull]
    private string? DataServiceInvalidOperationText { get; set; }

    [Parameter]
    public IDataService<TItem>? DataService { get; set; }

    [Inject]
    [NotNull]
    private IDataService<TItem>? InjectDataService { get; set; }

    private async Task<QueryData<TItem>> InternalOnQueryAsync(QueryPageOptions options)
    {
        QueryData<TItem>? ret = null;
        if (OnQueryAsync != null)
        {
            ret = await OnQueryAsync(options);
        }
        else
        {
            var d = DataService ?? InjectDataService;
            ret = await d.QueryAsync(options);
        }
        return ret;
    }

    private async Task<bool> InternalOnDeleteAsync()
    {
        var ret = false;
        if (OnDeleteAsync != null)
        {
            ret = await OnDeleteAsync(SelectedRows);
        }
        else
        {
            if (Items != null)
            {
                ret = true;
            }
            else
            {
                var d = DataService ?? InjectDataService;
                ret = await d.DeleteAsync(SelectedRows);
            }
        }
        return ret;
    }

    private async Task<bool> InternalOnSaveAsync(TItem item, ItemChangedType changedType)
    {
        var ret = false;
        if (OnSaveAsync != null)
        {
            ret = await OnSaveAsync(item, changedType);
        }
        else
        {
            if (Items != null)
            {
                ret = true;
            }
            else
            {
                var d = DataService ?? InjectDataService;
                ret = await d.SaveAsync(item, changedType);
            }
        }
        return ret;
    }

    private async Task InternalOnAddAsync()
    {
        SelectedRows.Clear();
        if (OnAddAsync != null)
        {
            EditModel = await OnAddAsync();
        }
        else
        {
            EditModel = new TItem();
            if (Items == null)
            {
                var d = DataService ?? InjectDataService;
                await d.AddAsync(EditModel);
            }
        }
    }

    protected async Task ClickRow(TItem val)
    {
        if (ClickToSelect)
        {
            if (!IsMultipleSelect)
            {
                SelectedRows.Clear();
            }

            if (SelectedRows.Any(row => Equals(val, row)))
            {
                SelectedRows.RemoveAll(row => Equals(val, row));
            }
            else
            {
                SelectedRows.Add(val);
            }
            await OnSelectedRowsChanged();
        }

        if (OnClickRowCallback != null)
        {
            await OnClickRowCallback(val);
        }
    }

    private async Task OnSelectedRowsChanged()
    {
        if (SelectedRowsChanged.HasDelegate)
        {
            await SelectedRowsChanged.InvokeAsync(SelectedRows);
        }
        else
        {
            StateHasChanged();
        }
    }

    protected virtual bool CheckActive(TItem val) => SelectedRows.Any(row => Equals(val, row));

    protected Task OnClickRefreshAsync() => QueryAsync();

    protected void OnClickCardView()
    {
        var model = RenderMode;
        if (model == TableRenderMode.Auto)
        {
            model = ActiveRenderMode;
        }
        RenderMode = model switch
        {
            TableRenderMode.Table => TableRenderMode.CardView,
            _ => TableRenderMode.Table
        };
        StateHasChanged();
    }

    private async Task QueryAsync(bool shouldRender, int? pageIndex = null)
    {
        if (ScrollMode == ScrollMode.Virtual && VirtualizeElement != null)
        {
            await VirtualizeElement.RefreshDataAsync();
        }
        else
        {
            await InternalToggleLoading(true);
            if (pageIndex.HasValue)
            {
                PageIndex = pageIndex.Value;
            }
            await QueryData();
            await InternalToggleLoading(false);
        }

        if (shouldRender)
        {
            StateHasChanged();
        }
    }

    public Task QueryAsync(int? pageIndex = null) => QueryAsync(true, pageIndex);

    public async ValueTask ToggleLoading(bool state)
    {
        if (ShowLoading)
        {
            IsLoading = state;
            await InvokeExecuteAsync(Id, "load", state ? "show" : "hide");
        }
    }

    protected async ValueTask InternalToggleLoading(bool state)
    {
        if (ShowLoading && !IsLoading)
        {
            await InvokeExecuteAsync(Id, "load", state ? "show" : "hide");
        }
    }

    protected async Task QueryData()
    {
        if (Items == null)
        {
            if (OnQueryAsync == null && DynamicContext != null && typeof(TItem).IsAssignableTo(typeof(IDynamicObject)))
            {
                QueryDynamicItems(DynamicContext);
            }
            else
            {
                await OnQuery();
            }
        }
        else
        {
            ResetSelectedRows(Items);
            RowsCache = null;
        }

        async Task OnQuery()
        {
            QueryData<TItem>? queryData = null;
            var queryOption = BuildQueryPageOptions();

            queryData = await InternalOnQueryAsync(queryOption);
            TotalCount = queryData.TotalCount;
            PageCount = (int)Math.Ceiling(TotalCount * 1.0 / Math.Max(1, PageItems));
            IsAdvanceSearch = queryData.IsAdvanceSearch;
            QueryItems = queryData.Items ?? Enumerable.Empty<TItem>();

            ResetSelectedRows(QueryItems);

            ProcessData();

            if (IsTree)
            {
                await ProcessTreeData();
            }

            RowsCache = null;

            void ProcessData()
            {
                var filtered = queryData.IsFiltered;
                var sorted = queryData.IsSorted;
                var searched = queryData.IsSearch;

                if (!searched && queryOption.Searchs.Any())
                {
                    QueryItems = QueryItems.Where(queryOption.Searchs.GetFilterFunc<TItem>(FilterLogic.Or));
                    TotalCount = QueryItems.Count();
                }

                if (!IsAdvanceSearch && queryOption.CustomerSearchs.Any())
                {
                    QueryItems = QueryItems.Where(queryOption.CustomerSearchs.GetFilterFunc<TItem>());
                    TotalCount = QueryItems.Count();
                    IsAdvanceSearch = true;
                }

                if (!filtered && queryOption.Filters.Any())
                {
                    QueryItems = QueryItems.Where(queryOption.Filters.GetFilterFunc<TItem>());
                    TotalCount = QueryItems.Count();
                }

                if (!sorted)
                {
                    if (OnSort == null && queryOption.SortOrder != SortOrder.Unset && !string.IsNullOrEmpty(queryOption.SortName))
                    {
                        var invoker = Utility.GetSortFunc<TItem>();
                        QueryItems = invoker(QueryItems, queryOption.SortName, queryOption.SortOrder);
                    }
                    else if (queryOption.SortList.Any())
                    {
                        var invoker = Utility.GetSortListFunc<TItem>();
                        QueryItems = invoker(QueryItems, queryOption.SortList);
                    }
                }
            }

            async Task ProcessTreeData()
            {
                var treeNodes = new List<TableTreeNode<TItem>>();
                if (TreeNodeConverter != null)
                {
                    treeNodes.AddRange(await TreeNodeConverter(QueryItems));
                }

                if (treeNodes.Any())
                {
                    await CheckExpand(treeNodes);
                }

                TreeRows.Clear();
                TreeRows.AddRange(treeNodes);

                async Task CheckExpand(IEnumerable<TableTreeNode<TItem>> nodes)
                {
                    foreach (var node in nodes)
                    {
                        await TreeNodeCache.CheckExpandAsync(node, GetChildrenRowAsync);

                        if (node.Items.Any())
                        {
                            await CheckExpand(node.Items);
                        }
                    }
                }
            }
        }
    }

    private QueryPageOptions BuildQueryPageOptions()
    {
        var queryOption = new QueryPageOptions()
        {
            IsPage = IsPagination,
            PageIndex = PageIndex,
            PageItems = PageItems,
            SearchText = SearchText,
            SortOrder = SortOrder,
            SortName = SortName,
            SearchModel = SearchModel,
            StartIndex = StartIndex
        };

        queryOption.Filters.AddRange(Filters.Values);
        queryOption.Searchs.AddRange(GetSearchs());
        queryOption.AdvanceSearchs.AddRange(GetAdvanceSearchs());
        queryOption.CustomerSearchs.AddRange(GetCustomerSearchs());

        if (!string.IsNullOrEmpty(SortString))
        {
            queryOption.SortList.AddRange(SortString.Split(",", StringSplitOptions.RemoveEmptyEntries));
        }

        if (CustomerSearchModel != null)
        {
            queryOption.SearchModel = CustomerSearchModel;
        }
        return queryOption;
    }

    private void ResetSelectedRows(IEnumerable<TItem> items)
    {
        if (SelectedRows.Any())
        {
            SelectedRows = items.Where(i => SelectedRows.Any(row => Equals(i, row))).ToList();
        }
    }

    public bool Equals(TItem? x, TItem? y) => DynamicContext?.EqualityComparer?.Invoke((IDynamicObject?)x, (IDynamicObject?)y) ?? this.Equals<TItem>(x, y);

    private async Task OnClickExtensionButton(TItem item, TableCellButtonArgs args)
    {
        if ((IsMultipleSelect || ClickToSelect) && args.AutoSelectedRowWhenClick)
        {
            SelectedRows.Clear();
            SelectedRows.Add(item);
            StateHasChanged();
        }
        if (args.AutoRenderTableWhenClick)
        {
            await QueryAsync();
        }
    }

    private async Task ClickEditButton(TItem item)
    {
        SelectedRows.Clear();
        SelectedRows.Add(item);
        await OnSelectedRowsChanged();

        await EditAsync();
    }

    private async Task ClickUpdateButtonCallback()
    {
        var context = new EditContext(EditModel);
        await SaveAsync(context, AddInCell ? ItemChangedType.Add : ItemChangedType.Update);
    }

    protected async Task DoubleClickRow(TItem item)
    {
        if (DoubleClickToEdit)
        {
            await ClickEditButton(item);
        }

        if (OnDoubleClickRowCallback != null)
        {
            await OnDoubleClickRowCallback(item);
        }

        StateHasChanged();
    }

    protected Func<Task<bool>> ClickBeforeDelete(TItem item) => () =>
    {
        SelectedRows.Clear();
        SelectedRows.Add(item);

        StateHasChanged();
        return Task.FromResult(true);
    };
}
