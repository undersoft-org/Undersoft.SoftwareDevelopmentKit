namespace Undersoft.SDK.Service.Application.Components;

public abstract class PopoverSelectBase<TValue> : PopoverDropdownBase<TValue>
{
    [Parameter]
    public bool IsPopover { get; set; }

    [Parameter]
    public string? Offset { get; set; }

    protected string? ToggleString => IsPopover ? Constants.DropdownToggleString : "dropdown";

    protected string? OffsetString => IsPopover ? null : Offset;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (IsPopover && Placement == Placement.Auto)
        {
            Placement = Placement.Bottom;
        }

        Offset ??= "[0, 10]";
    }
}
