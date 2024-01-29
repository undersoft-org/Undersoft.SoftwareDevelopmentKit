namespace Undersoft.SDK.Service.Application.Components;

public partial class Badge
{
    protected string? ClassString => CssBuilder.Default("badge")
        .AddClass($"bg-{Color.ToDescriptionString()}", Color != Color.None)
        .AddClass("rounded-pill", IsPill)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    [Parameter] public Color Color { get; set; } = Color.Primary;

    [Parameter] public bool IsPill { get; set; }

    [Parameter] public RenderFragment? ChildContent { get; set; }
}
