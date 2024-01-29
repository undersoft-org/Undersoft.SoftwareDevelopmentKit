namespace Undersoft.SDK.Service.Application.Components;

public abstract class TreeNodeBase<TItem> : NodeBase<TItem>
{
    public string? Text { get; set; }

    public string? Icon { get; set; }

    public string? ExpandIcon { get; set; }

    public string? CssClass { get; set; }

    public bool IsDisabled { get; set; }

    public bool IsActive { get; set; }

    public RenderFragment<TItem>? Template { get; set; }
}
