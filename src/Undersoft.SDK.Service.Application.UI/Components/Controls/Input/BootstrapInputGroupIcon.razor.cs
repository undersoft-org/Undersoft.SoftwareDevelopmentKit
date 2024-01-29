namespace Undersoft.SDK.Service.Application.Components;

public partial class BootstrapInputGroupIcon
{
    private string? ClassString => CssBuilder.Default("Input-group-text")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    [Parameter]
#if NET6_0_OR_GREATER
    [EditorRequired]
#endif
    public string? Icon { get; set; }
}
