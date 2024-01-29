using Microsoft.AspNetCore.Components.Forms;
using System.Linq.Expressions;

namespace Undersoft.SDK.Service.Application.Components;

public abstract class DisplayBase<TValue> : ComponentModule2
{
    protected bool IsShowLabel { get; set; }

    protected FieldIdentifier? FieldIdentifier { get; set; }

    protected Type? NullableUnderlyingType { get; set; }

    [NotNull]
    protected Type? ValueType { get; set; }

    [Parameter]
    [NotNull]
    public TValue? Value { get; set; }

    [Parameter]
    public EventCallback<TValue> ValueChanged { get; set; }

    [Parameter]
    public Expression<Func<TValue>>? ValueExpression { get; set; }

    [Parameter]
    public bool? ShowLabel { get; set; }

    [Parameter]
    public bool? ShowLabelTooltip { get; set; }

    [Parameter]
    public string? DisplayText { get; set; }

    [CascadingParameter]
    protected ValidateForm? ValidateForm { get; set; }

    [CascadingParameter(Name = "EidtorForm")]
    protected IShowLabel? EditorForm { get; set; }

    [CascadingParameter]
    protected BootstrapInputGroup? InputGroup { get; set; }

    [CascadingParameter]
    protected IFilter? Filter { get; set; }

    public override Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);

        NullableUnderlyingType = Nullable.GetUnderlyingType(typeof(TValue));
        ValueType ??= NullableUnderlyingType ?? typeof(TValue);

        if (ValueExpression != null)
        {
            FieldIdentifier = Microsoft.AspNetCore.Components.Forms.FieldIdentifier.Create(ValueExpression);
        }

        return base.SetParametersAsync(ParameterView.Empty);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        var showLabel = ShowLabel;

        if (Filter != null)
        {
            IsShowLabel = false;
        }
        else if (InputGroup == null)
        {
            if (ShowLabel == null && (EditorForm != null || ValidateForm != null))
            {
                showLabel = EditorForm?.ShowLabel ?? ValidateForm?.ShowLabel ?? true;
            }

            IsShowLabel = showLabel ?? false;

            if (IsShowLabel && DisplayText == null && FieldIdentifier.HasValue)
            {
                DisplayText = FieldIdentifier.Value.GetDisplayName();
            }
        }
        else
        {
            IsShowLabel = false;

            if (DisplayText == null && FieldIdentifier.HasValue)
            {
                DisplayText = FieldIdentifier.Value.GetDisplayName();
            }
        }

        if (ShowLabelTooltip == null && EditorForm != null)
        {
            ShowLabelTooltip = EditorForm.ShowLabelTooltip;
        }

        if (ShowLabelTooltip == null && ValidateForm != null)
        {
            ShowLabelTooltip = ValidateForm.ShowLabelTooltip;
        }
    }

    protected virtual string? FormatValueAsString(TValue value)
    {
        string? ret;
        if (value is SelectedItem item)
        {
            ret = item.Value;
        }
        else
        {
            ret = value?.ToString();
        }
        return ret;
    }
}
