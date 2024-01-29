using Microsoft.AspNetCore.Components.Forms;
using System.Linq.Expressions;

namespace Undersoft.SDK.Service.Application.Components;

public class TableColumn<TItem, TType> : Component, ITableColumn
{
    public IFilter? Filter { get; set; }

    [Parameter]
    public Type? ComponentType { get; set; }

    [Parameter]
    public IEnumerable<KeyValuePair<string, object>>? ComponentParameters { get; set; }

    [NotNull]
    public Type? PropertyType { get; set; }

    [Parameter]
    [MaybeNull]
    public TType Field { get; set; }

    [Parameter]
    public Expression<Func<TType>>? FieldExpression { get; set; }

    [Parameter]
    public bool Sortable { get; set; }

    [Parameter]
    public bool DefaultSort { get; set; }

    [Parameter]
    public bool TextWrap { get; set; }

    [Parameter]
    public bool TextEllipsis { get; set; }

    [Parameter]
    public bool? ShowLabelTooltip { get; set; }

    [Parameter]
    public bool HeaderTextWrap { get; set; }

    [Parameter]
    public bool ShowHeaderTooltip { get; set; }

    [Parameter]
    public string? HeaderTextTooltip { get; set; }

    [Parameter]
    public bool HeaderTextEllipsis { get; set; }

    [Parameter]
    public object? Step { get; set; }

    [Parameter]
    public int Rows { get; set; }

    [Parameter]
    public SortOrder DefaultSortOrder { get; set; }

    [Parameter]
    public bool Filterable { get; set; }

    [Parameter]
    public bool Searchable { get; set; }

    [Parameter]
    public bool Editable { get; set; } = true;

    [Parameter]
    public bool Readonly { get; set; }

    [Parameter]
    public bool IsReadonlyWhenAdd { get; set; }

    [Parameter]
    public bool IsReadonlyWhenEdit { get; set; }

    [Parameter]
    public bool SkipValidate { get; set; }

    [Parameter]
    public string? Text { get; set; }

    [Parameter]
    public string? PlaceHolder { get; set; }

    [Parameter]
    public int? Width { get; set; }

    [Parameter]
    public bool Fixed { get; set; }

    [Parameter]
    public bool ShowCopyColumn { get; set; }

    [Parameter]
    public bool Visible { get; set; } = true;

    [Parameter]
    public bool ShowTips { get; set; }

    [Parameter]
    public string? CssClass { get; set; }

    [Parameter]
    public Alignment Align { get; set; }

    [Parameter]
    public string? FormatString { get; set; }

    [Parameter]
    public Func<object?, Task<string>>? Formatter { get; set; }

    [Parameter]
#if NET5_0
    public RenderFragment<TableColumnContext<object, TType>>? Template { get; set; }

    /// <summary>
    /// 内部使用负责把 object 类型的绑定数据值转化为泛型数据传递给前端
    /// </summary>
    RenderFragment<object>? ITableColumn.Template
    {
        get => Template == null ? null : new RenderFragment<object>(context => builder =>
        {
            // 此处 context 为行数据
            var fieldName = GetFieldName();
            var value = Utility.GetPropertyValue<object, TType>(context, fieldName);
            builder.AddContent(0, Template.Invoke(new TableColumnContext<object, TType>(context, value)));
        });
        set
        {

        }
    }
#elif NET6_0_OR_GREATER
    public RenderFragment<TableColumnContext<TItem, TType>>? Template { get; set; }

    RenderFragment<object>? ITableColumn.Template
    {
        get => Template == null ? null : new RenderFragment<object>(context => builder =>
        {
            var fieldName = GetFieldName();
            var value = Utility.GetPropertyValue<object, TType>(context, fieldName);
            builder.AddContent(0, Template.Invoke(new TableColumnContext<TItem, TType>((TItem)context, value)));
        });
        set
        {

        }
    }
#endif

    [Parameter]
#if NET5_0
    public RenderFragment<object>? EditTemplate { get; set; }
#elif NET6_0_OR_GREATER
    public RenderFragment<TItem>? EditTemplate { get; set; }

    RenderFragment<object>? IEditorItem.EditTemplate
    {
        get
        {
            return EditTemplate == null ? null : new RenderFragment<object>(item => builder =>
            {
                builder.AddContent(0, EditTemplate((TItem)item));
            });
        }
        set
        {
        }
    }
#endif

    [Parameter]
#if NET5_0
    public RenderFragment<object>? SearchTemplate { get; set; }
#elif NET6_0_OR_GREATER
    public RenderFragment<TItem>? SearchTemplate { get; set; }

    RenderFragment<object>? ITableColumn.SearchTemplate
    {
        get
        {
            return SearchTemplate == null ? null : new RenderFragment<object>(item => builder =>
            {
                builder.AddContent(0, SearchTemplate((TItem)item));
            });
        }
        set
        {
        }
    }
#endif

    [Parameter]
    public RenderFragment? FilterTemplate { get; set; }

    [Parameter]
    public RenderFragment<ITableColumn>? HeaderTemplate { get; set; }

    [Parameter]
    public BreakPoint ShownWithBreakPoint { get; set; }

    [Parameter]
    public IEnumerable<SelectedItem>? Items { get; set; }

    [Parameter]
    public int Order { get; set; }

    [Parameter]
    public IEnumerable<SelectedItem>? Lookup { get; set; }

    [Parameter]
    public bool ShowSearchWhenSelect { get; set; }

    [Parameter]
    public bool IsPopover { get; set; }

    [Parameter]
    public StringComparison LookupStringComparison { get; set; } = StringComparison.OrdinalIgnoreCase;

    [Parameter]
    public string? LookupServiceKey { get; set; }

    [Parameter]
    public Action<TableCellArgs>? OnCellRender { get; set; }

    [Parameter]
    public List<IValidator>? ValidateRules { get; set; }

    [CascadingParameter]
    protected ITable? Table { get; set; }

    protected override void OnInitialized()
    {
        Table?.Columns.Add(this);
        if (FieldExpression != null)
        {
            _fieldIdentifier = FieldIdentifier.Create(FieldExpression);
        }

        PropertyType = typeof(TType);
    }

    private FieldIdentifier? _fieldIdentifier;
    public string GetDisplayName() => Text ?? _fieldIdentifier?.GetDisplayName() ?? FieldName ?? "";

    [Parameter]
    public string? FieldName { get; set; }

    [Parameter]
    public string? GroupName { get; set; }

    [Parameter]
    public int GroupOrder { get; set; }

    public string GetFieldName()
    {
        if (string.IsNullOrEmpty(FieldName))
        {
            var fields = new List<string>();
            Expression? express = FieldExpression;

            while (express is LambdaExpression lambda)
            {
                express = lambda.Body;
            }

            while (express is MemberExpression member)
            {
                if (member.Expression is MemberExpression)
                {
                    fields.Add(member.Member.Name);
                }
                express = member.Expression;
            }

            if (fields.Any())
            {
                fields.Reverse();
                FieldName = string.Join(".", fields);
            }
            else
            {
                FieldName = _fieldIdentifier?.FieldName;
            }
        }
        return FieldName ?? "";
    }
}
