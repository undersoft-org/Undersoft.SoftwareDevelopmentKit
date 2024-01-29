namespace Undersoft.SDK.Service.Application.Components;

internal class InternalTableColumn : ITableColumn
{
    private string FieldName { get; }

    public bool Sortable { get; set; }

    public bool DefaultSort { get; set; }

    public SortOrder DefaultSortOrder { get; set; }

    public bool Filterable { get; set; }

    public bool Searchable { get; set; }

    public int? Width { get; set; }

    public bool Fixed { get; set; }

    public bool Visible { get; set; } = true;

    public bool TextWrap { get; set; }

    public bool TextEllipsis { get; set; }

    public bool SkipValidate { get; set; }

    public bool IsReadonlyWhenAdd { get; set; }

    public bool IsReadonlyWhenEdit { get; set; }

    public bool? ShowLabelTooltip { get; set; }

    public string? CssClass { get; set; }

    public BreakPoint ShownWithBreakPoint { get; set; }

    public RenderFragment<object>? Template { get; set; }

    public RenderFragment<object>? SearchTemplate { get; set; }

    public RenderFragment? FilterTemplate { get; set; }

    public RenderFragment<ITableColumn>? HeaderTemplate { get; set; }

    public IFilter? Filter { get; set; }

    public string? FormatString { get; set; }

    public string? PlaceHolder { get; set; }

    public Func<object?, Task<string>>? Formatter { get; set; }

    public Alignment Align { get; set; }

    public bool ShowTips { get; set; }

    public Type PropertyType { get; }

    public bool Editable { get; set; } = true;

    public bool Readonly { get; set; }

    public object? Step { get; set; }

    public int Rows { get; set; }

    [NotNull]
    public string? Text { get; set; }

    public RenderFragment<object>? EditTemplate { get; set; }

    public Type? ComponentType { get; set; }

    public IEnumerable<KeyValuePair<string, object>>? ComponentParameters { get; set; }

    public IEnumerable<SelectedItem>? Items { get; set; }

    public int Order { get; set; }

    public IEnumerable<SelectedItem>? Lookup { get; set; }

    public bool ShowSearchWhenSelect { get; set; }

    public bool IsPopover { get; set; }

    public StringComparison LookupStringComparison { get; set; } = StringComparison.OrdinalIgnoreCase;

    public string? LookupServiceKey { get; set; }

    public Action<TableCellArgs>? OnCellRender { get; set; }

    public List<IValidator>? ValidateRules { get; set; }

    public string? GroupName { get; set; }

    public int GroupOrder { get; set; }

    public bool ShowCopyColumn { get; set; }

    public bool HeaderTextWrap { get; set; }

    public bool ShowHeaderTooltip { get; set; }

    public string? HeaderTextTooltip { get; set; }

    public bool HeaderTextEllipsis { get; set; }

    public InternalTableColumn(string fieldName, Type fieldType, string? fieldText = null)
    {
        FieldName = fieldName;
        PropertyType = fieldType;
        Text = fieldText;
    }

    public string GetDisplayName() => Text;

    public string GetFieldName() => FieldName;
}
