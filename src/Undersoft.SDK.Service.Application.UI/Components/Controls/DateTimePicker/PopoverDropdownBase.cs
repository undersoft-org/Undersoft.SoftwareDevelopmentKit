namespace Undersoft.SDK.Service.Application.Components;

public abstract class PopoverDropdownBase<TValue> : ValidateBase<TValue>
{
    [Parameter]
    public Placement Placement { get; set; }

    [Parameter]
    public string? CustomClass { get; set; }

    [Parameter]
    public bool ShowShadow { get; set; } = true;

    protected string? PlacementString => Placement == Placement.Auto ? null : Placement.ToDescriptionString();

    protected virtual string? CustomClassString => CssBuilder.Default(CustomClass)
        .AddClass("shadow", ShowShadow)
        .Build();
}
