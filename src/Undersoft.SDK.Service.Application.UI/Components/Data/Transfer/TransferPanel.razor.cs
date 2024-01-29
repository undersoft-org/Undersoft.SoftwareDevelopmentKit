using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class TransferPanel
{
    protected string? SearchText { get; set; }

    private string? PanelClassString => CssBuilder.Default("transfer-panel")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? SearchClass => CssBuilder.Default("Input-prefix")
        .AddClass("is-on", !string.IsNullOrEmpty(SearchText))
        .AddClass("disabled", IsDisabled)
        .Build();

    private string? PanelListClassString => CssBuilder.Default("transfer-panel-vector scroll")
        .AddClass("search", ShowSearch)
        .AddClass("disabled", IsDisabled)
        .Build();

    private string? GetItemClass(SelectedItem item) => CssBuilder.Default("transfer-panel-item")
        .AddClass(OnSetItemClass?.Invoke(item))
        .Build();

    private string? Disabled => IsDisabled ? "disabled" : null;

    [Parameter]
    [NotNull]
#if NET6_0_OR_GREATER
    [EditorRequired]
#endif
    public List<SelectedItem>? Items { get; set; }

    [Parameter]
    public Func<SelectedItem, string?>? OnSetItemClass { get; set; }

    [Parameter]
    [NotNull]
    public string? Text { get; set; }

    [Parameter]
    public bool ShowSearch { get; set; }

    [Parameter]
    public string? SearchIcon { get; set; }

    [Parameter]
    public Func<Task>? OnSelectedItemsChanged { get; set; }

    [Parameter]
    [NotNull]
    public string? SearchPlaceHolderString { get; set; }

    [Parameter]
    public bool IsDisabled { get; set; }

    [Parameter]
    public RenderFragment<List<SelectedItem>>? HeaderTemplate { get; set; }

    [Parameter]
    public RenderFragment<SelectedItem>? ItemTemplate { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<Transfer<string>>? Localizer { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        Items ??= new();
        SearchPlaceHolderString ??= Localizer[nameof(SearchPlaceHolderString)];
        Text ??= Localizer[nameof(Text)];

        SearchIcon ??= IconTheme.GetIconByKey(ComponentIcons.TransferPanelSearchIcon);
    }

    protected CheckboxState HeaderCheckState()
    {
        var ret = CheckboxState.Indeterminate;
        if (Items.Any() && Items.All(i => i.Active))
        {
            ret = CheckboxState.Checked;
        }
        else if (!Items.Any(i => i.Active))
        {
            ret = CheckboxState.UnChecked;
        }

        return ret;
    }

    protected async Task OnHeaderCheck(CheckboxState state, SelectedItem item)
    {
        if (Items != null)
        {
            if (state == CheckboxState.Checked)
            {
                GetShownItems().ForEach(i => i.Active = true);
            }
            else
            {
                GetShownItems().ForEach(i => i.Active = false);
            }

            if (OnSelectedItemsChanged != null)
            {
                await OnSelectedItemsChanged();
            }
        }
    }

    protected async Task OnStateChanged(CheckboxState state, SelectedItem item)
    {
        item.Active = state == CheckboxState.Checked;

        if (OnSelectedItemsChanged != null)
        {
            await OnSelectedItemsChanged();
        }
    }

    protected virtual void OnSearch(ChangeEventArgs e)
    {
        if (e.Value != null)
        {
            SearchText = e.Value.ToString();
        }
    }

    protected void OnKeyUp(KeyboardEventArgs e)
    {
        if (e.Key == "Escape")
        {
            ClearSearch();
        }
    }

    protected void ClearSearch()
    {
        SearchText = "";
    }

    private List<SelectedItem> GetShownItems() => (string.IsNullOrEmpty(SearchText)
        ? Items
        : Items.Where(i => i.Text.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList());
}
