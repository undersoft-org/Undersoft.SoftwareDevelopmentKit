using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

[JSModuleAutoLoader(JSObjectReference = true)]
public partial class Select<TValue> : ISelect
{
    [Inject]
    [NotNull]
    private SwalService? SwalService { get; set; }

    private string? ClassString => CssBuilder.Default("select dropdown")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? InputClassString => CssBuilder.Default("form-select form-control")
        .AddClass($"border-{Color.ToDescriptionString()}", Color != Color.None && !IsDisabled && !IsValid.HasValue)
        .AddClass($"border-success", IsValid.HasValue && IsValid.Value)
        .AddClass($"border-danger", IsValid.HasValue && !IsValid.Value)
        .AddClass(CssClass).AddClass(ValidCss)
        .Build();

    private string? AppendClassString => CssBuilder.Default("form-select-append")
        .AddClass($"text-{Color.ToDescriptionString()}", Color != Color.None && !IsDisabled && !IsValid.HasValue)
        .AddClass($"text-success", IsValid.HasValue && IsValid.Value)
        .AddClass($"text-danger", IsValid.HasValue && !IsValid.Value)
        .Build();

    private string? ActiveItem(SelectedItem item) => CssBuilder.Default("dropdown-item")
        .AddClass("active", () => item.Value == CurrentValueAsString)
        .AddClass("disabled", item.IsDisabled)
        .Build();

    private string? SearchClassString => CssBuilder.Default("search")
        .AddClass("is-fixed", IsFixedSearch)
        .Build();

    private List<SelectedItem> Children { get; } = new();

    [NotNull]
    private List<SelectedItem> DataSource { get; } = new();

    [Parameter]
    [NotNull]
    public string? DropdownIcon { get; set; }

    [Parameter]
    [NotNull]
    public Func<string, IEnumerable<SelectedItem>>? OnSearchTextChanged { get; set; }

    [Parameter]
    public bool IsFixedSearch { get; set; }

    [Parameter]
    public string? NoSearchDataText { get; set; }

    [Parameter]
    public string? PlaceHolder { get; set; }

    [Parameter]
    public RenderFragment? Options { get; set; }

    [Parameter]
    public RenderFragment<SelectedItem?>? DisplayTemplate { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<Select<TValue>>? Localizer { get; set; }

    protected override string? RetrieveId() => InputId;

    private string? InputId => $"{Id}_input";

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        Items ??= Enumerable.Empty<SelectedItem>();
        OnSearchTextChanged ??= text => Items.Where(i => i.Text.Contains(text, StringComparison));
        PlaceHolder ??= Localizer[nameof(PlaceHolder)];
        NoSearchDataText ??= Localizer[nameof(NoSearchDataText)];
        DropdownIcon ??= IconTheme.GetIconByKey(ComponentIcons.SelectDropdownIcon);

        if (!Items.Any() && ValueType.IsEnum())
        {
            var item = NullableUnderlyingType == null ? "" : PlaceHolder;
            Items = ValueType.ToSelectList(string.IsNullOrEmpty(item) ? null : new SelectedItem("", item));
        }
    }

    protected override bool TryParseValueFromString(string value, [MaybeNullWhen(false)] out TValue result, out string? validationErrorMessage) => ValueType == typeof(SelectedItem)
        ? TryParseSelectItem(value, out result, out validationErrorMessage)
        : base.TryParseValueFromString(value, out result, out validationErrorMessage);

    private bool TryParseSelectItem(string value, [MaybeNullWhen(false)] out TValue result, out string? validationErrorMessage)
    {
        SelectedItem = DataSource.FirstOrDefault(i => i.Value == value);

        result = default;
        if (SelectedItem != null)
        {
            result = (TValue)(object)SelectedItem;
        }
        validationErrorMessage = "";
        return SelectedItem != null;
    }

    private void ResetSelectedItem()
    {
        DataSource.Clear();

        if (string.IsNullOrEmpty(SearchText))
        {
            DataSource.AddRange(Items);
            DataSource.AddRange(Children);

            SelectedItem = DataSource.FirstOrDefault(i => i.Value.Equals(CurrentValueAsString, StringComparison))
                ?? DataSource.FirstOrDefault(i => i.Active)
                ?? DataSource.FirstOrDefault();

            if (!string.IsNullOrEmpty(SelectedItem?.Value) && CurrentValueAsString != SelectedItem.Value)
            {
                _ = SelectedItemChanged(SelectedItem);
            }
        }
        else
        {
            DataSource.AddRange(OnSearchTextChanged(SearchText));
        }
    }

    protected override async Task ModuleInitAsync()
    {
        if (SelectedItem != null && OnSelectedItemChanged != null && !string.IsNullOrEmpty(SelectedItem.Value))
        {
            await OnSelectedItemChanged.Invoke(SelectedItem);
        }

        await InvokeInitAsync(Id, nameof(ConfirmSelectedItem));
    }

    [JSInvokable]
    public async Task ConfirmSelectedItem(int index)
    {
        var ds = string.IsNullOrEmpty(SearchText)
            ? DataSource
            : OnSearchTextChanged(SearchText);
        var item = ds.ElementAt(index);
        await OnClickItem(item);
        StateHasChanged();
    }

    private async Task OnClickItem(SelectedItem item)
    {
        var ret = true;
        if (OnBeforeSelectedItemChange != null)
        {
            ret = await OnBeforeSelectedItemChange(item);
            if (ret)
            {
                var option = new SwalOption()
                {
                    Category = SwalCategory,
                    Title = SwalTitle,
                    Content = SwalContent
                };
                if (!string.IsNullOrEmpty(SwalFooter))
                {
                    option.ShowFooter = true;
                    option.FooterTemplate = builder => builder.AddContent(0, SwalFooter);
                }
                ret = await SwalService.ShowModal(option);
            }
            else
            {
                ret = true;
            }
        }
        if (ret)
        {
            await SelectedItemChanged(item);
        }
    }

    private async Task SelectedItemChanged(SelectedItem item)
    {
        item.Active = true;
        SelectedItem = item;

        if (CurrentValueAsString != item.Value)
        {
            CurrentValueAsString = item.Value;

            if (OnSelectedItemChanged != null)
            {
                await OnSelectedItemChanged(SelectedItem);
            }
        }
    }

    public void Add(SelectedItem item) => Children.Add(item);
}
