namespace Undersoft.SDK.Service.Application.Components;

[ExcludeFromCodeCoverage]
public class StepItem
{
    public string? Title { get; set; }

    public string? Icon { get; set; }

    public string? ErrorIcon { get; set; }

    public StepStatus Status { get; set; }

    public string? Description { get; set; }

    public string? Space { get; set; }

    public RenderFragment? Template { get; set; }
}
