namespace Undersoft.SDK.Service.Application.Components;

public sealed partial class GroupBox
{
    private string? ClassString => CssBuilder.Default("groupbox")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public string? Title { get; set; }
}
