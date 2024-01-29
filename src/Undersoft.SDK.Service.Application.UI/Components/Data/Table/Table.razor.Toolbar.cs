using Microsoft.AspNetCore.Components.Forms;

namespace Undersoft.SDK.Service.Application.Components;

public partial class Table<TItem>
{
    [Parameter]
    public bool ShowToolbar { get; set; }

    [Parameter]
    public bool ShowSkeleton { get; set; }

    [Parameter]
    public bool ShowLoadingInFirstRender { get; set; } = true;

    [Parameter]
    public bool ShowDefaultButtons { get; set; } = true;

    [Parameter]
    public bool ShowAddButton { get; set; } = true;

    [Parameter]
    public bool ShowEditButton { get; set; } = true;

    [Parameter]
    public Func<TItem, bool>? ShowEditButtonCallback { get; set; }

    [Parameter]
    public bool ShowDeleteButton { get; set; } = true;

    [Parameter]
    public Func<TItem, bool>? ShowDeleteButtonCallback { get; set; }

    [Parameter]
    public bool ShowExportButton { get; set; }

    [Parameter]
    public string? ExportButtonIcon { get; set; }

    [Parameter]
    public string? ExcelExportIcon { get; set; }

    [Parameter]
    public bool ShowToastAfterExport { get; set; } = true;

    [Parameter]
    public RenderFragment<ITableExportContext<TItem>>? ExportButtonDropdownTemplate { get; set; }

    [Parameter]
    public string? ExportExcelDropdownItemText { get; set; }

    [Parameter]
    public bool ShowExtendButtons { get; set; }

    [Parameter]
    public bool IsAutoCollapsedToolbarButton { get; set; } = true;

    [Parameter]
    public string? GearIcon { get; set; }

    [Parameter]
    public bool IsExtendButtonsInRowHeader { get; set; }

    [Parameter]
    public int ExtendButtonColumnWidth { get; set; } = 130;

    [Parameter]
    public Alignment ExtendButtonColumnAlignment { get; set; }

    [Parameter]
    public bool ShowExtendEditButton { get; set; } = true;

    [Parameter]
    public bool ShowExtendDeleteButton { get; set; } = true;

    [Parameter]
    public bool FixedExtendButtonsColumn { get; set; }

    [Parameter]
    public bool FixedMultipleColumn { get; set; }

    [Parameter]
    public bool FixedDetailRowHeaderColumn { get; set; }

    [Parameter]
    public bool FixedLineNoColumn { get; set; }

    [Parameter]
    public bool ShowRefresh { get; set; } = true;

    [Parameter]
    public bool ShowCardView { get; set; }

    [Parameter]
    public bool ShowColumnList { get; set; }

    [Parameter]
    public string? ColumnListButtonIcon { get; set; }

    [Parameter]
    public bool ShowToastAfterSaveOrDeleteModel { get; set; } = true;

    [Parameter]
    [Obsolete("已过期，请使用 TableToolbarBeforeTemplate 或者 TableToolbarAfterTemplate 参数代替")]
    [ExcludeFromCodeCoverage]
    public RenderFragment? TableToolbarTemplate { get { return TableToolbarAfterTemplate; } set { TableToolbarAfterTemplate = value; } }

    [Parameter]
    public RenderFragment? TableToolbarBeforeTemplate { get; set; }

    [Parameter]
    public RenderFragment? TableToolbarAfterTemplate { get; set; }

    [Parameter]
    public RenderFragment? TableExtensionToolbarBeforeTemplate { get; set; }

    [Parameter]
    public RenderFragment? TableExtensionToolbarAfterTemplate { get; set; }

    [Parameter]
    public Func<Task<TItem>>? OnAddAsync { get; set; }

    [Parameter]
    public Func<TItem, Task>? OnEditAsync { get; set; }

    [Parameter]
    public Func<TItem, ItemChangedType, Task<bool>>? OnSaveAsync { get; set; }

    [Parameter]
    public Func<IEnumerable<TItem>, Task<bool>>? OnDeleteAsync { get; set; }

    [Parameter]
    public Func<IEnumerable<TItem>, QueryPageOptions, Task<bool>>? OnExportAsync { get; set; }

    [Parameter]
    public string? EditDialogSaveButtonText { get; set; }

    [Parameter]
    public string? EditDialogCloseButtonText { get; set; }

    [Parameter]
    [NotNull]
    public string? ExportToastTitle { get; set; }

    [Parameter]
    [NotNull]
    public string? ExportToastContent { get; set; }

    [Parameter]
    [NotNull]
    public string? ExportToastInProgressContent { get; set; }

    [Inject]
    [NotNull]
    protected ToastService? Toast { get; set; }

    [Inject]
    [NotNull]
    protected DialogService? DialogService { get; set; }

    [Inject]
    [NotNull]
    private ITableExcelExport? ExcelExport { get; set; }

    private List<ColumnVisibleItem> VisibleColumns { get; } = new();

    public IEnumerable<ITableColumn> GetVisibleColumns()
    {
        var items = VisibleColumns.Where(i => i.Visible);
        return Columns.Where(i => items.Any(v => v.Name == i.GetFieldName()));
    }

    private bool GetColumnsListState(ITableColumn col) => VisibleColumns.First(i => i.Name == col.GetFieldName()).Visible && VisibleColumns.Count(i => i.Visible) == 1;

    private bool ShowAddForm { get; set; }

    private bool EditInCell { get; set; }

    private bool AddInCell { get; set; }

    public async Task AddAsync()
    {
        if (IsExcel || DynamicContext != null)
        {
            await AddDynamicOjbectExcelModelAsync();
        }
        else
        {
            await AddItemAsync();
        }

        async Task AddItemAsync()
        {
            await ToggleLoading(true);
            await InternalOnAddAsync();
            EditModalTitleString = AddModalTitle;
            if (EditMode == EditMode.Popup)
            {
                await ShowEditDialog(ItemChangedType.Add);
            }
            else if (EditMode == EditMode.EditForm)
            {
                ShowAddForm = true;
                ShowEditForm = false;
            }
            else if (EditMode == EditMode.InCell)
            {
                AddInCell = true;
                EditInCell = true;
                SelectedRows.Add(EditModel);
            }
            await OnSelectedRowsChanged();
            await ToggleLoading(false);
        }

        async Task AddDynamicOjbectExcelModelAsync()
        {
            if (DynamicContext != null)
            {
                await DynamicContext.AddAsync(SelectedRows.OfType<IDynamicObject>());
                ResetDynamicContext();
                SelectedRows.Clear();
                await OnSelectedRowsChanged();
            }
            else
            {
                await InternalOnAddAsync();
                await QueryAsync(false);
                await OnSelectedRowsChanged();
            }
        }
    }

    private bool ShowEditForm { get; set; }

    public async Task EditAsync()
    {
        if (SelectedRows.Count == 1)
        {
            if (ShowEditButtonCallback != null && !ShowEditButtonCallback(SelectedRows[0]))
            {
                await ShowToastAsync(EditButtonToastReadonlyContent);
            }
            else
            {
                await ToggleLoading(true);
                await InternalOnEditAsync();
                EditModalTitleString = EditModalTitle;

                if (EditMode == EditMode.Popup)
                {
                    await ShowEditDialog(ItemChangedType.Update);
                }
                else if (EditMode == EditMode.EditForm)
                {
                    ShowEditForm = true;
                    ShowAddForm = false;
                    StateHasChanged();

                }
                else if (EditMode == EditMode.InCell)
                {
                    AddInCell = false;
                    EditInCell = true;
                    StateHasChanged();
                }
                await ToggleLoading(false);
            }
        }
        else
        {
            var content = SelectedRows.Count == 0 ? EditButtonToastNotSelectContent : EditButtonToastMoreSelectContent;
            await ShowToastAsync(content);
        }

        async Task InternalOnEditAsync()
        {
            EditModel = IsTracking ? SelectedRows[0] : Utility.Clone(SelectedRows[0]);
            if (OnEditAsync != null)
            {
                await OnEditAsync(EditModel);
            }
            else
            {
                var d = DataService ?? InjectDataService;
                if (d is IEntityFrameworkCoreDataService ef)
                {
                    await ef.EditAsync(EditModel);
                }
            }
        }

        async Task ShowToastAsync(string content)
        {
            var option = new ToastOption
            {
                Category = ToastCategory.Information,
                Title = EditButtonToastTitle,
                Content = content
            };
            await Toast.Show(option);
        }
    }

    protected void CancelSave()
    {
        if (EditMode == EditMode.EditForm)
        {
            ShowAddForm = false;
            ShowEditForm = false;
        }
        else if (EditMode == EditMode.InCell)
        {
            SelectedRows.Clear();
            AddInCell = false;
            EditInCell = false;
        }
    }

    protected async Task<bool> SaveModelAsync(EditContext context, ItemChangedType changedType)
    {
        bool valid;
        if (DynamicContext != null)
        {
            await DynamicContext.SetValue(context.Model);
            RowsCache = null;
            valid = true;
        }
        else
        {
            valid = await InternalOnSaveAsync((TItem)context.Model, changedType);
        }

        if (OnAfterSaveAsync != null)
        {
            await OnAfterSaveAsync((TItem)context.Model);
        }
        if (ShowToastAfterSaveOrDeleteModel)
        {
            var option = new ToastOption
            {
                Category = valid ? ToastCategory.Success : ToastCategory.Error,
                Title = SaveButtonToastTitle
            };
            option.Content = string.Format(SaveButtonToastResultContent, valid ? SuccessText : FailText, Math.Ceiling(option.Delay / 1000.0));
            await Toast.Show(option);
        }
        return valid;
    }

    protected async Task SaveAsync(EditContext context, ItemChangedType changedType)
    {
        await ToggleLoading(true);
        if (await SaveModelAsync(context, changedType))
        {
            if (EditMode == EditMode.EditForm)
            {
                ShowEditForm = false;
                if (ShowAddForm)
                {
                    ShowAddForm = false;
                    if (IsTracking)
                    {
                        var index = InsertRowMode == InsertRowMode.First ? 0 : Rows.Count;
                        Rows.Insert(index, EditModel);
                        await InvokeItemsChanged();
                    }
                    else
                    {
                        await QueryData();
                    }
                }
                else
                {
                    StateHasChanged();
                }
            }
            else if (EditMode == EditMode.InCell)
            {
                EditInCell = false;
                AddInCell = false;

                if (changedType == ItemChangedType.Add)
                {
                    var index = InsertRowMode == InsertRowMode.First ? 0 : Rows.Count;
                    Rows.Insert(index, EditModel);
                }
                else
                {
                    var index = Rows.IndexOf(SelectedRows[0]);
                    Rows.RemoveAt(index);
                    Rows.Insert(index, EditModel);
                }
                SelectedRows.Clear();
                if (ItemsChanged.HasDelegate)
                {
                    await ItemsChanged.InvokeAsync(Rows);
                }
                else
                {
                    await QueryAsync();
                }
            }
        }
        await ToggleLoading(false);
    }

    [Parameter]
    public Size EditDialogSize { get; set; } = Size.ExtraExtraLarge;

    [Parameter]
    public bool EditDialogIsDraggable { get; set; }

    [Parameter]
    public FullScreenSize EditDialogFullScreenSize { get; set; }

    [Parameter]
    public bool EditDialogShowMaximizeButton { get; set; } = true;

    [Parameter]
    public bool ShowUnsetGroupItemsOnTop { get; set; }

    [Parameter]
    public RenderFragment<TItem>? EditFooterTemplate { get; set; }

    [Parameter]
    public Func<TItem, bool, Task>? EditDialogCloseAsync { get; set; }

    protected async Task ShowEditDialog(ItemChangedType changedType)
    {
        var saved = false;
        var option = new EditDialogOption<TItem>()
        {
            Class = "modal-dialog-table",
            IsScrolling = ScrollingDialogContent,
            IsKeyboard = IsKeyboard,
            ShowLoading = ShowLoading,
            Title = EditModalTitleString,
            Model = EditModel,
            Items = Columns.Where(i => i.Editable),
            SaveButtonText = EditDialogSaveButtonText,
            CloseButtonText = EditDialogCloseButtonText,
            DialogBodyTemplate = EditTemplate,
            RowType = EditDialogRowType,
            ItemsPerRow = EditDialogItemsPerRow,
            LabelAlign = EditDialogLabelAlign,
            ItemChangedType = changedType,
            Size = EditDialogSize,
            IsDraggable = EditDialogIsDraggable,
            ShowMaximizeButton = EditDialogShowMaximizeButton,
            FullScreenSize = EditDialogFullScreenSize,
            ShowUnsetGroupItemsOnTop = ShowUnsetGroupItemsOnTop,
            DisableAutoSubmitFormByEnter = DisableAutoSubmitFormByEnter,
            IsTracking = IsTracking,
            DialogFooterTemplate = EditFooterTemplate,
            OnCloseAsync = async () =>
            {
                if (EditDialogCloseAsync != null)
                {
                    await EditDialogCloseAsync(EditModel, saved);
                }
                if (!saved)
                {
                    var d = DataService ?? InjectDataService;
                    if (d is IEntityFrameworkCoreDataService ef)
                    {
                        await ToggleLoading(true);
                        await ef.CancelAsync();
                        await ToggleLoading(false);
                    }
                }
            },
            OnEditAsync = async context =>
            {
                await ToggleLoading(true);
                if (IsTracking)
                {
                    saved = true;
                    if (changedType == ItemChangedType.Add)
                    {
                        var index = InsertRowMode == InsertRowMode.First ? 0 : Rows.Count;
                        Rows.Insert(index, EditModel);
                    }
                    await InvokeItemsChanged();
                }
                else
                {
                    saved = await SaveModelAsync(context, changedType);
                    if (saved)
                    {
                        if (Items != null)
                        {
                            if (changedType == ItemChangedType.Add)
                            {
                                await AddItem();
                            }
                            else if (changedType == ItemChangedType.Update)
                            {
                                await EditItem();
                            }
                        }
                        else
                        {
                            await QueryAsync();
                        }
                    }
                }
                await ToggleLoading(false);
                return saved;

                async Task AddItem()
                {
                    var index = InsertRowMode == InsertRowMode.First ? 0 : Rows.Count;
                    Rows.Insert(index, (TItem)context.Model);
                    await UpdateRow();
                }

                async Task EditItem()
                {
                    var entity = Rows.FirstOrDefault(i => this.Equals<TItem>(i, (TItem)context.Model));
                    if (entity != null)
                    {
                        var index = Rows.IndexOf(entity);
                        Rows.RemoveAt(index);
                        Rows.Insert(index, (TItem)context.Model);
                        await UpdateRow();
                    }
                }

                async Task UpdateRow()
                {
                    if (ItemsChanged.HasDelegate)
                    {
                        await InvokeItemsChanged();
                    }
                    else
                    {
                        Items = Rows;
                    }
                }
            }
        };
        await DialogService.ShowEditDialog(option);
    }

    protected async Task<bool> ConfirmDelete()
    {
        var ret = false;
        if (SelectedRows.Count == 0)
        {
            await ShowToastAsync(DeleteButtonToastContent);
        }
        else
        {
            if (ShowDeleteButtonCallback != null && SelectedRows.Any(i => !ShowDeleteButtonCallback(i)))
            {
                await ShowToastAsync(DeleteButtonToastCanNotDeleteContent);
            }
            else
            {
                ret = true;
            }
        }
        return ret;

        async Task ShowToastAsync(string content)
        {
            var option = new ToastOption
            {
                Category = ToastCategory.Information,
                Title = DeleteButtonToastTitle
            };
            option.Content = string.Format(content, Math.Ceiling(option.Delay / 1000.0));
            await Toast.Show(option);
        }
    }

    protected async Task DeleteAsync()
    {
        if (IsExcel || DynamicContext != null)
        {
            await DeleteDynamicObjectExcelModelAsync();
        }
        else
        {
            await ToggleLoading(true);
            var ret = await DelteItemsAsync();

            if (ShowToastAfterSaveOrDeleteModel)
            {
                var option = new ToastOption()
                {
                    Title = DeleteButtonToastTitle,
                    Category = ret ? ToastCategory.Success : ToastCategory.Error
                };
                option.Content = string.Format(DeleteButtonToastResultContent, ret ? SuccessText : FailText, Math.Ceiling(option.Delay / 1000.0));
                await Toast.Show(option);
            }
            await ToggleLoading(false);
        }

        async Task<bool> DelteItemsAsync()
        {
            var ret = await InternalOnDeleteAsync();
            if (ret)
            {
                if (Items != null)
                {
                    SelectedRows.ForEach(i => Rows.Remove(i));
                    if (ItemsChanged.HasDelegate)
                    {
                        await InvokeItemsChanged();
                    }
                }
                else
                {
                    if (IsPagination)
                    {
                        PageIndex = Math.Max(1, Math.Min(PageIndex, int.Parse(Math.Ceiling((TotalCount - SelectedRows.Count) * 1d / PageItems).ToString())));
                        var items = PageItemsSource.Where(item => item >= (TotalCount - SelectedRows.Count));
                        if (items.Any())
                        {
                            PageItems = Math.Min(PageItems, items.Min());
                        }
                    }
                }
                SelectedRows.Clear();
                await QueryAsync();
            }
            return ret;
        }

        async Task DeleteDynamicObjectExcelModelAsync()
        {
            if (DynamicContext != null)
            {
                await DynamicContext.DeleteAsync(SelectedRows.AsEnumerable().OfType<IDynamicObject>());
                ResetDynamicContext();
                SelectedRows.Clear();
                await OnSelectedRowsChanged();
            }
            else
            {
                await InternalOnDeleteAsync();
                await QueryAsync();
            }
        }
    }

    private void ResetDynamicContext()
    {
        if (DynamicContext != null && typeof(TItem).IsAssignableTo(typeof(IDynamicObject)))
        {
            AutoGenerateColumns = false;

            var cols = DynamicContext.GetColumns();
            Columns.Clear();
            Columns.AddRange(cols);

            FirstFixedColumnCache.Clear();
            LastFixedColumnCache.Clear();

            InternalResetVisibleColumns(Columns.Select(i => new ColumnVisibleItem(i.GetFieldName(), i.Visible)));

            QueryDynamicItems(DynamicContext);
        }
    }

    private void QueryDynamicItems(IDynamicObjectContext context)
    {
        QueryItems = context.GetItems().Cast<TItem>();
        TotalCount = QueryItems.Count();
        RowsCache = null;

        ResetSelectedRows(QueryItems);
    }

    protected async Task ExportAsync()
    {
        var option = new ToastOption
        {
            Title = ExportToastTitle,
            Category = ToastCategory.Information
        };
        option.Content = string.Format(ExportToastInProgressContent, Math.Ceiling(option.Delay / 1000.0));
        await Toast.Show(option);

        var ret = false;
        if (OnExportAsync != null)
        {
            ret = await OnExportAsync(Rows, BuildQueryPageOptions());
        }
        else
        {
            ret = await ExcelExport.ExportAsync(Rows, GetVisibleColumns());
        }

        if (ShowToastAfterExport)
        {
            option = new ToastOption
            {
                Title = ExportToastTitle,
                Category = ret ? ToastCategory.Success : ToastCategory.Error
            };
            option.Content = string.Format(ExportToastContent, ret ? SuccessText : FailText, Math.Ceiling(option.Delay / 1000.0));
            await Toast.Show(option);
        }
    }

    protected IEnumerable<TItem> GetSelectedRows() => SelectedRows;

    protected bool GetShowEditButton(TItem item) => ShowEditButtonCallback?.Invoke(item) ?? ShowExtendEditButton;

    protected bool GetShowDeleteButton(TItem item) => ShowDeleteButtonCallback?.Invoke(item) ?? ShowExtendDeleteButton;
}
