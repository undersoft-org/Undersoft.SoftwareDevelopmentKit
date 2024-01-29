using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class TopMenu
{
    private string? GetDropdownClassString(MenuItem item, string className = "") => CssBuilder.Default(className)
        .AddClass("dropdown", string.IsNullOrEmpty(className) && !Parent.IsBottom)
        .AddClass("dropup", string.IsNullOrEmpty(className) && Parent.IsBottom)
        .AddClass("disabled", item.IsDisabled)
        .AddClass("active", item.IsActive)
        .Build();

    private static string? GetIconString(string icon) => CssBuilder.Default("menu-icon")
        .AddClass(icon)
        .Build();

    [Parameter]
    [NotNull]
    public string? DropdownIcon { get; set; }

    [Parameter]
    [NotNull]
    public string? ArrowIcon { get; set; }

    [Parameter]
    [NotNull]
    public IEnumerable<MenuItem>? Items { get; set; }

    [Parameter]
    public Func<MenuItem, Task>? OnClick { get; set; }

    [CascadingParameter]
    [NotNull]
    private Menu? Parent { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<Menu>? Localizer { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    public override Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);

        if (Parent == null)
        {
            throw new InvalidOperationException(Localizer["InvalidOperationExceptionMessage"]);
        }

        return base.SetParametersAsync(ParameterView.Empty);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        DropdownIcon ??= IconTheme.GetIconByKey(ComponentIcons.TopMenuDropdownIcon);
        ArrowIcon ??= IconTheme.GetIconByKey(ComponentIcons.MenuLinkArrowIcon);
    }

    private async Task OnClickItem(MenuItem item)
    {
        if (OnClick != null)
        {
            await OnClick(item);
        }
    }
}
