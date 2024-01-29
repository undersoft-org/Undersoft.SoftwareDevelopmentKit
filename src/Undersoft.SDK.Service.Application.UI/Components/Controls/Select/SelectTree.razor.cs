using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

[JSModuleAutoLoader("select-tree")]
public partial class SelectTree<TValue> : IModelEqualityComparer<TValue>
{
    private string? ClassName => CssBuilder.Default("select dropdown select-tree")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? InputClassName => CssBuilder.Default("form-select form-control")
        .AddClass($"border-{Color.ToDescriptionString()}", Color != Color.None && !IsDisabled && !IsValid.HasValue)
        .AddClass($"border-success", IsValid.HasValue && IsValid.Value)
        .AddClass($"border-danger", IsValid.HasValue && !IsValid.Value)
        .AddClass(CssClass).AddClass(ValidCss)
        .Build();

    private string? AppendClassName => CssBuilder.Default("form-select-append")
        .AddClass($"text-{Color.ToDescriptionString()}", Color != Color.None && !IsDisabled && !IsValid.HasValue)
        .AddClass($"text-success", IsValid.HasValue && IsValid.Value)
        .AddClass($"text-danger", IsValid.HasValue && !IsValid.Value)
        .Build();

    [Parameter]
    public Color Color { get; set; }

    [Parameter]
    public string? PlaceHolder { get; set; }

    [Parameter]
    public StringComparison StringComparison { get; set; } = StringComparison.OrdinalIgnoreCase;

    [Parameter]
    [NotNull]
#if NET6_0_OR_GREATER
    [EditorRequired]
#endif
    public List<TreeViewItem<TValue>>? Items { get; set; }

    [Parameter]
    public Func<TValue, Task>? OnSelectedItemChanged { get; set; }

    [Parameter]
    [NotNull]
    public Func<TreeViewItem<TValue>, Task<IEnumerable<TreeViewItem<TValue>>>>? OnExpandNodeAsync { get; set; }

    [Parameter]
    public Type CustomKeyAttribute { get; set; } = typeof(KeyAttribute);

    [Parameter]
    [NotNull]
    public Func<TValue, TValue, bool>? ModelEqualityComparer { get; set; }

    [Parameter]
    public bool ShowIcon { get; set; }

    [Parameter]
    public string? DropdownIcon { get; set; }

    [Parameter]
    public bool IsEdit { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<SelectTree<TValue>>? Localizer { get; set; }

    protected override string? RetrieveId() => InputId;

    private string? InputId => $"{Id}_input";

    private TreeViewItem<TValue>? SelectedItem { get; set; }

    private List<TreeViewItem<TValue>>? ItemCache { get; set; }

    [NotNull]
    private List<TreeViewItem<TValue>>? ExpandedItemsCache { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (Value != null)
        {
            await TriggerItemChanged(s => Equals(s.Value, Value));
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        DropdownIcon ??= IconTheme.GetIconByKey(ComponentIcons.SelectTreeDropdownIcon);
        PlaceHolder ??= Localizer[nameof(PlaceHolder)];

        Items ??= new List<TreeViewItem<TValue>>();

        if (Value == null)
        {
            await TriggerItemChanged(s => s.IsActive);
        }
    }

    protected override bool TryParseValueFromString(string value, [MaybeNullWhen(false)] out TValue result, out string? validationErrorMessage)
    {
        result = (TValue)(object)value;
        validationErrorMessage = null;
        return true;
    }

    private void OnChange(ChangeEventArgs args)
    {
        if (args.Value is string v)
        {
            CurrentValueAsString = v;
        }
    }

    private async Task TriggerItemChanged(Func<TreeViewItem<TValue>, bool> predicate)
    {
        var currentItem = GetExpandedItems().FirstOrDefault(predicate);
        if (currentItem != null)
        {
            await ItemChanged(currentItem);
        }
    }

    private IEnumerable<TreeViewItem<TValue>> GetExpandedItems()
    {
        if (ItemCache != Items)
        {
            ItemCache = Items;
            ExpandedItemsCache = TreeItemExtensions.GetAllItems(ItemCache).ToList();
        }
        return ExpandedItemsCache;
    }

    private async Task OnItemClick(TreeViewItem<TValue> item)
    {
        if (!Equals(item.Value, CurrentValue))
        {
            await ItemChanged(item);
            StateHasChanged();
        }
    }

    private async Task ItemChanged(TreeViewItem<TValue> item)
    {
        SelectedItem = item;
        CurrentValue = item.Value;

        if (OnSelectedItemChanged != null)
        {
            await OnSelectedItemChanged.Invoke(CurrentValue);
        }
    }

    public bool Equals(TValue? x, TValue? y) => this.Equals<TValue>(x, y);
}
