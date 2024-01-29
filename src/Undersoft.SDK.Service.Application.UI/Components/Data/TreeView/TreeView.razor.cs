using Microsoft.Extensions.Localization;
using Undersoft.SDK.Service.Application.Extensions;

namespace Undersoft.SDK.Service.Application.Components;

#if NET6_0_OR_GREATER
[CascadingTypeParameter(nameof(TItem))]
#endif
public partial class TreeView<TItem> : IModelEqualityComparer<TItem>
{
    private ElementReference TreeElement { get; set; }

    private string? ClassString => CssBuilder.Default("tree-view")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? LoadingClassString => CssBuilder.Default("table-loading")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private static string? GetIconClassString(TreeViewItem<TItem> item) => CssBuilder.Default("tree-icon")
        .AddClass(item.Icon)
        .AddClass(item.ExpandIcon, item.IsExpand && !string.IsNullOrEmpty(item.ExpandIcon))
        .Build();

    private string? GetCaretClassString(TreeViewItem<TItem> item) => CssBuilder.Default("node-icon")
        .AddClass("visible", item.HasChildren || item.Items.Any())
        .AddClass(NodeIcon, !item.IsExpand)
        .AddClass(ExpandNodeIcon, item.IsExpand)
        .Build();

    private string? GetItemClassString(TreeViewItem<TItem> item) => CssBuilder.Default("tree-item")
        .AddClass("active", ActiveItem == item)
        .AddClass("disabled", item.IsDisabled)
        .Build();

    private static string? GetTreeClassString(TreeViewItem<TItem> item) => CssBuilder.Default("tree-ul")
        .AddClass("show", item.IsExpand)
        .Build();

    private static string? GetNodeClassString(TreeViewItem<TItem> item) => CssBuilder.Default("tree-node")
        .AddClass("disabled", item.IsDisabled)
        .Build();

    private static bool TriggerNodeArrow(TreeViewItem<TItem> item) => !item.IsDisabled && (item.HasChildren || item.Items.Any());

    private static bool TriggerNodeLabel(TreeViewItem<TItem> item) => !item.IsDisabled;

    private TreeViewItem<TItem>? ActiveItem { get; set; }

    [Parameter]
    public bool IsAccordion { get; set; }

    [Parameter]
    public bool ClickToggleNode { get; set; }

    [Parameter]
    public bool ClickToggleCheck { get; set; }

    [Parameter]
    public bool ShowSkeleton { get; set; }

    [Parameter]
    public bool IsReset { get; set; }

    [Parameter]
    [NotNull]
    public List<TreeViewItem<TItem>>? Items { get; set; }

    [Parameter]
    public bool ShowCheckbox { get; set; }

    [Parameter]
    public bool ShowIcon { get; set; }

    [Parameter]
    public Func<TreeViewItem<TItem>, Task>? OnTreeItemClick { get; set; }

    [Parameter]
    public Func<List<TreeViewItem<TItem>>, Task>? OnTreeItemChecked { get; set; }

    [Parameter]
    public Func<TreeViewItem<TItem>, Task<IEnumerable<TreeViewItem<TItem>>>>? OnExpandNodeAsync { get; set; }

    [Parameter]
    public Type CustomKeyAttribute { get; set; } = typeof(KeyAttribute);

    [Parameter]
    public Func<TItem, TItem, bool>? ModelEqualityComparer { get; set; }

    [Parameter]
    public string? NodeIcon { get; set; }

    [Parameter]
    public string? ExpandNodeIcon { get; set; }

    [NotNull]
    private string? NotSetOnTreeExpandErrorMessage { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<TreeView<TItem>>? Localizer { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    [NotNull]
    protected TreeNodeCache<TreeViewItem<TItem>, TItem>? TreeNodeStateCache { get; set; }

    [Parameter]
    public bool AutoCheckChildren { get; set; }

    [Parameter]
    public bool AutoCheckParent { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        TreeNodeStateCache ??= new(Equals);
        NotSetOnTreeExpandErrorMessage = Localizer[nameof(NotSetOnTreeExpandErrorMessage)];
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        NodeIcon ??= IconTheme.GetIconByKey(ComponentIcons.TreeViewNodeIcon);
        ExpandNodeIcon ??= IconTheme.GetIconByKey(ComponentIcons.TreeViewExpandNodeIcon);
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Items != null)
        {
            if (IsReset)
            {
                TreeNodeStateCache.Reset();
            }
            else
            {
                if (Items.Any())
                {
                    await CheckExpand(Items);
                }

                if (ShowCheckbox && (AutoCheckParent || AutoCheckChildren))
                {
                    TreeNodeStateCache.IsChecked(Items);
                }

                if (ActiveItem != null)
                {
                    ActiveItem = TreeNodeStateCache.Find(Items, ActiveItem.Value, out _);
                }
            }

            ActiveItem ??= Items.FirstOrDefaultActiveItem();
            ActiveItem?.SetParentExpand<TreeViewItem<TItem>, TItem>(true);

            async Task CheckExpand(IEnumerable<TreeViewItem<TItem>> nodes)
            {
                foreach (var node in nodes)
                {
                    await TreeNodeStateCache.CheckExpandAsync(node, GetChildrenRowAsync);

                    if (node.Items.Any())
                    {
                        await CheckExpand(node.Items);
                    }
                }
            }
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync(TreeElement, "bb_tree");
        }
    }

    private async Task<IEnumerable<IExpandableNode<TItem>>> GetChildrenRowAsync(TreeViewItem<TItem> node)
    {
        if (OnExpandNodeAsync == null)
        {
            throw new InvalidOperationException(NotSetOnTreeExpandErrorMessage);
        }
        node.ShowLoading = true;

        StateHasChanged();

        var ret = await OnExpandNodeAsync(node);
        node.ShowLoading = false;
        return ret;
    }

    private async Task OnClick(TreeViewItem<TItem> item)
    {
        ActiveItem = item;
        if (ClickToggleNode && TriggerNodeArrow(item))
        {
            await OnToggleNodeAsync(item);
        }

        if (OnTreeItemClick != null)
        {
            await OnTreeItemClick(item);
        }

        if (ShowCheckbox && ClickToggleCheck)
        {
            await OnCheckStateChanged(item);
        }

        StateHasChanged();
    }

    private static CheckboxState ToggleCheckState(CheckboxState state) => state switch
    {
        CheckboxState.Checked => CheckboxState.UnChecked,
        _ => CheckboxState.Checked
    };

    private async Task OnToggleNodeAsync(TreeViewItem<TItem> node, bool shouldRender = false)
    {
        node.IsExpand = !node.IsExpand;
        if (IsAccordion)
        {
            await TreeNodeStateCache.ToggleNodeAsync(node, GetChildrenRowAsync);

            if (node.IsExpand)
            {
                var nodes = TreeNodeStateCache.FindParentNode(Items, node)?.Items ?? Items;
                foreach (var n in nodes)
                {
                    if (n != node)
                    {
                        n.IsExpand = false;
                        await TreeNodeStateCache.ToggleNodeAsync(n, GetChildrenRowAsync);
                    }
                }
            }
        }
        else
        {
            await TreeNodeStateCache.ToggleNodeAsync(node, GetChildrenRowAsync);
        }

        if (shouldRender)
        {
            StateHasChanged();
        }
    }

    private async Task OnCheckStateChanged(TreeViewItem<TItem> item, bool shouldRender = false)
    {
        item.CheckedState = ToggleCheckState(item.CheckedState);

        if (AutoCheckChildren)
        {
            item.SetChildrenCheck<TreeViewItem<TItem>, TItem>(item.CheckedState, TreeNodeStateCache);
        }

        if (AutoCheckParent)
        {
            item.SetParentCheck(item.CheckedState, TreeNodeStateCache);
        }

        TreeNodeStateCache.ToggleCheck(item);

        if (OnTreeItemChecked != null)
        {
            await OnTreeItemChecked(GetCheckedItems().ToList());
        }

        if (shouldRender)
        {
            StateHasChanged();
        }
    }

    public void ClearCheckedItems()
    {
        Items.ForEach(item =>
        {
            item.CheckedState = CheckboxState.UnChecked;
            TreeNodeStateCache.ToggleCheck(item);
            item.GetAllTreeSubItems().ToList().ForEach(s =>
            {
                s.CheckedState = CheckboxState.UnChecked;
                TreeNodeStateCache.ToggleCheck(s);
            });
            StateHasChanged();
        });
    }

    public IEnumerable<TreeViewItem<TItem>> GetCheckedItems() => Items.Aggregate(new List<TreeViewItem<TItem>>(), (t, item) =>
    {
        t.Add(item);
        t.AddRange(item.GetAllSubItems().OfType<TreeViewItem<TItem>>());
        return t;
    }).Where(i => i.CheckedState == CheckboxState.Checked);

    public bool Equals(TItem? x, TItem? y) => this.Equals<TItem>(x, y);
}
