using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class SideMenu
{
    private string? GetMenuClassString => CssBuilder.Default("submenu")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? ParentIdString => Parent.IsAccordion ? $"#{Id}" : null;

    private string? GetTargetIdString(MenuItem item) => $"#{GetTargetId(item)}";

    private string GetTargetId(MenuItem item) => ComponentIdGenerator.Generate(item);

    [Parameter]
    [NotNull]
    public IEnumerable<MenuItem>? Items { get; set; }

    [Parameter]
    [NotNull]
    public string? DropdownIcon { get; set; }

    [Parameter]
    [NotNull]
    public string? ArrowIcon { get; set; }

    [Parameter]
    [NotNull]
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

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (Parent == null)
        {
            throw new InvalidOperationException(Localizer["InvalidOperationExceptionMessage"]);
        }
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        DropdownIcon ??= IconTheme.GetIconByKey(ComponentIcons.SideMenuDropdownIcon);
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
