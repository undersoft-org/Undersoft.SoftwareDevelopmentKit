namespace Undersoft.SDK.Service.Application.Components;

[JSModuleAutoLoader("base/utility")]
public class Clipboard : ComponentModule2
{
    [Inject]
    [NotNull]
    private ClipboardService? ClipboardService { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        ClipboardService.Register(this, Copy);
    }

    protected override Task ModuleInvokeVoidAsync(bool firstRender) => Task.CompletedTask;

    private async Task Copy(ClipboardOption option)
    {
        if (Module != null)
        {
            await Module.InvokeVoidAsync("copy", option.Text);
        }
        if (option.Callback != null)
        {
            await option.Callback();
        }
    }

    protected override async ValueTask DisposeAsync(bool disposing)
    {
        if (disposing)
        {
            ClipboardService.UnRegister(this);
        }
        await base.DisposeAsync(disposing);
    }
}
