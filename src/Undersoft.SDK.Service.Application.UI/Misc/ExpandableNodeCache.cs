namespace Undersoft.SDK.Service.Application.Components;

public class ExpandableNodeCache<TNode, TItem> where TNode : IExpandableNode<TItem>
{
    protected List<TItem> ExpandedNodeCache { get; } = new(50);

    protected List<TItem> CollapsedNodeCache { get; } = new(50);

    protected IEqualityComparer<TItem> EqualityComparer { get; }

    public ExpandableNodeCache(Func<TItem, TItem, bool> comparer)
    {
        EqualityComparer = new ModelComparer<TItem>(comparer);
    }

    public virtual async Task ToggleNodeAsync(TNode node, Func<TNode, Task<IEnumerable<IExpandableNode<TItem>>>> callback)
    {
        if (node.IsExpand)
        {
            if (!ExpandedNodeCache.Any(i => EqualityComparer.Equals(i, node.Value)))
            {
                ExpandedNodeCache.Add(node.Value);
            }

            CollapsedNodeCache.RemoveAll(i => EqualityComparer.Equals(i, node.Value));

            if (!node.Items.Any())
            {
                var items = await callback(node);
                node.Items = items.ToList();
                ICheckableNode<TItem>? checkNode = null;
                if (node is ICheckableNode<TItem> c)
                {
                    checkNode = c;
                }
                foreach (var n in node.Items)
                {
                    n.Parent = node;
                    if (checkNode != null && n is ICheckableNode<TItem> cn)
                    {
                        cn.CheckedState = checkNode.CheckedState == CheckboxState.Checked ? CheckboxState.Checked : CheckboxState.UnChecked;
                    }
                }
            }
        }
        else
        {
            ExpandedNodeCache.RemoveAll(i => EqualityComparer.Equals(i, node.Value));

            if (!CollapsedNodeCache.Any(i => EqualityComparer.Equals(i, node.Value)))
            {
                CollapsedNodeCache.Add(node.Value);
            }
        }
    }

    public async Task CheckExpandAsync(TNode node, Func<TNode, Task<IEnumerable<IExpandableNode<TItem>>>> callback)
    {
        if (node.IsExpand)
        {
            if (CollapsedNodeCache.Contains(node.Value, EqualityComparer))
            {
                node.IsExpand = false;
            }
            else if (!ExpandedNodeCache.Contains(node.Value, EqualityComparer))
            {
                ExpandedNodeCache.Add(node.Value);
            }
        }
        else
        {
            var needRemove = true;
            if (ExpandedNodeCache.Any(i => EqualityComparer.Equals(i, node.Value)))
            {
                if (node.HasChildren)
                {
                    node.IsExpand = true;
                    needRemove = false;
                    if (!node.Items.Any())
                    {
                        var items = await callback(node);
                        node.Items = items.ToList();
                        foreach (var n in node.Items)
                        {
                            n.Parent = node;
                        }
                    }
                }
            }
            if (needRemove)
            {
                ExpandedNodeCache.RemoveAll(i => EqualityComparer.Equals(i, node.Value));
            }
        }
    }

    public bool TryFind(IEnumerable<TNode> items, TItem target, [MaybeNullWhen(false)] out TNode ret)
    {
        ret = Find(items, target);
        return ret != null;
    }

    private TNode? Find(IEnumerable<TNode> items, TItem target) => Find(items, target, out _);

    public TNode? Find(IEnumerable<TNode> source, TItem target, out int degree)
    {
        degree = -1;
        var ret = source.FirstOrDefault(item => EqualityComparer.Equals(item.Value, target));
        if (ret == null)
        {
            var children = source.SelectMany(e => e.Items.OfType<TNode>());
            if (children.Any())
            {
                ret = Find(children, target, out degree);
            }
        }
        if (ret != null)
        {
            degree++;
        }
        return ret;
    }
}
