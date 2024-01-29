namespace Undersoft.SDK.Service.Application.Components;

[JSModuleAutoLoader("./_content/BootstrapBlazor/modules/fullscreen.js", Relative = false)]
public class FullScreen : ComponentModule
{
    [Inject]
    [NotNull]
    private FullScreenService? FullScreenService { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        FullScreenService.Register(this, Show);
    }

    private FullScreenOption? Option { get; set; }

    private async Task Show(FullScreenOption option)
    {
        Option = option;

        await InvokeVoidAsync("execute", Id, Option.Element.Context != null ? option.Element : Option.Id);
    }

    protected override async ValueTask DisposeAsync(bool disposing)
    {
        if (disposing)
        {
            FullScreenService.UnRegister(this);
        }

        await base.DisposeAsync(disposing);
    }
}
