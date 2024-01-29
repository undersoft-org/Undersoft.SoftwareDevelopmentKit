namespace Undersoft.SDK.Service.Application.Components;

public class DropdownWidgetItem : Component
{
    [Parameter]
    public string? Icon { get; set; }

    [Parameter]
    public string? Title { get; set; }

    [Parameter]
    public Color BadgeColor { get; set; } = Color.Success;

    [Parameter]
    public Color HeaderColor { get; set; } = Color.Primary;

    [Parameter]
    public string? BadgeNumber { get; set; }

    [Parameter]
    public bool ShowArrow { get; set; } = true;

    [Parameter]
    public RenderFragment? HeaderTemplate { get; set; }

    [Parameter]
    public RenderFragment? BodyTemplate { get; set; }

    [Parameter]
    public RenderFragment? FooterTemplate { get; set; }

    [CascadingParameter]
    private DropdownWidget? Container { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Container?.Add(this);
    }
}
