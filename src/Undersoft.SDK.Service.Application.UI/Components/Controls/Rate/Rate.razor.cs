namespace Undersoft.SDK.Service.Application.Components;

public partial class Rate
{
    private string? ClassString => CssBuilder.Default("rate")
        .AddClass("disabled", IsDisable)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? GetItemClassString(int i) => CssBuilder.Default("rate-item")
        .AddClass("is-on", Value >= i)
        .Build();

    private string? GetIcon(int i) => Value >= i ? StarIcon : UnStarIcon;

    [Parameter]
    [NotNull]
    public string? StarIcon { get; set; }

    [Parameter]
    [NotNull]
    public string? UnStarIcon { get; set; }

    [Parameter]
    public int Value { get; set; }

    [Parameter]
    public bool IsDisable { get; set; }

    [Parameter]
    public RenderFragment<int>? ItemTemplate { get; set; }

    [Parameter]
    public EventCallback<int> ValueChanged { get; set; }

    [Parameter]
    public Func<int, Task>? OnValueChanged { get; set; }

    [Parameter]
    public int Max { get; set; } = 5;

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        StarIcon ??= IconTheme.GetIconByKey(ComponentIcons.RateStarIcon);
        UnStarIcon ??= IconTheme.GetIconByKey(ComponentIcons.RateUnStarIcon);

        if (Max < 1)
        {
            Max = 5;
        }

        if (Value < 1)
        {
            Value = 1;
        }
    }

    private async Task OnClickItem(int value)
    {
        Value = value;
        if (OnValueChanged != null)
        {
            await OnValueChanged(Value);
        }
        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(Value);
        }
        else
        {
            StateHasChanged();
        }
    }
}
