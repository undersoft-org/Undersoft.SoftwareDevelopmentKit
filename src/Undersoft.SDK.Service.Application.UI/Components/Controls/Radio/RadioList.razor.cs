using System.Collections;

namespace Undersoft.SDK.Service.Application.Components;

public partial class RadioList<TValue>
{
    [Parameter]
    public Color Color { get; set; }

    [Parameter]
    public bool IsButton { get; set; }

    [Parameter]
    public bool IsAutoAddNullItem { get; set; }

    [Parameter]
    [NotNull]
    public string? NullItemText { get; set; }

    [Parameter]
    public RenderFragment<SelectedItem>? ItemTemplate { get; set; }

    [Parameter]
    public bool AutoSelectFirstWhenValueIsNull { get; set; } = true;

    private string? GroupName => Id;

    private string? RadioClassString => CssBuilder.Default("radio-vector")
        .AddClass("form-control", !IsButton)
        .AddClass("is-button", IsButton)
        .Build();

    protected override void OnParametersSet()
    {
        var t = NullableUnderlyingType ?? typeof(TValue);
        if (t.IsEnum && Items == null)
        {
            Items = t.ToSelectList((NullableUnderlyingType != null && IsAutoAddNullItem) ? new SelectedItem("", NullItemText) : null);
        }

        base.OnParametersSet();

        NullItemText ??= "";

        if (AutoSelectFirstWhenValueIsNull && !Items.Any(i => i.Value == CurrentValueAsString))
        {
            CurrentValueAsString = Items.FirstOrDefault()?.Value ?? "";
        }
    }

    protected override string? FormatValueAsString(TValue value) => value is SelectedItem v ? v.Value : base.FormatValueAsString(value);

    protected override bool TryParseValueFromString(string value, [MaybeNullWhen(false)] out TValue result, out string? validationErrorMessage)
    {
        var ret = false;
        var t = NullableUnderlyingType ?? typeof(TValue);
        result = default;
        if (t == typeof(SelectedItem))
        {
            var item = Items.FirstOrDefault(i => i.Value == value);
            if (item != null)
            {
                result = (TValue)(object)item;
                ret = true;
            }
        }
        validationErrorMessage = null;
        return ret || base.TryParseValueFromString(value, out result, out validationErrorMessage);
    }

    protected override void ProcessGenericItems(Type typeValue, IEnumerable? list) { }

    protected override void EnsureParameterValid() { }

    private async Task OnClick(SelectedItem item)
    {
        if (!IsDisabled)
        {
            if (typeof(TValue) == typeof(SelectedItem))
            {
                CurrentValue = (TValue)(object)item;
            }
            else
            {
                CurrentValueAsString = item.Value;
            }
            if (OnSelectedChanged != null)
            {
                await OnSelectedChanged.Invoke(new SelectedItem[] { item }, Value);
            }

            StateHasChanged();
        }
    }

    private CheckboxState CheckState(SelectedItem item) => item.Value == CurrentValueAsString ? CheckboxState.Checked : CheckboxState.UnChecked;

    private RenderFragment? GetChildContent(SelectedItem item) => ItemTemplate == null
        ? null
        : ItemTemplate(item);
}
