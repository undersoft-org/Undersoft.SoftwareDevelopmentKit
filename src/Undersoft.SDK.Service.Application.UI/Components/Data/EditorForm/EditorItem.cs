using Microsoft.AspNetCore.Components.Forms;
using System.Linq.Expressions;

namespace Undersoft.SDK.Service.Application.Components;

#if NET6_0_OR_GREATER
public class EditorItem<TModel, TValue> : ComponentBase, IEditorItem
#else
public class EditorItem<TValue> : ComponentBase, IEditorItem
#endif
{
    [Parameter]
    public TValue? Field { get; set; }

    [Parameter]
    public EventCallback<TValue> FieldChanged { get; set; }

    [NotNull]
    public Type? PropertyType { get; set; }

    [Parameter]
    public Expression<Func<TValue>>? FieldExpression { get; set; }

    [Parameter]
    public bool Editable { get; set; } = true;

    [Parameter]
    public bool Readonly { get; set; }

    public bool IsReadonlyWhenAdd { get; set; }

    public bool IsReadonlyWhenEdit { get; set; }

    [Parameter]
    public bool SkipValidate { get; set; }

    [Parameter]
    public bool? ShowLabelTooltip { get; set; }

    [Parameter]
    public string? Text { get; set; }

    [Parameter]
    public object? Step { get; set; }

    [Parameter]
    public int Rows { get; set; }

    [Parameter]
#if NET5_0
    public RenderFragment<object>? EditTemplate { get; set; }
#elif NET6_0_OR_GREATER
    public RenderFragment<TModel>? EditTemplate { get; set; }

    RenderFragment<object>? IEditorItem.EditTemplate
    {
        get
        {
            return EditTemplate == null ? null : new RenderFragment<object>(item => builder =>
            {
                builder.AddContent(0, EditTemplate((TModel)item));
            });
        }
        set
        {
        }
    }
#endif

    [Parameter]
    public Type? ComponentType { get; set; }

    [Parameter]
    public IEnumerable<KeyValuePair<string, object>>? ComponentParameters { get; set; }

    [Parameter]
    public string? PlaceHolder { get; set; }

    [Parameter]
    public int Order { get; set; }

    [Parameter]
    public IEnumerable<SelectedItem>? Items { get; set; }

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
    public List<IValidator>? ValidateRules { get; set; }

    [CascadingParameter]
    private List<IEditorItem>? EditorItems { get; set; }

    [Parameter]
    public string? GroupName { get; set; }

    [Parameter]
    public int GroupOrder { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        EditorItems?.Add(this);
        if (FieldExpression != null)
        {
            _fieldIdentifier = FieldIdentifier.Create(FieldExpression);
        }

        PropertyType = typeof(TValue);
    }

    private FieldIdentifier? _fieldIdentifier;
    public string GetDisplayName() => Text ?? _fieldIdentifier?.GetDisplayName() ?? string.Empty;

    public string GetFieldName() => _fieldIdentifier?.FieldName ?? string.Empty;
}
