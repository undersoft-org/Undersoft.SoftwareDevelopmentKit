namespace Undersoft.SDK.Service.Application.Components;

public interface ITooltip
{
    Placement Placement { get; set; }

    string? Title { get; set; }

    bool IsHtml { get; set; }

    string? Trigger { get; set; }

    string? CustomClass { get; set; }

    string? Delay { get; set; }

    bool Sanitize { get; set; }

    string? Selector { get; set; }
}
