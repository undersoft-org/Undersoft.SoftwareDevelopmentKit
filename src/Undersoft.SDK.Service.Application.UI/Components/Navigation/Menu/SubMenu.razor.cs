using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public sealed partial class SubMenu
{
    private string? ClassString => CssBuilder.Default("has-leaf nav-link")
        .AddClass("active", Item.IsActive)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? GetIconString => CssBuilder.Default("menu-icon")
        .AddClass(Item.Icon)
        .Build();

    private string? DropdownIconString => CssBuilder.Default("nav-link-right")
        .AddClass(DropdownIcon)
        .Build();

    [Parameter]
    [NotNull]
    public MenuItem? Item { get; set; }

    [Parameter]
    public string? DropdownIcon { get; set; }

    [Parameter]
    public string? ArrowIcon { get; set; }

    [Parameter]
    public Func<MenuItem, Task>? OnClick { get; set; }

    [CascadingParameter]
    [NotNull]
    private Menu? Parent { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<Menu>? Localizer { get; set; }

    private static string? GetClassString(MenuItem item) => CssBuilder.Default()
        .AddClass("active", !item.IsDisabled && item.IsActive)
        .AddClass("disabled", item.IsDisabled)
        .Build();

    public override Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);

        if (Parent == null)
        {
            throw new InvalidOperationException(Localizer["InvalidOperationExceptionMessage"]);
        }

        return base.SetParametersAsync(ParameterView.Empty);
    }

    private async Task OnClickItem(MenuItem item)
    {
        if (OnClick != null)
        {
            await OnClick(item);
        }
    }
}
