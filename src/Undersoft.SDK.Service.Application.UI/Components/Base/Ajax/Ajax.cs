namespace Undersoft.SDK.Service.Application.Components;

[JSModuleAutoLoader("./_content/Undersoft.Sdk.Service.Application/modules/ajax.js", Relative = false, AutoInvokeInit = false, AutoInvokeDispose = false)]
public class Ajax : ComponentModule
{
    [Inject]
    [NotNull]
    private AjaxService? AjaxService { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        AjaxService.Register(this, InvokeAsync);
        AjaxService.RegisterGoto(this, Goto);
    }

    private Task<string?> InvokeAsync(AjaxOption option) => InvokeAsync<string?>("execute", option);

    private Task Goto(string url) => InvokeVoidAsync("goto", url);

    protected override async ValueTask DisposeAsync(bool disposing)
    {
        if (disposing)
        {
            AjaxService.UnRegister(this);
            AjaxService.UnRegisterGoto(this);
        }

        await base.DisposeAsync(disposing);
    }
}
