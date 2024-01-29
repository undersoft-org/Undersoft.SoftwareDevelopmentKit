namespace Undersoft.SDK.Service.Application.Components;

public partial class Scroll
{
    private string? ClassString => CssBuilder.Default("scroll")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? StyleString => CssBuilder.Default()
        .AddClass($"height: {Height};", !string.IsNullOrEmpty(Height))
        .AddStyleFromAttributes(AdditionalAttributes)
        .Build();

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public string? Height { get; set; }
}
