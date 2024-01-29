namespace Undersoft.SDK.Service.Application.Components;

public abstract class NodeBase
{
    public bool IsExpand { get; set; }

    public bool HasChildren { get; set; }
}

public abstract class NodeBase<TItem> : NodeBase
{
    [NotNull]
    public TItem? Value { get; set; }
}
