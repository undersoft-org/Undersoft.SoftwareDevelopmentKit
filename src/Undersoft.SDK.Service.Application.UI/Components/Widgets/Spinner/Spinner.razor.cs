namespace Undersoft.SDK.Service.Application.Components;

public partial class Spinner
{
    protected string? ClassString => CssBuilder.Default("spinner")
        .AddClass($"spinner-{SpinnerType.ToDescriptionString()}")
        .AddClass($"text-{Color.ToDescriptionString()}", Color != Color.None)
        .AddClass($"spinner-border-{Size.ToDescriptionString()}", Size != Size.None)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    [Parameter]
    public Color Color { get; set; }

    [Parameter]
    public Size Size { get; set; }

    [Parameter]
    public SpinnerType SpinnerType { get; set; }
}
