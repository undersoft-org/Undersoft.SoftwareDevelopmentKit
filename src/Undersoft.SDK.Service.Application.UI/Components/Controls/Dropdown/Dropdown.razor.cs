namespace Undersoft.SDK.Service.Application.Components;

public partial class Dropdown<TValue>
{
    private string? DirectionClassName => CssBuilder.Default()
        .AddClass($"btn-group", DropdownType == DropdownType.ButtonGroup)
        .AddClass(Direction.ToDescriptionString(), DropdownType == DropdownType.DropdownMenu)
        .AddClass($"{Direction.ToDescriptionString()}-center", MenuAlignment == Alignment.Center && (Direction == Direction.Dropup || Direction == Direction.Dropdown))
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? ButtonClassName => CssBuilder.Default("btn")
        .AddClass("dropdown-toggle", !ShowSplit)
        .AddClass($"btn-primary", Color == Color.None)
        .AddClass($"btn-{Color.ToDescriptionString()}", Color != Color.None)
        .AddClass($"btn-{Size.ToDescriptionString()}", Size != Size.None)
        .Build();

    private string? ClassName => CssBuilder.Default("btn dropdown-toggle")
      .AddClass("dropdown-toggle-split")
      .AddClass($"btn-{Color.ToDescriptionString()}", Color != Color.None)
      .AddClass($"btn-{Size.ToDescriptionString()}", Size != Size.None)
      .Build();

    private string? DropdownToggle => !ShowSplit ? "dropdown" : null;

    private string? MenuAlignmentClass => CssBuilder.Default("dropdown-menu shadow")
        .AddClass($"dropdown-menu-{MenuAlignment.ToDescriptionString()}", MenuAlignment == Alignment.Right)
        .Build();

    protected string? ActiveItem(SelectedItem item) => CssBuilder.Default("dropdown-item")
        .AddClass("active", () => item.Value == CurrentValueAsString)
        .Build();

    [Parameter]
    public Color Color { get; set; }

    [Parameter]
    [NotNull]
    public IEnumerable<SelectedItem>? Items { get; set; }

    [Parameter]
    public RenderFragment<SelectedItem>? ItemTemplate { get; set; }

    [Parameter]
    public bool ShowSplit { get; set; }

    [Parameter]
    public Alignment MenuAlignment { get; set; }

    [Parameter]
    public Direction Direction { get; set; }

    [Parameter]
    public Size Size { get; set; }

    [Parameter]
    public bool IsFixedButtonText { get; set; }

    [Parameter]
    public bool ShowFixedButtonTextInDropdown { get; set; }

    [Parameter]
    public DropdownType DropdownType { get; set; }

    [Parameter]
    public string? FixedButtonText { get; set; }

    [Parameter]
    public Func<SelectedItem, Task>? OnSelectedItemChanged { get; set; }

    [NotNull]
    private List<SelectedItem>? DataSource { get; set; }

    private SelectedItem? SelectedItem { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        Items ??= Enumerable.Empty<SelectedItem>();

        if (!Items.Any() && typeof(TValue).IsEnum())
        {
            Items = typeof(TValue).ToSelectList();
        }

        DataSource = Items.ToList();

        SelectedItem = DataSource.FirstOrDefault(i => i.Value.Equals(CurrentValueAsString, StringComparison.OrdinalIgnoreCase))
            ?? DataSource.FirstOrDefault(i => i.Active)
            ?? DataSource.FirstOrDefault();

        FixedButtonText ??= SelectedItem?.Text;
    }

    private IEnumerable<SelectedItem> GetItems() => (IsFixedButtonText && !ShowFixedButtonTextInDropdown)
        ? Items.Where(i => i.Text != FixedButtonText)
        : Items;

    protected async Task OnItemClick(SelectedItem item)
    {
        item.Active = true;
        SelectedItem = item;
        CurrentValueAsString = item.Value;

        if (OnSelectedItemChanged != null)
        {
            await OnSelectedItemChanged.Invoke(SelectedItem);
        }
    }

    private string? ButtonText => IsFixedButtonText ? FixedButtonText : SelectedItem?.Text;
}
