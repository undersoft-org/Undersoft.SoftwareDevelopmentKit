namespace Undersoft.SDK.Service.Application.Components;

public interface IExpandableNode<TItem>
{
    public bool IsExpand { get; set; }

    public bool HasChildren { get; set; }

    [DisallowNull]
    [NotNull]
    IEnumerable<IExpandableNode<TItem>>? Items { get; set; }

    [DisallowNull]
    [NotNull]
    TItem? Value { get; set; }

    IExpandableNode<TItem>? Parent { get; set; }
}
