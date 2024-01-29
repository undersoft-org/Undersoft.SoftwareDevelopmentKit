namespace Undersoft.SDK.Service.Application.Components;

public class TreeViewItem<TItem> : TreeNodeBase<TItem>, ICheckableNode<TItem>
{
    public bool ShowLoading { get; set; }

    public CheckboxState CheckedState { get; set; }

    public List<TreeViewItem<TItem>> Items { get; set; } = new();

    IEnumerable<IExpandableNode<TItem>> IExpandableNode<TItem>.Items { get => Items; set => Items = value.OfType<TreeViewItem<TItem>>().ToList(); }

    public TreeViewItem<TItem>? Parent { get; set; }

    IExpandableNode<TItem>? IExpandableNode<TItem>.Parent
    {
        get => Parent;
        set
        {
            Parent = null;
            if (value is TreeViewItem<TItem> item)
            {
                Parent = item;
            }
        }
    }

    public TreeViewItem([DisallowNull] TItem item)
    {
        Value = item;
    }

}
