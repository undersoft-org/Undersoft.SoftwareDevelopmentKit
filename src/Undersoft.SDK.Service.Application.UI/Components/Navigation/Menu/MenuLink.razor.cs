using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public sealed partial class MenuLink
{
    private string? ClassString => CssBuilder.Default("nav-link")
        .AddClass(Item.CssClass, !string.IsNullOrEmpty(Item.CssClass))
        .AddClass("active", Parent.DisableNavigation && Item.IsActive && !Item.IsDisabled)
        .AddClass("disabled", Item.IsDisabled)
        .AddClass("expand", Parent.IsVertical && !Item.IsCollapsed)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? MenuArrowClassString => CssBuilder.Default("arrow")
        .AddClass(ArrowIcon, Item.Items.Any())
        .Build();

    private string? HrefString => (Parent.DisableNavigation || Item.IsDisabled || Item.Items.Any() || string.IsNullOrEmpty(Item.Url)) ? "#" : Item.Url.TrimStart('/');

    private string? TargetString => string.IsNullOrEmpty(Item.Target) ? null : Item.Target;

    private bool PreventDefault => HrefString == "#";

    [Parameter]
    [NotNull]
    public MenuItem? Item { get; set; }

    [Parameter]
    public string? ArrowIcon { get; set; }

    [CascadingParameter]
    [NotNull]
    private Menu? Parent { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<Menu>? Localizer { get; set; }

    private NavLinkMatch ItemMatch => string.IsNullOrEmpty(Item.Url) ? NavLinkMatch.All : Item.Match;

    private string? IconString => CssBuilder.Default("menu-icon")
        .AddClass(Item.Icon)
        .Build();

    private string? StyleClassString => Parent.IsVertical
        ? (Item.Indent == 0
            ? null
            : $"padding-left: {Item.Indent * Parent.IndentSize}px;")
        : null;

    public override Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);

        if (Parent == null)
        {
            throw new InvalidOperationException(Localizer["InvalidOperationExceptionMessage"]);
        }

        return base.SetParametersAsync(ParameterView.Empty);
    }
}
