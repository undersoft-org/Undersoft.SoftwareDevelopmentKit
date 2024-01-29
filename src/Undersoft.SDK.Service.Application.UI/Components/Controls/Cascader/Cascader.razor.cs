using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class Cascader<TValue>
{
    private List<CascaderItem> SelectedItems { get; } = new();

    private string? InputId => $"{Id}_input";

    protected string? DisplayTextString { get; set; }

    [Parameter]
    public Color Color { get; set; } = Color.None;

    [Parameter]
    public string? PlaceHolder { get; set; }

    [Parameter]
    [NotNull]
#if NET6_0_OR_GREATER
    [EditorRequired]
#endif
    public IEnumerable<CascaderItem>? Items { get; set; }

    [Parameter]
    public Func<CascaderItem[], Task>? OnSelectedItemChanged { get; set; }

    [Parameter]
    public bool ParentSelectable { get; set; } = true;

    [Parameter]
    public bool ShowFullLevels { get; set; } = true;

    [Parameter]
    public string? Icon { get; set; }

    [Parameter]
    public string? SubMenuIcon { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<Cascader<TValue>>? Localizer { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    private string _lastVaslue = string.Empty;

    private string? SubMenuIconString => CssBuilder.Default("nav-link-right")
        .AddClass(SubMenuIcon, !string.IsNullOrEmpty(SubMenuIcon))
        .Build();

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        Icon ??= IconTheme.GetIconByKey(ComponentIcons.CascaderIcon);
        SubMenuIcon ??= IconTheme.GetIconByKey(ComponentIcons.CascaderSubMenuIcon);

        Items ??= Enumerable.Empty<CascaderItem>();

        PlaceHolder ??= Localizer[nameof(PlaceHolder)];

        if (_lastVaslue != CurrentValueAsString)
        {
            _lastVaslue = CurrentValueAsString;
            SetDefaultValue(CurrentValueAsString);
        }
    }

    private void SetDefaultValue(string defaultValue)
    {
        SelectedItems.Clear();
        var item = GetNodeByValue(Items, defaultValue);
        if (item != null)
        {
            SetSelectedNodeWithParent(item, SelectedItems);
        }
        else
        {
            CurrentValueAsString = Items.FirstOrDefault()?.Value ?? string.Empty;
        }
        RefreshDisplayText();
    }

    private CascaderItem? GetNodeByValue(IEnumerable<CascaderItem> items, string value)
    {
        foreach (var item in items)
        {
            if (item.Value == value)
            {
                return item;
            }

            if (item.HasChildren)
            {
                var nd = GetNodeByValue(item.Items, value);
                if (nd != null)
                {
                    return nd;
                }
            }
        }
        return null;
    }

    private string? ClassString => CssBuilder.Default("select cascade menu dropdown")
        .AddClass("disabled", IsDisabled)
        .AddClass(CssClass).AddClass(ValidCss)
        .Build();

    private string? InputClassName => CssBuilder.Default("form-control form-select")
        .AddClass($"border-{Color.ToDescriptionString()}", Color != Color.None && !IsDisabled)
        .AddClass(ValidCss)
        .Build();

    private string? AppendClassName => CssBuilder.Default("form-select-append")
        .AddClass($"text-{Color.ToDescriptionString()}", Color != Color.None && !IsDisabled)
        .Build();

    private string? ActiveItem(string className, CascaderItem item) => CssBuilder.Default(className)
        .AddClass("active", () => SelectedItems.Contains(item))
        .Build();

    private Task OnItemClick(CascaderItem item) => SetSelectedItem(item);

    private async Task SetSelectedItem(CascaderItem item)
    {
        if (ParentSelectable || !item.HasChildren)
        {
            SelectedItems.Clear();
            SetSelectedNodeWithParent(item, SelectedItems);
            await SetValue(item.Value);
            await JSRuntime.InvokeVoidAsync(InputId, "bb_cascader_hide");
        }
    }

    private async Task SetValue(string value)
    {
        RefreshDisplayText();
        CurrentValueAsString = value;
        if (OnSelectedItemChanged != null)
        {
            await OnSelectedItemChanged(SelectedItems.ToArray());
        }
        if (SelectedItems.Count != 1)
        {
            StateHasChanged();
        }
    }

    private void RefreshDisplayText() => DisplayTextString = ShowFullLevels
        ? string.Join("/", SelectedItems.Select(item => item.Text))
        : SelectedItems.LastOrDefault()?.Text;

    private static void SetSelectedNodeWithParent(CascaderItem? item, List<CascaderItem> list)
    {
        if (item != null)
        {
            SetSelectedNodeWithParent(item.Parent, list);
            list.Add(item);
        }
    }
}
