namespace Undersoft.SDK.Service.Application.Components;

public class RibbonTabItem : MenuItem
{
    public string? ImageUrl { get; set; }

    public string? GroupName { get; set; }

    public string? Command { get; set; }

    public BootstrapDynamicComponent? Component { get; set; }

    public bool IsDefault { get; set; }
}
