namespace Undersoft.SDK.Service.Application.Components;

public partial class Anchor
{
    [Parameter]
    public string? Target { get; set; }

    [Parameter]
    public string? Container { get; set; }

    [Parameter]
    public bool IsAnimation { get; set; }

    protected string? AnimationString => IsAnimation ? "true" : null;

    [Parameter]
    public int Offset { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private string? GetTargetString => string.IsNullOrEmpty(Target) ? null : Target;

    private string? ClassString => CssBuilder.Default()
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();
}
