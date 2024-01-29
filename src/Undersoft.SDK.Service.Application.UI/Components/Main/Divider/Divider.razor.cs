namespace Undersoft.SDK.Service.Application.Components;

public partial class Divider
{
    protected virtual string? ClassString => CssBuilder.Default("divider")
        .AddClass("divider-vertical", IsVertical)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    protected virtual string? TextClassString => CssBuilder.Default("divider-text")
        .AddClass("is-left", Alignment.Left == Alignment)
        .AddClass("is-center", Alignment.Center == Alignment)
        .AddClass("is-right", Alignment.Right == Alignment)
        .Build();

    [Parameter]
    public bool IsVertical { get; set; }

    [Parameter]
    public Alignment Alignment { get; set; } = Alignment.Center;

    [Parameter]
    public string? Text { get; set; }

    [Parameter]
    public string? Icon { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
