namespace Undersoft.SDK.Service.Application.Components;

[AttributeUsage(AttributeTargets.Property)]
public class AutoGenerateColumnAttribute : AutoGenerateBaseAttribute, ITableColumn
{
    public int Order { get; set; }

    public bool Ignore { get; set; }

    public bool DefaultSort { get; set; }

    public bool SkipValidate { get; set; }

    public bool IsReadonlyWhenAdd { get; set; }

    public bool IsReadonlyWhenEdit { get; set; }

    public bool ShowLabelTooltip { get; set; }

    bool? IEditorItem.ShowLabelTooltip
    {
        get => ShowLabelTooltip;
        set => ShowLabelTooltip = value.HasValue && value.Value;
    }

    public SortOrder DefaultSortOrder { get; set; }

    IEnumerable<SelectedItem>? IEditorItem.Items { get; set; }

    public int Width { get; set; }

    int? ITableColumn.Width
    {
        get => Width <= 0 ? null : Width;
        set => Width = value == null ? 0 : Width;
    }

    public bool Fixed { get; set; }

    public bool Visible { get; set; } = true;

    public string? CssClass { get; set; }

    public BreakPoint ShownWithBreakPoint { get; set; }

    public string? FormatString { get; set; }

    public string? PlaceHolder { get; set; }

    public Func<object?, Task<string>>? Formatter { get; set; }

    RenderFragment<object>? IEditorItem.EditTemplate { get; set; }

    RenderFragment<ITableColumn>? ITableColumn.HeaderTemplate { get; set; }

    public Type? ComponentType { get; set; }

    IEnumerable<KeyValuePair<string, object>>? IEditorItem.ComponentParameters { get; set; }

    RenderFragment<object>? ITableColumn.Template { get; set; }

    RenderFragment<object>? ITableColumn.SearchTemplate { get; set; }

    RenderFragment? ITableColumn.FilterTemplate { get; set; }

    public object? Step { get; set; }

    public int Rows { get; set; }

    IFilter? ITableColumn.Filter { get; set; }

    [NotNull]
    public Type? PropertyType { get; internal set; }

    public string? Text { get; set; }

    [NotNull]
    internal string? FieldName { get; set; }

    IEnumerable<SelectedItem>? IEditorItem.Lookup { get; set; }

    public bool ShowSearchWhenSelect { get; set; }

    public bool IsPopover { get; set; }

    public StringComparison LookupStringComparison { get; set; } = StringComparison.OrdinalIgnoreCase;

    public string? LookupServiceKey { get; set; }

    Action<TableCellArgs>? ITableColumn.OnCellRender { get; set; }

    List<IValidator>? IEditorItem.ValidateRules { get; set; }

    public string GetDisplayName() => Text ?? "";

    public string GetFieldName() => FieldName;

    public string? GroupName { get; set; }

    public int GroupOrder { get; set; }

    public bool HeaderTextWrap { get; set; }

    public bool ShowHeaderTooltip { get; set; }

    public string? HeaderTextTooltip { get; set; }

    public bool HeaderTextEllipsis { get; set; }
}
