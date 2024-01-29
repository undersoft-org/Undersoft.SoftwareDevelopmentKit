namespace Undersoft.SDK.Service.Application.Components;

public partial class Table<TItem>
{
    [Parameter]
    public bool IsTree { get; set; }

    [Parameter]
    public Func<IEnumerable<TItem>, Task<IEnumerable<TableTreeNode<TItem>>>>? TreeNodeConverter { get; set; }

    [Parameter]
    public Func<TItem, Task<IEnumerable<TableTreeNode<TItem>>>>? OnTreeExpand { get; set; }

    [NotNull]
    private List<TableTreeNode<TItem>> TreeRows { get; } = new(100);

    private bool IsLoadChildren { get; set; }

    [NotNull]
    private string? NotSetOnTreeExpandErrorMessage { get; set; }

    [Parameter]
    public string? TreeIcon { get; set; }

    [Parameter]
    public string? TreeExpandIcon { get; set; }

    [Parameter]
    public string? TreeNodeLoadingIcon { get; set; }

    [Parameter]
    public int IndentSize { get; set; } = 16;

    protected string? GetTreeStyleString(int degree) => CssBuilder.Default()
        .AddClass($"margin-left: {degree * IndentSize}px;")
        .Build();

    protected string? GetTreeClassString(bool isExpand) => CssBuilder.Default("is-tree")
        .AddClass(TreeIcon, !IsLoadChildren && !isExpand)
        .AddClass(TreeExpandIcon, !IsLoadChildren && isExpand)
        .AddClass(TreeNodeLoadingIcon, IsLoadChildren)
        .Build();

    [NotNull]
    protected ExpandableNodeCache<TableTreeNode<TItem>, TItem>? TreeNodeCache { get; set; }

    protected Func<Task> ToggleTreeRow(TItem item) => async () =>
    {
        if (!IsLoadChildren)
        {
            if (TreeNodeCache.TryFind(TreeRows, item, out var node))
            {
                IsLoadChildren = true;
                node.IsExpand = !node.IsExpand;
                await TreeNodeCache.ToggleNodeAsync(node, GetChildrenRowAsync);
                IsLoadChildren = false;

                RowsCache = null;

                StateHasChanged();
            }
        }
    };

    private async Task<IEnumerable<IExpandableNode<TItem>>> GetChildrenRowAsync(TableTreeNode<TItem> node)
    {
        if (OnTreeExpand == null)
        {
            throw new InvalidOperationException(NotSetOnTreeExpandErrorMessage);
        }
        return await OnTreeExpand(node.Value);
    }
}
