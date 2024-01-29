namespace Undersoft.SDK.Service.Application.Components;

public abstract class TooltipWrapperBase : ComponentModule2
{
    protected virtual string? PlacementString => (!string.IsNullOrEmpty(TooltipText) && TooltipPlacement != Placement.Auto) ? TooltipPlacement.ToDescriptionString() : null;

    protected virtual string? TriggerString => TooltipTrigger == "hover focus" ? null : TooltipTrigger;

    [CascadingParameter]
    protected Tooltip? Tooltip { get; set; }

    [Parameter]
    public string? TooltipText { get; set; }

    [Parameter]
    public Placement TooltipPlacement { get; set; } = Placement.Top;

    [Parameter]
    [NotNull]
    public string? TooltipTrigger { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        TooltipTrigger ??= "hover focus";
    }
}
