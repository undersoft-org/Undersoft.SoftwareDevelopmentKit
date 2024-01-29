namespace Undersoft.SDK.Service.Application.Components;

public class TableTreeNode<TItem> : NodeBase<TItem>, IExpandableNode<TItem>
{
    [DisallowNull]
    [NotNull]
    public IEnumerable<TableTreeNode<TItem>>? Items { get; set; } = Enumerable.Empty<TableTreeNode<TItem>>();

    IEnumerable<IExpandableNode<TItem>> IExpandableNode<TItem>.Items { get => Items; set => Items = value.OfType<TableTreeNode<TItem>>(); }

    public TableTreeNode<TItem>? Parent { get; set; }

    IExpandableNode<TItem>? IExpandableNode<TItem>.Parent
    {
        get => Parent;
        set
        {
            Parent = null;
            if (value is TableTreeNode<TItem> item)
            {
                Parent = item;
            }
        }
    }

    public TableTreeNode([DisallowNull] TItem item)
    {
        Value = item;
    }
}
