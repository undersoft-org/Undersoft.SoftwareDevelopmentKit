using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class LogoutLink
{
    [Inject]
    [NotNull]
    private IStringLocalizer<LogoutLink>? Localizer { get; set; }

    [Parameter]
    public string? Icon { get; set; }

    [Parameter]
    [NotNull]
    public string? Text { get; set; }

    [Parameter]
    [NotNull]
    public string? Url { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        Text ??= Localizer[nameof(Text)];
        Icon ??= IconTheme.GetIconByKey(ComponentIcons.LogoutLinkIcon);

        Url ??= "/Account/Logout";
    }
}
