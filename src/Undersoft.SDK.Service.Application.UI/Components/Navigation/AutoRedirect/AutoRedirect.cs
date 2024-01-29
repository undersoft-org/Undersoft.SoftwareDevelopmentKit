namespace Undersoft.SDK.Service.Application.Components;

[JSModuleAutoLoader("./_content/BootstrapBlazor/modules/autoredirect.js", JSObjectReference = true, Relative = false)]
public class AutoRedirect : ComponentModule
{
    [Parameter]
    public string? RedirectUrl { get; set; }

    [Parameter]
    public bool IsForceLoad { get; set; }

    [Parameter]
    public int Interval { get; set; } = 60000;

    [Parameter]
    public Func<Task<bool>>? OnBeforeRedirectAsync { get; set; }

    [Inject]
    [NotNull]
    private NavigationManager? NavigationManager { get; set; }

    protected override Task InvokeInitAsync() => InvokeVoidAsync("init", Id, Interop, Interval, nameof(Lock));

    [JSInvokable]
    public async Task Lock()
    {
        var interrupt = false;
        if (OnBeforeRedirectAsync != null)
        {
            interrupt = await OnBeforeRedirectAsync();
        }
        if (!interrupt && !string.IsNullOrEmpty(RedirectUrl))
        {
            NavigationManager.NavigateTo(RedirectUrl, IsForceLoad);
        }
    }
}
