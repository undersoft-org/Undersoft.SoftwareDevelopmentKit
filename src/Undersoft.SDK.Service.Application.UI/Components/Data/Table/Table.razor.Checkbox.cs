namespace Undersoft.SDK.Service.Application.Components;

public partial class Table<TItem>
{
    protected string? CheckboxDisplayTextString => ShowCheckboxText ? CheckboxDisplayText : null;

    protected string? HeaderClass => CssBuilder.Default()
        .AddClass(HeaderStyle.ToDescriptionString(), HeaderStyle != TableHeaderStyle.None)
        .Build();

    protected CheckboxState HeaderCheckState()
    {
        var ret = CheckboxState.UnChecked;
        var filterRows = ShowRowCheckboxCallback == null ? Rows : Rows.Where(ShowRowCheckboxCallback);
        if (filterRows.Any())
        {
            if (filterRows.All(AnyRow))
            {
                ret = CheckboxState.Checked;
            }
            else if (filterRows.Any(AnyRow))
            {
                ret = CheckboxState.Indeterminate;
            }
        }
        return ret;

        bool AnyRow(TItem row) => SelectedRows.Any(i => Equals(i, row));
    }

    protected CheckboxState RowCheckState(TItem item) => SelectedRows.Any(i => Equals(i, item)) ? CheckboxState.Checked : CheckboxState.UnChecked;

    [Parameter]
    public bool IsMultipleSelect { get; set; }

    [Parameter]
    public bool ShowCheckboxText { get; set; }

    [Parameter]
    [NotNull]
    public string? CheckboxDisplayText { get; set; }

    [Parameter]
    public Func<TItem, bool>? ShowRowCheckboxCallback { get; set; }

    private bool GetShowRowCheckbox(TItem item) => ShowRowCheckboxCallback == null || ShowRowCheckboxCallback(item);

    protected virtual async Task OnHeaderCheck(CheckboxState state, TItem val)
    {
        switch (state)
        {
            case CheckboxState.Checked:
                SelectedRows.Clear();
                SelectedRows.AddRange(ShowRowCheckboxCallback == null ? Rows : Rows.Where(ShowRowCheckboxCallback));
                await OnSelectedRowsChanged();
                break;
            case CheckboxState.UnChecked:
            default:
                SelectedRows.Clear();
                await OnSelectedRowsChanged();
                break;
        }
    }

    protected async Task OnCheck(CheckboxState state, TItem val)
    {
        if (state == CheckboxState.Checked)
        {
            SelectedRows.Add(val);
        }
        else
        {
            var item = SelectedRows.FirstOrDefault(i => Equals(i, val));
            if (item != null)
            {
                SelectedRows.Remove(item);
            }
        }

        AddInCell = false;
        EditInCell = false;

        await OnSelectedRowsChanged();
    }

    [Parameter]
    public Func<string, bool, Task>? OnColumnVisibleChanged { get; set; }

    private async Task OnToggleColumnVisible(string columnName, bool visible)
    {
        if (OnColumnVisibleChanged != null)
        {
            await OnColumnVisibleChanged(columnName, visible);
        }
    }
}
