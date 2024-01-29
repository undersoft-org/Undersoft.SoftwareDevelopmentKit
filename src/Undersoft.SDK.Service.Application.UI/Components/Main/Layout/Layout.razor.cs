using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Localization;
using System.Reflection;

namespace Undersoft.SDK.Service.Application.Components;

public partial class Layout : IHandlerException, IAsyncDisposable
{
    protected bool IsSmallScreen { get; set; }

    [Parameter]
    public bool IsCollapsed { get; set; }

    [Parameter]
    public EventCallback<bool> IsCollapsedChanged { get; set; }

    [Parameter]
    public bool IsAccordion { get; set; }

    [Parameter]
    public RenderFragment? Header { get; set; }

    [Parameter]
    public RenderFragment? Footer { get; set; }

    [Parameter]
    public string? MenuBarIcon { get; set; }

    [Parameter]
    public RenderFragment? Side { get; set; }

    [Parameter]
    public RenderFragment? NotAuthorized { get; set; }

    [Parameter]
    public RenderFragment? NotFound { get; set; }

    [Parameter]
    [NotNull]
    public string? NotFoundTabText { get; set; }

    [Parameter]
    public string? SideWidth { get; set; }

    [Parameter]
    [NotNull]
    public RenderFragment? Main { get; set; }

    [Parameter]
    public bool IsFullSide { get; set; }

    [Parameter]
    public bool IsPage { get; set; }

    [Parameter]
    public IEnumerable<MenuItem>? Menus { get; set; }

    [Parameter]
    public bool UseTabSet { get; set; }

    [Parameter]
    public bool IsOnlyRenderActiveTab { get; set; }

    [Parameter]
    public bool IsFixedFooter { get; set; }

    [Parameter]
    public bool IsFixedHeader { get; set; }

    [Parameter]
    public bool ShowCollapseBar { get; set; }

    [Parameter]
    public bool ShowFooter { get; set; }

    [Parameter]
    public bool ShowGotoTop { get; set; }

    [Parameter]
    public Func<MenuItem, Task>? OnClickMenu { get; set; }

    [Parameter]
    public Func<bool, Task>? OnCollapsed { get; set; }

    [Parameter]
    public string TabDefaultUrl { get; set; } = "";

    [Parameter]
    public Func<string, Task<bool>>? OnAuthorizing { get; set; }

    [Parameter]
    public string NotAuthorizeUrl { get; set; } = "/Signin";

    [Inject]
    [NotNull]
    private NavigationManager? Navigation { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    private bool SubscribedLocationChangedEvent { get; set; }

    private JSInterop<Layout>? Interop { get; set; }

    private bool IsAuthenticated { get; set; }

    private string? ClassString => CssBuilder.Default("layout")
        .AddClass("has-sidebar", Side != null && IsFullSide)
        .AddClass("is-page", IsPage)
        .AddClass("has-footer", ShowFooter)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? FooterClassString => CssBuilder.Default("layout-footer")
        .AddClass("is-fixed", IsFixedFooter)
        .AddClass("is-collapsed", IsCollapsed)
        .Build();

    private string? HeaderClassString => CssBuilder.Default("layout-header")
        .AddClass("is-fixed", IsFixedHeader)
        .Build();

    private string? SideClassString => CssBuilder.Default("layout-side")
        .AddClass("is-collapsed", IsCollapsed)
        .AddClass("is-fixed-header", IsFixedHeader)
        .AddClass("is-fixed-footer", IsFixedFooter)
        .Build();

    private string? SideStyleString => CssBuilder.Default()
        .AddClass($"width: {SideWidth.ConvertToPercentString()}", !string.IsNullOrEmpty(SideWidth) && SideWidth != "0")
        .Build();

    private string? MainClassString => CssBuilder.Default("layout-main")
        .AddClass("is-collapsed", IsCollapsed)
        .Build();

    private string? CollapseBarClassString => CssBuilder.Default("layout-header-bar")
        .AddClass("is-collapsed", IsCollapsed)
        .Build();

    [Parameter]
    public IEnumerable<string>? ExcludeUrls { get; set; }

    [Parameter]
    [NotNull]
    public IEnumerable<Assembly>? AdditionalAssemblies { get; set; }

    [Parameter]
    [NotNull]
    public string? TooltipText { get; set; }

    [Parameter]
    public Func<string, Task>? OnUpdateAsync { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<Layout>? Localizer { get; set; }

    [CascadingParameter]
    private Task<AuthenticationState>? AuthenticationStateTask { get; set; }

    [Inject]
    private IAuthorizationPolicyProvider? AuthorizationPolicyProvider { get; set; }

    [Inject]
    private IAuthorizationService? AuthorizationService { get; set; }

    private bool IsInit { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (OnAuthorizing != null)
        {
            SubscribedLocationChangedEvent = true;
            Navigation.LocationChanged += Navigation_LocationChanged;
        }

        ErrorLogger?.Register(this);
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (AuthenticationStateTask != null)
        {
            AdditionalAssemblies ??= new[] { Assembly.GetEntryAssembly()! };

            var url = Navigation.ToBaseRelativePath(Navigation.Uri);
            var context = RouteTableFactory.Create(AdditionalAssemblies, url);
            if (context.Handler != null)
            {
                IsAuthenticated = await context.Handler.IsAuthorizedAsync(AuthenticationStateTask, AuthorizationPolicyProvider, AuthorizationService);
            }
        }
        else
        {
            IsAuthenticated = true;
        }

        IsInit = true;
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        TooltipText ??= Localizer[nameof(TooltipText)];
        SideWidth ??= "300";

        MenuBarIcon ??= IconTheme.GetIconByKey(ComponentIcons.LayoutMenuBarIcon);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            Interop = new JSInterop<Layout>(JSRuntime);
            await Interop.InvokeVoidAsync(this, null, "bb_layout", nameof(SetCollapsed));
        }
    }

    protected virtual RenderFragment HandlerMain() => builder =>
    {
        builder.AddContent(0, _errorContent ?? Main);
        _errorContent = null;
    };

    [JSInvokable]
    public void SetCollapsed(int width)
    {
        IsSmallScreen = width < 768;
    }

    public async Task UpdateAsync(string key)
    {
        if (OnUpdateAsync != null)
        {
            await OnUpdateAsync(key);
        }
    }

    private async void Navigation_LocationChanged(object? sender, LocationChangedEventArgs e)
    {
        if (OnAuthorizing != null)
        {
            var auth = await OnAuthorizing(e.Location);
            if (!auth)
            {
                Navigation.NavigateTo(NotAuthorizeUrl, true);
            }
        }
    }

    private async Task CollapseMenu()
    {
        IsCollapsed = !IsCollapsed;
        if (IsCollapsedChanged.HasDelegate)
        {
            await IsCollapsedChanged.InvokeAsync(IsCollapsed);
        }

        if (OnCollapsed != null)
        {
            await OnCollapsed(IsCollapsed);
        }
    }

    private Func<MenuItem, Task> ClickMenu() => async item =>
    {
        if (IsSmallScreen && !item.Items.Any())
        {
            await CollapseMenu();
        }

        if (OnClickMenu != null)
        {
            await OnClickMenu(item);
        }
    };

    private RenderFragment? _errorContent;

    public virtual Task HandlerException(Exception ex, RenderFragment<Exception> errorContent)
    {
        _errorContent = errorContent(ex);
        return Task.CompletedTask;
    }

    protected virtual async ValueTask DisposeAsyncCore(bool disposing)
    {
        if (disposing)
        {
            ErrorLogger?.UnRegister(this);
            if (SubscribedLocationChangedEvent)
            {
                Navigation.LocationChanged -= Navigation_LocationChanged;
            }

            if (Interop != null)
            {
                await Interop.InvokeVoidAsync(this, null, "bb_layout", "dispose");
                Interop.Dispose();
                Interop = null;
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore(true);
        GC.SuppressFinalize(this);
    }
}
