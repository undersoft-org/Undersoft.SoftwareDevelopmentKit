namespace Undersoft.SDK.Service.Application.Components;

using Undersoft.SDK.Service.Application.Extensions;

[ExcludeFromCodeCoverage]
public partial class Tree
{
    private ElementReference TreeElement { get; set; }

    [NotNull]
    private string? GroupName { get; set; }

    private string? ClassString => CssBuilder.Default("tree")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? LoadingClassString => CssBuilder.Default("table-loading")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private static string? GetIconClassString(TreeItem item) => CssBuilder.Default("tree-icon")
        .AddClass(item.Icon)
        .Build();

    private string? GetCaretClassString(TreeItem item) => CssBuilder.Default("node-icon")
        .AddClass("invisible", !item.HasChildNode && !item.Items.Any())
        .AddClass(NodeIcon, item.IsCollapsed)
        .AddClass(ExpandNodeIcon, !item.IsCollapsed)
        .Build();

    private string? GetItemClassString(TreeItem item) => CssBuilder.Default("tree-item")
        .AddClass("active", ActiveItem == item)
        .Build();

    private static string? GetTreeNodeClassString(TreeItem item) => CssBuilder.Default("tree-ul")
        .AddClass("show", !item.IsCollapsed)
        .Build();

    [Parameter]
    public TreeItem? ActiveItem { get; set; }

    [Parameter]
    public bool IsAccordion { get; set; }

    [Parameter]
    public bool ClickToggleNode { get; set; }

    [Parameter]
    public bool ShowSkeleton { get; set; }

    [Parameter]
    public string? NodeIcon { get; set; }

    [Parameter]
    public string? ExpandNodeIcon { get; set; }

    [Parameter]
    [NotNull]
    public List<TreeItem>? Items { get; set; }

    [Parameter]
    public bool ShowCheckbox { get; set; }

    [Parameter]
    public bool ShowRadio { get; set; }

    [Parameter]
    public bool ShowIcon { get; set; }

    [Parameter]
    public Func<TreeItem, Task>? OnTreeItemClick { get; set; }

    [Parameter]
    public Func<List<TreeItem>, Task>? OnTreeItemChecked { get; set; }

    [Parameter]
    public Func<TreeItem, Task>? OnExpandNode { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        GroupName = this.GetHashCode().ToString();
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        NodeIcon ??= IconTheme.GetIconByKey(ComponentIcons.TreeViewNodeIcon);
        ExpandNodeIcon ??= IconTheme.GetIconByKey(ComponentIcons.TreeViewExpandNodeIcon);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync(TreeElement, "bb_tree");
        }
    }

    private async Task OnClick(TreeItem item)
    {
        ActiveItem = item;
        if (ClickToggleNode)
        {
            await OnExpandRowAsync(item);
        }

        if (OnTreeItemClick != null)
        {
            await OnTreeItemClick(item);
        }

        if (ShowRadio)
        {
            await OnRadioClick(item);
        }
        else if (ShowCheckbox)
        {
            item.Checked = !item.Checked;
            var status = item.Checked ? CheckboxState.Checked : CheckboxState.UnChecked;
            await OnStateChanged(status, item);
        }
    }

    private async Task OnExpandRowAsync(TreeItem item)
    {
        if (IsAccordion)
        {
            foreach (var rootNode in Items.Where(p => !p.IsCollapsed && p != item))
            {
                rootNode.IsCollapsed = true;
            }
        }
        item.IsCollapsed = !item.IsCollapsed;
        if (OnExpandNode != null)
        {
            await OnExpandNode(item);
        }
    }

    private async Task OnStateChanged(CheckboxState state, TreeItem item)
    {
        item.CascadeSetCheck(item.Checked);

        if (OnTreeItemChecked != null)
        {
            await OnTreeItemChecked(GetCheckedItems().ToList());
        }
    }

    public IEnumerable<TreeItem> GetCheckedItems() => Items.Aggregate(new List<TreeItem>(), (t, item) =>
    {
        t.Add(item);
        t.AddRange(item.GetAllSubItems());
        return t;
    }).Where(i => i.Checked);

    private async Task OnRadioClick(TreeItem item)
    {
        if (ActiveItem != null)
        {
            ActiveItem.Checked = false;
        }
        ActiveItem = item;
        ActiveItem.Checked = true;

        if (OnTreeItemChecked != null)
        {
            await OnTreeItemChecked(new List<TreeItem> { item });
        }
    }

    private static CheckboxState CheckState(TreeItem item)
    {
        return item.Checked ? CheckboxState.Checked : CheckboxState.UnChecked;
    }
}
