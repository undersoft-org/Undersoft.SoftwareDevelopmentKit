namespace Undersoft.SDK.Service.Application.Components;

public abstract class SelectBase<TValue> : PopoverSelectBase<TValue>
{
    [Parameter]
    public Color Color { get; set; }

    [Parameter]
    [NotNull]
    public IEnumerable<SelectedItem>? Items { get; set; }

    [Parameter]
    public bool ShowSearch { get; set; }

    [Parameter]
    public string? SearchIcon { get; set; }

    [Parameter]
    public StringComparison StringComparison { get; set; } = StringComparison.OrdinalIgnoreCase;

    [Parameter]
    public RenderFragment<SelectedItem>? ItemTemplate { get; set; }

    [Inject]
    [NotNull]
    protected IIconTheme? IconTheme { get; set; }

    protected string? SearchText { get; set; }

    protected string? SearchIconString => CssBuilder.Default("icon")
        .AddClass(SearchIcon)
        .Build();

    protected override string? CustomClassString => CssBuilder.Default()
        .AddClass("select", IsPopover)
        .AddClass(base.CustomClassString)
        .Build();

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        SearchIcon ??= IconTheme.GetIconByKey(ComponentIcons.SelectSearchIcon);
    }
}
