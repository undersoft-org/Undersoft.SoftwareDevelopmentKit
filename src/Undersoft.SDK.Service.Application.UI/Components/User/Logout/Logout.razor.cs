using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class Logout
{
    private string? LogoutClassString => CssBuilder.Default("dropdown dropdown-logout")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    [Parameter]
    public string? ImageUrl { get; set; }

    [Parameter]
    public string? DisplayName { get; set; }

    [Parameter]
    public string? PrefixDisplayNameText { get; set; }

    [Parameter]
    public string? UserName { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public bool ShowUserName { get; set; } = true;

    [Parameter]
    public string? PrefixUserNameText { get; set; }

    [Parameter]
    public RenderFragment? HeaderTemplate { get; set; }

    [Parameter]
    public RenderFragment? LinkTemplate { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<Logout>? Localizer { get; set; }

    protected override void OnInitialized()
    {
        PrefixDisplayNameText ??= Localizer[nameof(PrefixDisplayNameText)];
        PrefixUserNameText ??= Localizer[nameof(PrefixUserNameText)];
    }
}
