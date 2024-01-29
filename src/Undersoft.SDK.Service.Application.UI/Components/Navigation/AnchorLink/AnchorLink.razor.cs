namespace Undersoft.SDK.Service.Application.Components;

public partial class AnchorLink
{
    [Parameter]
    public string? Text { get; set; }

    [Parameter]
    public string? TooltipText { get; set; }

    [Parameter]
    public string? Icon { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    private string? IconString => CssBuilder.Default("anchor-link-icon")
        .AddClass(Icon)
        .Build();

    private string? ClassString => CssBuilder.Default("anchor-link")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        Icon ??= IconTheme.GetIconByKey(ComponentIcons.AnchorLinkIcon);
    }
}
