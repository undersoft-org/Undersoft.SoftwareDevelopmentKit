namespace Undersoft.SDK.Service.Application.Components;

public static class ExpandableNodeExtensions
{
    public static List<TItem> GetAllItems<TItem>(this IEnumerable<IExpandableNode<TItem>> items) => items.GetAllItems(new List<TItem>());

    private static List<TItem> GetAllItems<TItem>(this IEnumerable<IExpandableNode<TItem>> items, List<TItem> results)
    {
        foreach (var item in items)
        {
            if (item.Value != null)
            {
                results.Add(item.Value);
                if (item.Items.Any())
                {
                    if (item.IsExpand)
                    {
                        GetAllItems(item.Items, results);
                    }
                }
            }
        }
        return results;
    }

    public static IEnumerable<IExpandableNode<TItem>> GetAllSubItems<TItem>(this IExpandableNode<TItem> item) => item.Items.Concat(GetSubItems(item.Items));

    private static IEnumerable<IExpandableNode<TItem>> GetSubItems<TItem>(IEnumerable<IExpandableNode<TItem>> items) => items.SelectMany(i => i.Items.Any() ? i.Items.Concat(GetSubItems(i.Items)) : i.Items);

    public static IEnumerable<TreeViewItem<TItem>> GetAllTreeSubItems<TItem>(this IExpandableNode<TItem> item) => item.GetAllSubItems().OfType<TreeViewItem<TItem>>();

    public static void SetChildrenCheck<TNode, TItem>(this TNode node, CheckboxState state, TreeNodeCache<TNode, TItem>? cache = null) where TNode : ICheckableNode<TItem>
    {
        foreach (var item in node.Items.OfType<TNode>())
        {
            item.CheckedState = state;
            cache?.ToggleCheck(item);

            if (item.Items.Any())
            {
                item.SetChildrenCheck<TNode, TItem>(state, cache);
            }
        }
    }

    public static void SetParentCheck<TNode, TItem>(this TNode node, CheckboxState state, TreeNodeCache<TNode, TItem>? cache = null) where TNode : ICheckableNode<TItem>
    {
        if (node.Parent is TNode p)
        {
            var nodes = p.Items.OfType<TNode>();
            if (nodes.All(i => i.CheckedState == CheckboxState.Checked))
            {
                p.CheckedState = CheckboxState.Checked;
            }
            else if (nodes.Any(i => i.CheckedState == CheckboxState.Checked || i.CheckedState == CheckboxState.Indeterminate))
            {
                p.CheckedState = CheckboxState.Indeterminate;
            }
            else
            {
                p.CheckedState = CheckboxState.UnChecked;
            }
            cache?.ToggleCheck(p);

            p.SetParentCheck(state, cache);
        }
    }

    public static void SetParentExpand<TNode, TItem>(this TNode node, bool expand) where TNode : IExpandableNode<TItem>
    {
        if (node.Parent is TNode p)
        {
            p.IsExpand = expand;
            p.SetParentExpand<TNode, TItem>(expand);
        }
    }

    public static IEnumerable<TreeViewItem<TItem>> CascadingTree<TItem>(this IEnumerable<TItem> items, TreeViewItem<TItem>? parent, Func<TItem, TreeViewItem<TItem>?, bool> predicate, Func<TItem, TreeViewItem<TItem>> valueFactory) => items
        .Where(i => predicate(i, parent))
        .Select(i =>
        {
            var item = valueFactory(i);
            item.Items = CascadingTree(items, item, predicate, valueFactory).ToList();
            item.Parent = parent;
            return item;
        });
}
