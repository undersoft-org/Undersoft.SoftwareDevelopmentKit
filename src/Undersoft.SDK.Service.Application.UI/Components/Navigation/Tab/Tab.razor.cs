using Undersoft.SDK.Service.Application.Extensions;
using Microsoft.Extensions.Localization;
using System.Collections.Concurrent;
using System.Reflection;

namespace Undersoft.SDK.Service.Application.Components;

public partial class Tab : IHandlerException, IDisposable
{
    private bool FirstRender { get; set; } = true;

    private static string? GetContentClassString(TabItem item) => CssBuilder.Default("tabs-body-content")
        .AddClass("d-none", !item.IsActive)
        .Build();

    private string? WrapClassString => CssBuilder.Default("tabs-nav-wrap")
        .AddClass("extend", ShouldShowExtendButtons())
        .Build();

    private string? GetClassString(TabItem item) => CssBuilder.Default("tabs-item")
        .AddClass("active", item.IsActive)
        .AddClass("is-closeable", ShowClose)
        .Build();

    private static string? GetIconClassString(string icon) => CssBuilder.Default()
        .AddClass(icon)
        .Build();

    private ElementReference TabElement { get; set; }

    private string? ClassString => CssBuilder.Default("tabs")
        .AddClass("tabs-card", IsCard)
        .AddClass("tabs-border-card", IsBorderCard)
        .AddClass($"tabs-{Placement.ToDescriptionString()}", Placement == Placement.Top || Placement == Placement.Right || Placement == Placement.Bottom || Placement == Placement.Left)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? StyleString => CssBuilder.Default()
        .AddClass($"height: {Height}px;", Height > 0)
        .Build();

    private readonly List<TabItem> _items = new(50);

    public IEnumerable<TabItem> Items => _items;

    private bool Excluded { get; set; }

    [Parameter]
    public bool IsCard { get; set; }

    [Parameter]
    public bool IsBorderCard { get; set; }

    [Parameter]
    public bool IsOnlyRenderActiveTab { get; set; }

    [Parameter]
    public bool IsLazyLoadTabItem { get; set; }

    [Parameter]
    public int Height { get; set; }

    [Parameter]
    public Placement Placement { get; set; } = Placement.Top;

    [Parameter]
    public bool ShowClose { get; set; }

    [Parameter]
    public Func<TabItem, Task<bool>>? OnCloseTabItemAsync { get; set; }

    [Parameter]
    public bool ShowExtendButtons { get; set; }

    [Parameter]
    public bool ClickTabToNavigation { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public RenderFragment? NotAuthorized { get; set; }

    [Parameter]
    public RenderFragment? NotFound { get; set; }

    [Parameter]
    public RenderFragment? Body { get; set; }

    [Parameter]
    [NotNull]
    public IEnumerable<Assembly>? AdditionalAssemblies { get; set; }

    [Parameter]
    public IEnumerable<string>? ExcludeUrls { get; set; }

    [Parameter]
    public string? DefaultUrl { get; set; }

    [Parameter]
    [Obsolete("OnClickTabItemAsync")]
    [ExcludeFromCodeCoverage]
    public Func<TabItem, Task>? OnClickTab { get; set; }

    [Parameter]
    public Func<TabItem, Task>? OnClickTabItemAsync { get; set; }

    [Parameter]
    [NotNull]
    public string? NotFoundTabText { get; set; }

    [Parameter]
    [NotNull]
    public string? CloseCurrentTabText { get; set; }

    [Parameter]
    [NotNull]
    public string? CloseAllTabsText { get; set; }

    [Parameter]
    [NotNull]
    public string? CloseOtherTabsText { get; set; }

    [Parameter]
    public RenderFragment? ButtonTemplate { get; set; }

    [Parameter]
    public string? PreviousIcon { get; set; }

    [Parameter]
    public string? NextIcon { get; set; }

    [Parameter]
    public string? DropdownIcon { get; set; }

    [Parameter]
    public string? CloseIcon { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<Tab>? Localizer { get; set; }

    [Inject]
    [NotNull]
    private NavigationManager? Navigator { get; set; }

    [Inject]
    [NotNull]
    private TabItemTextOptions? Options { get; set; }

    [Inject]
    [NotNull]
    private IOptionsMonitor<TabItemBindOptions>? TabItemMenuBinder { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    private ConcurrentDictionary<TabItem, bool> LazyTabCache { get; } = new();

    private bool HandlerNavigation { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        ErrorLogger?.Register(this);
    }

    protected override void OnParametersSet()
    {
        if (ShowExtendButtons)
        {
            IsBorderCard = true;
        }

        CloseOtherTabsText ??= Localizer[nameof(CloseOtherTabsText)];
        CloseAllTabsText ??= Localizer[nameof(CloseAllTabsText)];
        CloseCurrentTabText ??= Localizer[nameof(CloseCurrentTabText)];
        NotFoundTabText ??= Localizer[nameof(NotFoundTabText)];

        PreviousIcon ??= IconTheme.GetIconByKey(ComponentIcons.TabPreviousIcon);
        NextIcon ??= IconTheme.GetIconByKey(ComponentIcons.TabNextIcon);
        DropdownIcon ??= IconTheme.GetIconByKey(ComponentIcons.TabDropdownIcon);
        CloseIcon ??= IconTheme.GetIconByKey(ComponentIcons.TabCloseIcon);

        AdditionalAssemblies ??= new[] { Assembly.GetEntryAssembly()! };

        if (ClickTabToNavigation)
        {
            if (!HandlerNavigation)
            {
                HandlerNavigation = true;
                Navigator.LocationChanged += Navigator_LocationChanged;
            }
            AddTabByUrl();
        }
        else
        {
            RemoveLocationChanged();
        }
    }

    private void RemoveLocationChanged()
    {
        if (HandlerNavigation)
        {
            Navigator.LocationChanged -= Navigator_LocationChanged;
            HandlerNavigation = false;
        }
    }

    private void Navigator_LocationChanged(object? sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
    {
        AddTabByUrl();

        StateHasChanged();
    }

    private void AddTabByUrl()
    {
        var requestUrl = Navigator.ToBaseRelativePath(Navigator.Uri);

        var urls = ExcludeUrls ?? Enumerable.Empty<string>();
        Excluded = requestUrl == ""
            ? urls.Any(u => u is "" or "/")
            : urls.Any(u => u != "/" && requestUrl.StartsWith(u.TrimStart('/'), StringComparison.OrdinalIgnoreCase));
        if (!Excluded)
        {
            var tab = Items.FirstOrDefault(tab => tab.Url.TrimStart('/').Equals(requestUrl, StringComparison.OrdinalIgnoreCase));
            if (tab != null)
            {
                ActiveTabItem(tab);
            }
            else
            {
                AddTabItem(requestUrl);
            }
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            FirstRender = false;
        }

        await JSRuntime.InvokeVoidAsync(TabElement, "bb_tab");
    }

    private bool ShouldShowExtendButtons() => ShowExtendButtons && (Placement == Placement.Top || Placement == Placement.Bottom);

    private async Task OnClickTabItem(TabItem item)
    {
        if (OnClickTabItemAsync != null)
        {
            await OnClickTabItemAsync(item);
        }

        if (!ClickTabToNavigation)
        {
            Items.ToList().ForEach(i => i.SetActive(false));
            item.SetActive(true);
            StateHasChanged();
        }
    }

    public Task ClickPrevTab()
    {
        var item = Items.FirstOrDefault(i => i.IsActive);
        if (item != null)
        {
            var index = _items.IndexOf(item);
            if (index > -1)
            {
                index--;
                if (index < 0)
                {
                    index = _items.Count - 1;
                }

                if (!ClickTabToNavigation)
                {
                    item.SetActive(false);
                }

                item = Items.ElementAt(index);
                if (ClickTabToNavigation)
                {
                    Navigator.NavigateTo(item.Url);
                }
                else
                {
                    item.SetActive(true);
                    StateHasChanged();
                }
            }
        }
        return Task.CompletedTask;
    }

    public Task ClickNextTab()
    {
        var item = Items.FirstOrDefault(i => i.IsActive);
        if (item != null)
        {
            var index = _items.IndexOf(item);
            if (index < _items.Count)
            {
                if (!ClickTabToNavigation)
                {
                    item.SetActive(false);
                }

                index++;
                if (index + 1 > _items.Count)
                {
                    index = 0;
                }

                item = Items.ElementAt(index);

                if (ClickTabToNavigation)
                {
                    Navigator.NavigateTo(item.Url);
                }
                else
                {
                    item.SetActive(true);
                    StateHasChanged();
                }
            }
        }
        return Task.CompletedTask;
    }

    public async Task CloseCurrentTab()
    {
        var tab = _items.FirstOrDefault(t => t.IsActive);
        if (tab is { Closable: true })
        {
            await RemoveTab(tab);
        }
    }

    private void OnClickCloseAllTabs() => _items.RemoveAll(t => t.Closable);

    public void CloseAllTabs()
    {
        OnClickCloseAllTabs();
        StateHasChanged();
    }

    private void OnClickCloseOtherTabs() => _items.RemoveAll(t => t is { Closable: true, IsActive: false });

    public void CloseOtherTabs()
    {
        OnClickCloseOtherTabs();
        StateHasChanged();
    }

    internal void AddItem(TabItem item) => _items.Add(item);

    public void AddTab(string url, string text, string? icon = null, bool active = true, bool closable = true)
    {
        Options.Text = text;
        Options.Icon = icon;
        Options.IsActive = active;
        Options.Closable = closable;

        AddTabItem(url);
        StateHasChanged();
    }

    private void AddTabItem(string url)
    {
        var parameters = new Dictionary<string, object?>();
        var context = RouteTableFactory.Create(AdditionalAssemblies, url);
        if (context.Handler != null)
        {
            var option = context.Handler.GetCustomAttribute<TabItemOptionAttribute>(false)
                ?? TabItemMenuBinder.CurrentValue.Binders
                    .FirstOrDefault(i => i.Key.TrimStart('/').Equals(url.TrimStart('/'), StringComparison.OrdinalIgnoreCase))
                    .Value;
            if (option != null)
            {
                parameters.Add(nameof(TabItem.Icon), option.Icon);
                parameters.Add(nameof(TabItem.Closable), option.Closable);
                parameters.Add(nameof(TabItem.IsActive), true);
                parameters.Add(nameof(TabItem.Text), option.Text);
            }
            else if (Options.Valid())
            {
                parameters.Add(nameof(TabItem.Icon), Options.Icon);
                parameters.Add(nameof(TabItem.Closable), Options.Closable);
                parameters.Add(nameof(TabItem.IsActive), Options.IsActive);
                parameters.Add(nameof(TabItem.Text), Options.Text);
                Options.Reset();
            }
            else
            {
                parameters.Add(nameof(TabItem.Text), url.Split("/").FirstOrDefault());
            }
            parameters.Add(nameof(TabItem.Url), url);

            parameters.Add(nameof(TabItem.ChildContent), new RenderFragment(builder =>
            {
                builder.OpenComponent<PresenterAuthorizeView>(0);
                builder.AddAttribute(1, nameof(PresenterAuthorizeView.Type), context.Handler);
                builder.AddAttribute(2, nameof(PresenterAuthorizeView.Parameters), context.Parameters);
                builder.AddAttribute(3, nameof(PresenterAuthorizeView.NotAuthorized), NotAuthorized);
                builder.CloseComponent();
            }));
        }
        else
        {
            parameters.Add(nameof(TabItem.Text), NotFoundTabText);
            parameters.Add(nameof(TabItem.ChildContent), new RenderFragment(builder =>
            {
                builder.AddContent(0, NotFound);
            }));
        }

        AddTabItem(parameters);
    }

    public void AddTab(Dictionary<string, object?> parameters, int? index = null)
    {
        AddTabItem(parameters, index);
        StateHasChanged();
    }

    private void AddTabItem(Dictionary<string, object?> parameters, int? index = null)
    {
        var item = TabItem.Create(parameters);
        item.TabSet = this;
        if (item.IsActive)
        {
            _items.ForEach(i => i.SetActive(false));
        }

        if (index.HasValue)
        {
            _items.Insert(index.Value, item);
        }
        else
        {
            _items.Add(item);
        }
    }

    public async Task RemoveTab(TabItem item)
    {
        if (OnCloseTabItemAsync != null && !await OnCloseTabItemAsync(item))
        {
            return;
        }

        var index = _items.IndexOf(item);
        _items.Remove(item);

        var activeItem = _items.FirstOrDefault(i => i.IsActive)
                         ?? (index < _items.Count ? _items[index] : _items.LastOrDefault());
        if (activeItem != null)
        {
            if (ClickTabToNavigation)
            {
                Navigator.NavigateTo(activeItem.Url);
            }
            else
            {
                activeItem.SetActive(true);
                StateHasChanged();
            }
        }
        else
        {
            StateHasChanged();
        }
    }

    public void ActiveTab(TabItem item)
    {
        ActiveTabItem(item);

        StateHasChanged();
    }

    public void ActiveTab(int index)
    {
        var item = _items.ElementAtOrDefault(index);
        if (item != null)
        {
            ActiveTab(item);
        }
    }

    public TabItem? GetActiveTab() => _items.FirstOrDefault(s => s.IsActive);

    private void ActiveTabItem(TabItem item)
    {
        _items.ForEach(i => i.SetActive(false));
        item.SetActive(true);
    }

    private RenderFragment RenderTabItemContent(TabItem item) => builder =>
    {
        if (item.IsActive)
        {
            var content = _errorContent ?? item.ChildContent;
            builder.AddContent(0, content);
            _errorContent = null;
            if (IsLazyLoadTabItem)
            {
                LazyTabCache.AddOrUpdate(item, _ => true, (_, _) => true);
            }
        }
        else if (!IsLazyLoadTabItem || item.AlwaysLoad || LazyTabCache.TryGetValue(item, out var init) && init)
        {
            builder.AddContent(0, item.ChildContent);
        }
    };

    private RenderFragment? _errorContent;

    public virtual Task HandlerException(Exception ex, RenderFragment<Exception> errorContent)
    {
        _errorContent = errorContent(ex);
        return Task.CompletedTask;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            RemoveLocationChanged();
            ErrorLogger?.UnRegister(this);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
