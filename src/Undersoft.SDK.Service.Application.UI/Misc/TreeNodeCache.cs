namespace Undersoft.SDK.Service.Application.Components;

public class TreeNodeCache<TNode, TItem> : ExpandableNodeCache<TNode, TItem> where TNode : ICheckableNode<TItem>
{
    protected List<TItem> CheckedNodeCache { get; } = new(50);

    protected List<TItem> UncheckedNodeCache { get; } = new(50);

    protected List<TItem> IndeterminateNodeCache { get; } = new(50);

    public TreeNodeCache(Func<TItem, TItem, bool> comparer) : base(comparer)
    {

    }

    public virtual void ToggleCheck(TNode node)
    {
        if (node.CheckedState == CheckboxState.Checked)
        {
            UncheckedNodeCache.RemoveAll(i => EqualityComparer.Equals(i, node.Value));
            IndeterminateNodeCache.RemoveAll(i => EqualityComparer.Equals(i, node.Value));

            if (!CheckedNodeCache.Any(i => EqualityComparer.Equals(i, node.Value)))
            {
                CheckedNodeCache.Add(node.Value);
            }
        }
        else if (node.CheckedState == CheckboxState.UnChecked)
        {
            CheckedNodeCache.RemoveAll(i => EqualityComparer.Equals(i, node.Value));
            IndeterminateNodeCache.RemoveAll(i => EqualityComparer.Equals(i, node.Value));

            if (!UncheckedNodeCache.Any(i => EqualityComparer.Equals(i, node.Value)))
            {
                UncheckedNodeCache.Add(node.Value);
            }
        }
        else
        {
            CheckedNodeCache.RemoveAll(i => EqualityComparer.Equals(i, node.Value));
            UncheckedNodeCache.RemoveAll(i => EqualityComparer.Equals(i, node.Value));

            if (!IndeterminateNodeCache.Any(i => EqualityComparer.Equals(i, node.Value)))
            {
                IndeterminateNodeCache.Add(node.Value);
            }
        }
    }

    private void IsChecked(TNode node)
    {
        var nodes = node.Items.OfType<ICheckableNode<TItem>>();
        if (CheckedNodeCache.Any(i => EqualityComparer.Equals(i, node.Value)))
        {
            node.CheckedState = CheckboxState.Checked;
        }
        else if (UncheckedNodeCache.Contains(node.Value, EqualityComparer))
        {
            node.CheckedState = CheckboxState.UnChecked;
        }
        else if (IndeterminateNodeCache.Contains(node.Value, EqualityComparer))
        {
            node.CheckedState = CheckboxState.Indeterminate;
        }
        CheckChildren(nodes);

        void CheckChildren(IEnumerable<ICheckableNode<TItem>> nodes)
        {
            if (nodes.Any())
            {
                CheckedNodeCache.RemoveAll(i => EqualityComparer.Equals(i, node.Value));
                UncheckedNodeCache.RemoveAll(i => EqualityComparer.Equals(i, node.Value));
                IndeterminateNodeCache.RemoveAll(i => EqualityComparer.Equals(i, node.Value));

                if (nodes.All(i => i.CheckedState == CheckboxState.Checked))
                {
                    node.CheckedState = CheckboxState.Checked;
                    CheckedNodeCache.Add(node.Value);
                }
                else if (nodes.All(i => i.CheckedState == CheckboxState.UnChecked))
                {
                    node.CheckedState = CheckboxState.UnChecked;
                    UncheckedNodeCache.Add(node.Value);
                }
                else
                {
                    node.CheckedState = CheckboxState.Indeterminate;
                    IndeterminateNodeCache.Add(node.Value);
                }
            }
        }
    }

    public void IsChecked(IEnumerable<TNode> nodes)
    {
        if (nodes.Any())
        {
            ResetCheckNodes(nodes);
        }

        void ResetCheckNodes(IEnumerable<TNode> items)
        {
            foreach (var node in items)
            {
                if (node.Items.Any())
                {
                    IsChecked(node.Items.OfType<TNode>());
                }

                IsChecked(node);
            }
        }
    }

    public TNode? FindParentNode(IEnumerable<TNode> nodes, TNode node)
    {
        TNode? ret = default;
        foreach (var treeNode in nodes)
        {
            var subNodes = treeNode.Items.OfType<TNode>();
            if (subNodes.Any(i => EqualityComparer.Equals(i.Value, node.Value)))
            {
                ret = treeNode;
                break;
            }
            if (ret == null && subNodes.Any())
            {
                ret = FindParentNode(subNodes, node);
            }
        }
        return ret;
    }

    public void Reset()
    {
        UncheckedNodeCache.Clear();
        CheckedNodeCache.Clear();
        IndeterminateNodeCache.Clear();
        ExpandedNodeCache.Clear();
        CollapsedNodeCache.Clear();
    }
}
