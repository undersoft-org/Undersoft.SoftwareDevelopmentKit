using Microsoft.AspNetCore.Components.Web.Virtualization;
using System.Globalization;
using Undersoft.SDK.Instant.Proxies;

namespace Undersoft.SDK.Service.Application.Components;

#if NET6_0_OR_GREATER
[CascadingTypeParameter(nameof(TItem))]
#endif
[JSModuleAutoLoader(JSObjectReference = true)]
public partial class Table<TItem> : ITable, IModelEqualityComparer<TItem> where TItem : class, new()
{
    protected Virtualize<TItem>? VirtualizeElement { get; set; }

    [NotNull]
    private JSInterop<Table<TItem>>? Interop { get; set; }

    private string? ClassName => CssBuilder.Default("table-container")
        .AddClass("table-fixed", IsFixedHeader && !Height.HasValue)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? StyleString => CssBuilder.Default()
        .AddClass($"height: {Height}px;", IsFixedHeader && Height.HasValue)
        .AddStyleFromAttributes(AdditionalAttributes)
        .Build();

    private string? TableClassName => CssBuilder.Default("table")
        .AddClass("table-sm", TableSize == TableSize.Compact)
        .AddClass("table-excel", IsExcel)
        .AddClass("table-bordered", IsBordered)
        .AddClass("table-striped table-hover", IsStriped)
        .AddClass("table-layout-fixed", IsFixedHeader)
        .Build();

    protected string? WrapperClassName => CssBuilder.Default()
        .AddClass("table-wrapper", IsBordered)
        .AddClass("is-clickable", ClickToSelect || DoubleClickToEdit || OnClickRowCallback != null || OnDoubleClickRowCallback != null)
        .AddClass("table-scroll", !IsFixedHeader || FixedColumn)
        .AddClass("table-fixed", IsFixedHeader)
        .AddClass("table-fixed-column", FixedColumn)
        .AddClass("table-resize", AllowResizing)
        .AddClass("table-fixed-body", RenderMode == TableRenderMode.CardView && IsFixedHeader)
        .AddClass("table-striped table-hover", ActiveRenderMode == TableRenderMode.CardView && IsStriped)
        .Build();

    private bool FixedColumn => FixedExtendButtonsColumn || FixedMultipleColumn || FixedDetailRowHeaderColumn || FixedLineNoColumn || Columns.Any(c => c.Fixed);

    protected string? GetRowClassString(TItem item, string? css = null) => CssBuilder.Default(css)
        .AddClass(SetRowClassFormatter?.Invoke(item))
        .AddClass("active", CheckActive(item))
        .AddClass("is-master", ShowDetails())
        .AddClass("is-click", ClickToSelect)
        .AddClass("is-dblclick", DoubleClickToEdit)
        .AddClass("is-edit", EditInCell)
        .Build();

    protected string? GetDetailBarClassString(TItem item) => CssBuilder.Default("table-cell is-bar")
        .AddClass("is-load", DetailRows.Contains(item))
        .Build();

    private string? CopyColumnButtonIconString => CssBuilder.Default("col-copy")
        .AddClass(CopyColumnButtonIcon)
        .Build();

    protected string? GetDetailRowClassString(TItem item) => CssBuilder.Default("is-detail")
        .AddClass("show", ExpandRows.Contains(item))
        .Build();

    protected string? GetDetailCaretClassString(TItem item) => CssBuilder.Default("node-icon")
        .AddClass(TreeIcon, !ExpandRows.Contains(item))
        .AddClass(TreeExpandIcon, ExpandRows.Contains(item))
        .Build();

    private string? LineCellClassString => CssBuilder.Default("table-cell")
        .AddClass(LineNoColumnAlignment.ToDescriptionString())
        .Build();

    private string? ExtendButtonsCellClassString => CssBuilder.Default("table-cell")
        .AddClass(ExtendButtonColumnAlignment.ToDescriptionString())
        .Build();

    private string? GetSortTooltip(ITableColumn col) => SortName != col.GetFieldName()
        ? UnsetText
        : SortOrder switch
        {
            SortOrder.Asc => SortAscText,
            SortOrder.Desc => SortDescText,
            _ => UnsetText
        };

    private static string GetHeaderTooltipText(string? headerTooltip, string displayName) => headerTooltip ?? displayName;

    private static string? GetColspan(int colspan) => colspan > 1 ? colspan.ToString() : null;

    private bool IsShowFooter => ShowFooter && (Rows.Any() || !IsHideFooterWhenNoData);

    private int PageStartIndex => Rows.Any() ? (PageIndex - 1) * PageItems + 1 : 0;

    private string? PageInfoLabelString => Localizer[nameof(PageInfoText), PageStartIndex, (PageIndex - 1) * PageItems + Rows.Count, TotalCount];

    private static string? GetColWidthString(int? width) => width.HasValue ? $"width: {width.Value}px;" : null;

    [Parameter]
    public int? Height { get; set; }

    [Parameter]
    public bool IsFixedHeader { get; set; }

    [Parameter]
    public RenderFragment? MultiHeaderTemplate { get; set; }

    [Parameter]
    public string? CopyColumnTooltipText { get; set; }

    [Parameter]
    public string? CopyColumnCopiedTooltipText { get; set; }

    [Parameter]
    public bool ShowCopyColumnTooltip { get; set; } = true;

    protected List<TItem> ExpandRows { get; } = new List<TItem>();

    [Parameter]
    public bool IsExcel { get; set; }

    [Parameter]
    public bool? IsDetails { get; set; }

    [Parameter]
    public bool IsHideFooterWhenNoData { get; set; }

    [Parameter]
    public int EditDialogItemsPerRow { get; set; } = 2;

    [Parameter]
    public RowType EditDialogRowType { get; set; } = RowType.Inline;

    [Parameter]
    public Alignment EditDialogLabelAlign { get; set; }

    [Parameter]
    public bool? DisableAutoSubmitFormByEnter { get; set; }

    [Parameter]
    public int DetailColumnWidth { get; set; }

    [Parameter]
    public int ShowCheckboxTextColumnWidth { get; set; }

    [Parameter]
    public int CheckboxColumnWidth { get; set; }

    [Parameter]
    public int LineNoColumnWidth { get; set; }

    [Parameter]
    public Alignment LineNoColumnAlignment { get; set; }

    [Parameter]
    public Action<TItem>? OnBeforeRenderRow { get; set; }

    [Parameter]
    public Func<Table<TItem>, Task>? OnAfterRenderCallback { get; set; }

    [Parameter]
    public Func<string, TItem, object?, Task>? OnDoubleClickCellCallback { get; set; }

    [Parameter]
    public ScrollMode ScrollMode { get; set; }

    [Parameter]
    public float RowHeight { get; set; } = 38f;

    [Inject]
    [NotNull]
    private IOptionsMonitor<ApplicationOptions>? Options { get; set; }

    [Parameter]
    public bool IsTracking { get; set; }

    [Inject]
    [NotNull]
    private ILookupService? LookupService { get; set; }

    private Task OnBreakPointChanged(BreakPoint size)
    {
        if (size != ScreenSize)
        {
            ScreenSize = size;
            StateHasChanged();
        }
        return Task.CompletedTask;
    }

    private bool ShowDetails() => IsDetails == null
        ? DetailRowTemplate != null
        : IsDetails.Value && DetailRowTemplate != null;

    public void ExpandDetailRow(TItem item)
    {
        DetailRows.Add(item);
        if (ExpandRows.Contains(item))
        {
            ExpandRows.Remove(item);
        }
        else
        {
            ExpandRows.Add(item);
        }
    }

    protected List<TItem> DetailRows { get; } = new List<TItem>();

    public List<ITableColumn> Columns { get; } = new(50);

    [Parameter]
    public RenderFragment<TItem>? DetailRowTemplate { get; set; }

    [Parameter]
    public RenderFragment<TItem>? TableColumns { get; set; }

    [Parameter]
    public RenderFragment<IEnumerable<TItem>>? TableFooter { get; set; }

    [Parameter]
    public RenderFragment<IEnumerable<TItem>>? FooterTemplate { get; set; }

    [Parameter]
    public IEnumerable<TItem>? Items { get; set; }

    [Parameter]
    public EventCallback<IEnumerable<TItem>> ItemsChanged { get; set; }

    [Parameter]
    public TableSize TableSize { get; set; }

    [Parameter]
    public RenderFragment? EmptyTemplate { get; set; }

    [Parameter]
    public string? EmptyText { get; set; }

    [Parameter]
    public string? EmptyImage { get; set; }

    [Parameter]
    public bool ShowEmpty { get; set; }

    [Parameter]
    public bool ShowFilterHeader { get; set; }

    [Parameter]
    public bool ShowMultiFilterHeader { get; set; }

    [Parameter]
    public bool ShowFooter { get; set; }

    [Parameter]
    public bool AllowResizing { get; set; }

    [Parameter]
    public bool HeaderTextWrap { get; set; }

    [Parameter]
    public bool IsStriped { get; set; }

    [Parameter]
    public bool IsBordered { get; set; }

    [Parameter]
    public bool IsAutoRefresh { get; set; }

    [Parameter]
    public int AutoRefreshInterval { get; set; } = 2000;

    [Parameter]
    public TableHeaderStyle HeaderStyle { get; set; } = TableHeaderStyle.None;

    [Parameter]
    public Func<TItem, Task>? OnClickRowCallback { get; set; }

    [Parameter]
    public Func<TItem, Task>? OnDoubleClickRowCallback { get; set; }

    [Parameter]
    public Func<TItem, bool>? ShowDetailRow { get; set; }

    [Parameter]
    public IDynamicObjectContext? DynamicContext { get; set; }

    [Parameter]
    [NotNull]
    public string? UnsetText { get; set; }

    [Parameter]
    [NotNull]
    public string? SortAscText { get; set; }

    [Parameter]
    [NotNull]
    public string? SortDescText { get; set; }

    [Parameter]
    public Func<List<ITableColumn>, Task>? OnColumnCreating { get; set; }

    private bool OnAfterRenderIsTriggered { get; set; }

    [Parameter]
    [NotNull]
    public Type? CustomKeyAttribute { get; set; } = typeof(KeyAttribute);

    [Parameter]
    public Func<TItem, TItem, bool>? ModelEqualityComparer { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    private bool UpdateSortTooltip { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        TreeNodeCache ??= new(Equals);

        OnInitLocalization();

        Interop = new JSInterop<Table<TItem>>(JSRuntime);

        InternalOnSortAsync = async (sortName, sortOrder) =>
        {
            if (OnSort != null)
            {
                SortString = OnSort(sortName, SortOrder);
            }

            await QueryAsync();
        };

        OnFilterAsync = async () =>
        {
            PageIndex = 1;
            await QueryAsync();
        };
    }

    protected override async Task OnInitializedAsync()
    {
        if (IsTree && Items != null && TreeNodeConverter != null)
        {
            TreeRows.AddRange(await TreeNodeConverter(Items));
        }
    }

    private void OnInitParameters()
    {
        var op = Options.CurrentValue;
        if (ShowCheckboxTextColumnWidth == 0)
        {
            ShowCheckboxTextColumnWidth = op.TableSettings.ShowCheckboxTextColumnWidth;
        }

        if (DetailColumnWidth == 0)
        {
            DetailColumnWidth = op.TableSettings.DetailColumnWidth;
        }

        if (LineNoColumnWidth == 0)
        {
            LineNoColumnWidth = op.TableSettings.LineNoColumnWidth;
        }

        if (CheckboxColumnWidth == 0)
        {
            CheckboxColumnWidth = op.TableSettings.CheckboxColumnWidth;
        }

        if (op.TableSettings.TableRenderMode != null && RenderMode == TableRenderMode.Auto)
        {
            RenderMode = op.TableSettings.TableRenderMode.Value;
        }

        PageItemsSource ??= new int[] { 20, 50, 100, 200, 500, 1000 };

        if (PageItems == 0)
        {
            PageItems = PageItemsSource.First();
        }

        if (ExtendButtonColumnAlignment == Alignment.None)
        {
            ExtendButtonColumnAlignment = Alignment.Center;
        }

        if (LineNoColumnAlignment == Alignment.None)
        {
            LineNoColumnAlignment = Alignment.Center;
        }

        SortIconAsc ??= IconTheme.GetIconByKey(ComponentIcons.TableSortIconAsc);
        SortIconDesc ??= IconTheme.GetIconByKey(ComponentIcons.TableSortDesc);
        SortIcon ??= IconTheme.GetIconByKey(ComponentIcons.TableSortIcon);
        FilterIcon ??= IconTheme.GetIconByKey(ComponentIcons.TableFilterIcon);
        ExportButtonIcon ??= IconTheme.GetIconByKey(ComponentIcons.TableExportButtonIcon);

        AddButtonIcon ??= IconTheme.GetIconByKey(ComponentIcons.TableAddButtonIcon);
        EditButtonIcon ??= IconTheme.GetIconByKey(ComponentIcons.TableEditButtonIcon);
        DeleteButtonIcon ??= IconTheme.GetIconByKey(ComponentIcons.TableDeleteButtonIcon);
        RefreshButtonIcon ??= IconTheme.GetIconByKey(ComponentIcons.TableRefreshButtonIcon);
        CardViewButtonIcon ??= IconTheme.GetIconByKey(ComponentIcons.TableCardViewButtonIcon);
        ColumnListButtonIcon ??= IconTheme.GetIconByKey(ComponentIcons.TableColumnListButtonIcon);
        ExcelExportIcon ??= IconTheme.GetIconByKey(ComponentIcons.TableExcelExportIcon);
        SearchButtonIcon ??= IconTheme.GetIconByKey(ComponentIcons.TableSearchButtonIcon);
        ResetSearchButtonIcon ??= IconTheme.GetIconByKey(ComponentIcons.TableResetSearchButtonIcon);
        CloseButtonIcon ??= IconTheme.GetIconByKey(ComponentIcons.TableCloseButtonIcon);
        SaveButtonIcon ??= IconTheme.GetIconByKey(ComponentIcons.TableSaveButtonIcon);
        AdvanceButtonIcon ??= IconTheme.GetIconByKey(ComponentIcons.TableAdvanceButtonIcon);
        CancelButtonIcon ??= IconTheme.GetIconByKey(ComponentIcons.TableCancelButtonIcon);
        CopyColumnButtonIcon ??= IconTheme.GetIconByKey(ComponentIcons.TableCopyColumnButtonIcon);
        GearIcon ??= IconTheme.GetIconByKey(ComponentIcons.TableGearIcon);

        TreeIcon ??= IconTheme.GetIconByKey(ComponentIcons.TableTreeIcon);
        TreeExpandIcon ??= IconTheme.GetIconByKey(ComponentIcons.TableTreeExpandIcon);
        TreeNodeLoadingIcon ??= IconTheme.GetIconByKey(ComponentIcons.TableTreeNodeLoadingIcon);
    }

    protected bool FirstRender { get; set; } = true;

    protected CancellationTokenSource? AutoRefreshCancelTokenSource { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        OnInitParameters();

        if (ScrollMode == ScrollMode.Virtual)
        {
            IsFixedHeader = true;
            RenderMode = TableRenderMode.Table;
        }

        RowsCache = null;

        if (IsExcel)
        {
            IsStriped = false;
            IsMultipleSelect = true;
            IsTree = false;
        }

        if (!FirstRender)
        {
            ResetDynamicContext();

            var col = Columns.FirstOrDefault(i => i.Sortable && i.DefaultSort);
            if (col != null)
            {
                SortName = col.GetFieldName();
                SortOrder = col.DefaultSortOrder;
            }
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            IsLoading = true;

            FirstRender = false;

            if (DynamicContext != null && typeof(TItem).IsAssignableTo(typeof(IDynamicObject)))
            {
                AutoGenerateColumns = false;

                var cols = DynamicContext.GetColumns();
                Columns.Clear();
                Columns.AddRange(cols);
            }

            if (AutoGenerateColumns)
            {
                var cols = Utility.GetTableColumns<TItem>(Columns);
                Columns.Clear();
                Columns.AddRange(cols);
            }

            if (OnColumnCreating != null)
            {
                await OnColumnCreating(Columns);
            }

            InternalResetVisibleColumns(Columns.Select(i => new ColumnVisibleItem(i.GetFieldName(), i.Visible)));

            var col = Columns.FirstOrDefault(i => i.Sortable && i.DefaultSort);
            if (col != null)
            {
                SortName = col.GetFieldName();
                SortOrder = col.DefaultSortOrder;
            }

            await QueryAsync();

            _init = true;

            IsLoading = false;
        }

        if (!OnAfterRenderIsTriggered && OnAfterRenderCallback != null)
        {
            OnAfterRenderIsTriggered = true;
            await OnAfterRenderCallback(this);
        }

        if (_init)
        {
            _init = false;
            await InvokeVoidAsync("init", Id);
        }

        if (UpdateSortTooltip)
        {
            UpdateSortTooltip = false;
            await InvokeExecuteAsync(Id, "sort");
        }

        if (!_loop && IsAutoRefresh && AutoRefreshInterval > 500)
        {
            _loop = true;
            await LoopQueryAsync();
            _loop = false;
        }
    }

    protected override async Task ModuleInitAsync() => ScreenSize = await InvokeAsync<BreakPoint>("getResponsive");

    private void InternalResetVisibleColumns(IEnumerable<ColumnVisibleItem> columns)
    {
        VisibleColumns.Clear();
        VisibleColumns.AddRange(columns);
    }

    public void ResetVisibleColumns(IEnumerable<ColumnVisibleItem> columns)
    {
        InternalResetVisibleColumns(columns);
        StateHasChanged();
    }

    protected async Task LoopQueryAsync()
    {
        try
        {
            AutoRefreshCancelTokenSource ??= new();
            await Task.Delay(AutoRefreshInterval, AutoRefreshCancelTokenSource.Token);

            await QueryData();
            StateHasChanged();
        }
        catch (TaskCanceledException)
        {

        }
    }

    private bool _loop;
    private bool _init;

    protected bool CheckShownWithBreakpoint(ITableColumn col) => ScreenSize >= col.ShownWithBreakPoint;

    private IEnumerable<TItem> QueryItems { get; set; } = Enumerable.Empty<TItem>();

    [NotNull]
    private List<TItem>? RowsCache { get; set; }

    public List<TItem> Rows
    {
        get
        {
            RowsCache ??= IsTree ? TreeRows.GetAllItems() : (Items ?? QueryItems).ToList();
            return RowsCache;
        }
    }

    protected RenderFragment GetValue(ITableColumn col, TItem item) => async builder =>
    {
        if (col.Template != null)
        {
            builder.AddContent(0, col.Template(item));
        }
        else if (col.ComponentType == typeof(ColorPicker))
        {
            var val = GetItemValue(col.GetFieldName(), item);
            var v = val?.ToString() ?? "#000";
            var style = $"background-color: {v};";
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "is-color");
            builder.AddAttribute(2, "style", style);
            builder.CloseElement();
        }
        else
        {
            var val = GetItemValue(col.GetFieldName(), item);

            if (col.Lookup == null && !string.IsNullOrEmpty(col.LookupServiceKey))
            {
                col.Lookup = LookupService.GetItemsByKey(col.LookupServiceKey);
            }

            if (col.Lookup == null && val is bool v1)
            {
                builder.OpenComponent(0, typeof(Switch));
                builder.AddAttribute(1, "Value", v1);
                builder.AddAttribute(2, "IsDisabled", true);
                builder.CloseComponent();
            }
            else if (col.Lookup != null && val != null)
            {
                var lookupVal = col.Lookup.FirstOrDefault(l => l.Value.Equals(val.ToString(), col.LookupStringComparison));
                if (lookupVal != null)
                {
                    builder.AddContent(0, RenderTooltip(lookupVal.Text));
                }
            }
            else
            {
                string? content;
                if (col.Formatter != null)
                {
                    content = await col.Formatter(new TableColumnContext<TItem, object?>(item, val));
                }
                else if (!string.IsNullOrEmpty(col.FormatString))
                {
                    content = Utility.Format(val, col.FormatString);
                }
                else if (col.PropertyType.IsDateTime())
                {
                    content = Utility.Format(val, CultureInfo.CurrentUICulture.DateTimeFormat);
                }
                else if (val is IEnumerable<object> v)
                {
                    content = string.Join(",", v);
                }
                else
                {
                    content = val?.ToString();
                }
                builder.AddContent(0, RenderTooltip(content));
            }
        }

        RenderFragment RenderTooltip(string? text) => pb =>
        {
            if (col.ShowTips && !string.IsNullOrEmpty(text))
            {
                pb.OpenComponent<Tooltip>(0);
                pb.AddAttribute(1, nameof(Tooltip.Title), text);
                pb.AddAttribute(2, nameof(Tooltip.ChildContent), RenderContent());
                pb.CloseComponent();
            }
            else
            {
                pb.AddContent(3, text);
            }

            RenderFragment RenderContent() => context => context.AddContent(0, text);
        };
    };

    private static object? GetItemValue(string fieldName, TItem item)
    {
        object? ret = null;
        if (item != null)
        {
            if (item is IDynamicObject dynamicObject)
            {
                ret = dynamicObject.GetValue(fieldName);
            }
            else
            {
                ret = item.ValueOf(fieldName);

                if (ret != null)
                {
                    var t = ret.GetType();
                    if (t.IsEnum)
                    {
                        var itemName = ret.ToString();
                        if (!string.IsNullOrEmpty(itemName))
                        {
                            ret = Utility.GetDisplayName(t, itemName);
                        }
                    }
                }
            }
        }
        return ret;
    }

    protected RenderFragment RenderCell(ITableColumn col, TItem item, ItemChangedType changedType) => col.CanWrite(typeof(TItem), changedType)
        ? (col.EditTemplate == null
            ? builder => builder.CreateComponentByFieldType(this, col, item, changedType, false, LookupService)
            : col.EditTemplate(item))
        : (col.Template == null
            ? builder => builder.CreateDisplayByFieldType(col, item)
            : col.Template(item));

    protected RenderFragment RenderExcelCell(ITableColumn col, TItem item, ItemChangedType changedType)
    {
        col.PlaceHolder ??= "";
        col.IsPopover = true;

        if (col.IsEditable(changedType) && col.EditTemplate == null)
        {
            if (DynamicContext != null)
            {
                SetDynamicEditTemplate();
            }
            else
            {
                SetEditTemplate();
            }
        }
        return RenderCell(col, item, changedType);

        void SetDynamicEditTemplate()
        {
            col.EditTemplate = row => builder =>
            {
                var d = (IDynamicObject)row;
                var onValueChanged = Utility.GetOnValueChangedInvoke<IDynamicObject>(col.PropertyType);
                if (DynamicContext.OnValueChanged != null)
                {
                    var parameters = col.ComponentParameters?.ToList() ?? new List<KeyValuePair<string, object>>();
                    parameters.Add(new(nameof(ValidateBase<string>.OnValueChanged), onValueChanged.Invoke(d, col, (model, column, val) => DynamicContext.OnValueChanged(model, column, val))));
                    col.ComponentParameters = parameters;
                }
                builder.CreateComponentByFieldType(this, col, row, changedType, false, LookupService);
            };
        }

        void SetEditTemplate()
        {
            var onValueChanged = Utility.GetOnValueChangedInvoke<TItem>(col.PropertyType);
            col.ComponentParameters = new List<KeyValuePair<string, object>>
            {
                new(nameof(ValidateBase<string>.OnValueChanged), onValueChanged(item, col, (model, column, val) => InternalOnSaveAsync(model, ItemChangedType.Update)))
            };
        }
    }

    #region Filter
    [NotNull]
    public Func<Task>? OnFilterAsync { get; private set; }

    public Dictionary<string, IFilterAction> Filters { get; } = new();
    #endregion

    private async ValueTask<ItemsProviderResult<TItem>> LoadItems(ItemsProviderRequest request)
    {
        StartIndex = request.StartIndex;
        if (TotalCount > 0)
        {
            PageItems = Math.Min(request.Count, TotalCount - request.StartIndex);
        }
        await QueryData();
        return new ItemsProviderResult<TItem>(QueryItems, TotalCount);
    }

    private Func<Task> TriggerDoubleClickCell(ITableColumn col, TItem item) => async () =>
    {
        if (OnDoubleClickCellCallback != null)
        {
            var val = GetItemValue(col.GetFieldName(), item);
            await OnDoubleClickCellCallback(col.GetFieldName(), item, val);
        }
    };

    private static string? GetDoubleClickCellClassString(bool trigger) => CssBuilder.Default()
        .AddClass("is-dbcell", trigger)
        .Build();

    private bool IsShowEmpty => ShowEmpty && !Rows.Any();

    private int GetColumnCount()
    {
        var colspan = GetVisibleColumns().Count(col => col.Visible);
        if (IsMultipleSelect)
        {
            colspan++;
        }

        if (ShowLineNo)
        {
            colspan++;
        }

        if (ShowExtendButtons)
        {
            colspan++;
        }
        return colspan;
    }

    private int GetEmptyColumnCount() => ShowDetails() ? GetColumnCount() + 1 : GetColumnCount();

    private bool GetShowHeader()
    {
        var ret = true;
        if (MultiHeaderTemplate != null)
        {
            ret = ShowMultiFilterHeader;
        }
        return ret;
    }

    public async Task ResetFilters()
    {
        foreach (var column in Columns)
        {
            column.Filter?.FilterAction.Reset();
        }
        Filters.Clear();
        await OnFilterAsync();
    }

    private bool GetEditButtonStatus() => ShowAddForm || AddInCell || SelectedRows.Count != 1;

    private bool GetDeleteButtonStatus() => ShowAddForm || AddInCell || !SelectedRows.Any();

    private async Task InvokeItemsChanged()
    {
        if (ItemsChanged.HasDelegate)
        {
            await ItemsChanged.InvokeAsync(Rows);
        }
    }

    protected override async ValueTask DisposeAsync(bool disposing)
    {
        if (disposing)
        {
            AutoRefreshCancelTokenSource?.Cancel();
            AutoRefreshCancelTokenSource?.Dispose();
            AutoRefreshCancelTokenSource = null;

            if (Interop != null)
            {
                Interop.Dispose();
                Interop = null;
            }
        }

        await base.DisposeAsync(disposing);
    }
}
