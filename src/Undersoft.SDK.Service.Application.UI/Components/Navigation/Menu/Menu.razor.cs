namespace Undersoft.SDK.Service.Application.Components;

public partial class Menu
{
    protected string? ClassString => CssBuilder.Default("menu")
        .AddClass("is-bottom", IsBottom)
        .AddClass("is-vertical", IsVertical)
        .AddClass("is-collapsed", IsVertical && IsCollapsed)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? SideMenuClassString => CssBuilder.Default()
        .AddClass("accordion", IsAccordion)
        .Build();

    private string? ExpandString => (IsVertical && IsExpandAll) ? "true" : null;

    private string SideMenuId => $"{Id}_sub";

    private MenuItem? ActiveMenu { get; set; }

    [Parameter]
    [NotNull]
    public IEnumerable<MenuItem>? Items { get; set; }

    [Parameter]
    public bool IsAccordion { get; set; }

    [Parameter]
    public bool IsExpandAll { get; set; }

    [Parameter]
    public bool IsCollapsed { get; set; }

    [Parameter]
    public bool IsVertical { get; set; }

    [Parameter]
    public bool IsBottom { get; set; }

    [Parameter]
    public int IndentSize { get; set; } = 16;

    [Parameter]
    public bool DisableNavigation { get; set; }

    [Parameter]
    public Func<MenuItem, Task>? OnClick { get; set; }

    [Inject]
    [NotNull]
    private NavigationManager? Navigator { get; set; }

    [Inject]
    [NotNull]
    private TabItemTextOptions? Options { get; set; }

    private bool _isExpandAll;
    private bool _isAccordion;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        _isAccordion = IsAccordion;
        _isExpandAll = IsExpandAll;
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        Items ??= Enumerable.Empty<MenuItem>();
        InitMenus(null, Items, Navigator.ToBaseRelativePath(Navigator.Uri));
        if (!DisableNavigation)
        {
            Options.Text = ActiveMenu?.Text;
            Options.Icon = ActiveMenu?.Icon;
            Options.IsActive = true;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender)
        {
            await InvokeUpdateAsync();
        }
    }

    private async Task InvokeUpdateAsync()
    {
        if (ShouldInvoke() && Module != null)
        {
            _isAccordion = IsAccordion;
            _isExpandAll = IsExpandAll;
            await InvokeVoidAsync("update", Id);
        }

        bool ShouldInvoke() => IsVertical && (_isAccordion != IsAccordion || _isExpandAll != IsExpandAll);
    }

    private void InitMenus(MenuItem? parent, IEnumerable<MenuItem> menus, string url)
    {
        foreach (var item in menus)
        {
            if (parent != null)
            {
                item.Parent = parent;
            }

            item.SetIndent();

            if (!DisableNavigation)
            {
                item.IsActive = false;
            }

            if (item.Items.Any())
            {
                InitMenus(item, item.Items, url);
            }
            else if (!DisableNavigation && (item.Url?.TrimStart('/').Equals(url, StringComparison.OrdinalIgnoreCase) ?? false))
            {
                item.IsActive = true;
            }

            if (item.IsActive)
            {
                ActiveMenu = item;
            }
        }
    }

    private async Task OnClickMenu(MenuItem item)
    {
        if (!item.IsDisabled)
        {
            if (!DisableNavigation && !item.Items.Any())
            {
                Options.Text = item.Text;
                Options.Icon = item.Icon;
                Options.IsActive = true;
            }

            if (OnClick != null)
            {
                await OnClick(item);
            }

            if (DisableNavigation)
            {
                if (IsVertical)
                {
                    if (ActiveMenu != null)
                    {
                        ActiveMenu.IsActive = false;
                    }
                    item.IsActive = true;
                    if (IsCollapsed)
                    {
                        item.CascadingSetActive();
                    }
                }
                else
                {
                    ActiveMenu?.CascadingSetActive(false);
                    item.CascadingSetActive();
                }
                ActiveMenu = item;

                StateHasChanged();
            }
        }
    }
}
