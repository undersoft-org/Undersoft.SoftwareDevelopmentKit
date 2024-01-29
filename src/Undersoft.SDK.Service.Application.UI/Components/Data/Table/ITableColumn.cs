namespace Undersoft.SDK.Service.Application.Components;

public interface ITableColumn : IEditorItem
{
    bool Sortable { get; set; }

    bool DefaultSort { get; set; }

    SortOrder DefaultSortOrder { get; set; }

    bool Filterable { get; set; }

    bool Searchable { get; set; }

    int? Width { get; set; }

    bool Fixed { get; set; }

    bool Visible { get; set; }

    bool TextWrap { get; set; }

    bool TextEllipsis { get; set; }

    bool HeaderTextWrap { get; set; }

    bool ShowHeaderTooltip { get; set; }

    string? HeaderTextTooltip { get; set; }

    bool HeaderTextEllipsis { get; set; }

    string? CssClass { get; set; }

    BreakPoint ShownWithBreakPoint { get; set; }

    bool ShowCopyColumn { get; set; }

    RenderFragment<object>? Template { get; set; }

    RenderFragment<object>? SearchTemplate { get; set; }

    RenderFragment? FilterTemplate { get; set; }

    RenderFragment<ITableColumn>? HeaderTemplate { get; set; }

    IFilter? Filter { get; set; }

    string? FormatString { get; set; }

    Func<object?, Task<string>>? Formatter { get; set; }

    Alignment Align { get; set; }

    bool ShowTips { get; set; }

    Action<TableCellArgs>? OnCellRender { get; set; }
}
