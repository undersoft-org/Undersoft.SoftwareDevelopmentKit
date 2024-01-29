using Microsoft.Extensions.Localization;
using System.Collections;

namespace Undersoft.SDK.Service.Application.Components;

[JSModuleAutoLoader("multi-select", JSObjectReference = true)]
public partial class MultiSelect<TValue>
{
    [NotNull]
    private List<SelectedItem>? DataSource { get; set; }

    private IEnumerable<SelectedItem> SelectedItems => DataSource.Where(i => i.Active);

    private static string? ClassString => CssBuilder.Default("select dropdown multi-select")
        .Build();

    private string? ToggleClassString => CssBuilder.Default("dropdown-toggle")
        .AddClass($"border-{Color.ToDescriptionString()}", Color != Color.None && !IsDisabled)
        .AddClass("disabled", IsDisabled)
        .AddClass(CssClass).AddClass(ValidCss)
        .Build();

    private string? GetItemClassString(SelectedItem item) => CssBuilder.Default("dropdown-item")
        .AddClass("active", GetCheckedState(item))
        .Build();

    private string? PlaceHolderClassString => CssBuilder.Default("multi-select-ph")
        .AddClass("d-none", SelectedItems.Any())
        .Build();

    [Parameter]
    [NotNull]
    public string? PlaceHolder { get; set; }

    [Parameter]
    public bool ShowCloseButton { get; set; } = true;

    [Parameter]
    public bool ShowToolbar { get; set; }

    [Parameter]
    public bool ShowDefaultButtons { get; set; } = true;

    [Parameter]
    public RenderFragment? ButtonTemplate { get; set; }

    [Parameter]
    public Func<string, IEnumerable<SelectedItem>>? OnSearchTextChanged { get; set; }

    [Parameter]
    public Func<IEnumerable<SelectedItem>, Task>? OnSelectedItemsChanged { get; set; }

    [Parameter]
    [NotNull]
    public string? SelectAllText { get; set; }

    [Parameter]
    [NotNull]
    public string? ReverseSelectText { get; set; }

    [Parameter]
    [NotNull]
    public string? ClearText { get; set; }

    [Parameter]
    public int Max { get; set; }

    [Parameter]
    [NotNull]
    public string? MaxErrorMessage { get; set; }

    [Parameter]
    public int Min { get; set; }

    [Parameter]
    [NotNull]
    public string? MinErrorMessage { get; set; }

    [Parameter]
    [NotNull]
    public string? ClearIcon { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<MultiSelect<TValue>>? Localizer { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        PlaceHolder ??= Localizer[nameof(PlaceHolder)];
        SelectAllText ??= Localizer[nameof(SelectAllText)];
        ReverseSelectText ??= Localizer[nameof(ReverseSelectText)];
        ClearText ??= Localizer[nameof(ClearText)];
        MinErrorMessage ??= Localizer[nameof(MinErrorMessage)];
        MaxErrorMessage ??= Localizer[nameof(MaxErrorMessage)];

        ClearIcon ??= IconTheme.GetIconByKey(ComponentIcons.MultiSelectClearIcon);

        ResetItems();

        OnSearchTextChanged ??= text => Items.Where(i => i.Text.Contains(text, StringComparison.OrdinalIgnoreCase));

        ResetRules();
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        var list = CurrentValueAsString.Split(',', StringSplitOptions.RemoveEmptyEntries);
        foreach (var item in Items)
        {
            item.Active = list.Any(i => i.Equals(item.Value, StringComparison.OrdinalIgnoreCase));
        }
    }

    protected override Task ModuleInitAsync() => InvokeInitAsync(Id, nameof(ToggleRow));

    protected override string? FormatValueAsString(TValue value) => value == null
        ? null
        : Utility.ConvertValueToString(value);

    [JSInvokable]
    public async Task ToggleRow(string val)
    {
        if (!IsDisabled)
        {
            var d = DataSource.FirstOrDefault(i => i.Value == val);
            if (d != null)
            {
                d.Active = !d.Active;
            }

            await SetValue();

            await TriggerSelectedItemChanged();

            StateHasChanged();
        }
    }

    private string? GetValueString(SelectedItem item) => IsPopover ? item.Value : null;

    private async Task TriggerSelectedItemChanged()
    {
        if (OnSelectedItemsChanged != null)
        {
            await OnSelectedItemsChanged.Invoke(SelectedItems);
        }
    }

    private int _min;
    private int _max;
    private void ResetRules()
    {
        if (Max != _max)
        {
            _max = Max;
            Rules.RemoveAll(v => v is MaxValidator);

            if (Max > 0)
            {
                Rules.Add(new MaxValidator() { Value = Max, ErrorMessage = MaxErrorMessage });
            }
        }

        if (Min != _min)
        {
            _min = Min;
            Rules.RemoveAll(v => v is MinValidator);

            if (Min > 0)
            {
                Rules.Add(new MinValidator() { Value = Min, ErrorMessage = MinErrorMessage });
            }
        }
    }

    private async Task SetValue()
    {
        var typeValue = NullableUnderlyingType ?? typeof(TValue);
        if (typeValue == typeof(string))
        {
            CurrentValueAsString = string.Join(",", SelectedItems.Select(i => i.Value));
        }
        else if (typeValue.IsGenericType || typeValue.IsArray)
        {
            var t = typeValue.IsGenericType ? typeValue.GenericTypeArguments[0] : typeValue.GetElementType()!;
            var listType = typeof(List<>).MakeGenericType(t);
            var instance = (IList)Activator.CreateInstance(listType, SelectedItems.Count())!;

            foreach (var item in SelectedItems)
            {
                if (item.Value.TryConvertTo(t, out var val))
                {
                    instance.Add(val);
                }
            }
            CurrentValue = (TValue)(typeValue.IsGenericType ? instance : listType.GetMethod("ToArray")!.Invoke(instance, null)!);
        }

        if (ValidateForm == null && (Min > 0 || Max > 0))
        {
            var validationContext = new ValidationContext(Value!) { MemberName = FieldIdentifier?.FieldName };
            var validationResults = new List<ValidationResult>();

            await ValidatePropertyAsync(CurrentValue, validationContext, validationResults);
            ToggleMessage(validationResults, true);
        }
    }

    private async Task Clear()
    {
        foreach (var item in Items)
        {
            item.Active = false;
        }

        await SetValue();

        await TriggerSelectedItemChanged();
    }

    private async Task SelectAll()
    {
        foreach (var item in GetData())
        {
            item.Active = true;
        }

        await SetValue();

        await TriggerSelectedItemChanged();
    }

    private async Task InvertSelect()
    {
        foreach (var item in GetData())
        {
            item.Active = !item.Active;
        }

        await SetValue();

        await TriggerSelectedItemChanged();
    }

    private bool GetCheckedState(SelectedItem item) => SelectedItems.Any(i => i.Value == item.Value);

    private bool CheckCanTrigger(SelectedItem item)
    {
        var ret = true;
        if (Max > 0)
        {
            ret = SelectedItems.Count() < Max || GetCheckedState(item);
        }
        return ret;
    }

    private bool CheckCanSelect(SelectedItem item)
    {
        var ret = GetCheckedState(item);
        if (!ret)
        {
            ret = CheckCanTrigger(item);
        }
        return !ret;
    }

    private IEnumerable<SelectedItem> GetData()
    {
        var data = Items;
        if (ShowSearch && !string.IsNullOrEmpty(SearchText) && OnSearchTextChanged != null)
        {
            data = OnSearchTextChanged(SearchText).ToList();
        }
        return data;
    }

    protected override void OnValidate(bool? valid)
    {
        if (valid != null)
        {
            Color = valid.Value ? Color.Success : Color.Danger;
        }
    }

    private void ResetItems()
    {
        if (Items == null)
        {
            var type = typeof(TValue);
            Type? innerType;
            if (type.IsGenericType && type.IsAssignableTo(typeof(IEnumerable)))
            {
                innerType = type.GetGenericArguments()[0];
            }
            else
            {
                innerType = NullableUnderlyingType ?? type;
            }
            if (innerType.IsEnum)
            {
                Items = innerType.ToSelectList();
            }
            else
            {
                Items = Enumerable.Empty<SelectedItem>();
            }
        }

        DataSource = Items.ToList();
    }
}
