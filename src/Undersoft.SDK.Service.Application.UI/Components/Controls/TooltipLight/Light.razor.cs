namespace Undersoft.SDK.Service.Application.Components;

public partial class Light
{
    protected string? ClassString => CssBuilder.Default("light")
        .AddClass("flash", IsFlash)
        .AddClass($"light-{Color.ToDescriptionString()}", Color != Color.None)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    [Parameter]
    public bool IsFlash { get; set; }

    [Parameter]
    public Color Color { get; set; } = Color.Success;
}
