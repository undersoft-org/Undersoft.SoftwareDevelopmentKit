namespace Undersoft.SDK.Service.Application.Components;

public class TimelineItem
{
    public string? Content { get; set; }

    public string? Description { get; set; }

    public Color Color { get; set; }

    public string? Icon { get; set; }

    public BootstrapDynamicComponent? Component { get; set; }

    internal string? ToNodeClassString() => CssBuilder.Default("timeline-item-node-normal timeline-item-node")
        .AddClass($"bg-{Color.ToDescriptionString()}", Color != Color.None && string.IsNullOrEmpty(Icon))
        .AddClass("is-icon", !string.IsNullOrEmpty(Icon))
        .Build();

    internal string? ToIconClassString() => CssBuilder.Default("timeline-item-icon")
        .AddClass(Icon, !string.IsNullOrEmpty(Icon))
        .AddClass($"text-{Color.ToDescriptionString()}", Color != Color.None)
        .Build();
}
