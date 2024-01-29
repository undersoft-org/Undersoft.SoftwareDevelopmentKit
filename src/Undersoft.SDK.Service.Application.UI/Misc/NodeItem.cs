namespace Undersoft.SDK.Service.Application.Components;

public abstract class NodeItem
{
    public string? Id { get; set; }

    public string? ParentId { get; set; }

    public string? Text { get; set; }

    public string? Icon { get; set; }

    public string? CssClass { get; set; }

    public bool IsDisabled { get; set; }

    public bool IsActive { get; set; }

    public bool IsCollapsed { get; set; } = true;

    public RenderFragment? Template { get; set; }
}
