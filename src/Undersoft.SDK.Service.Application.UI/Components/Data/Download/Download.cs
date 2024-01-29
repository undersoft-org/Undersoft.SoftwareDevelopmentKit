namespace Undersoft.SDK.Service.Application.Components;

[JSModuleAutoLoader("./_content/BootstrapBlazor/modules/download.js", Relative = false, AutoInvokeInit = false, AutoInvokeDispose = false)]
public class Download : ComponentModule
{
    [Inject]
    [NotNull]
    private DownloadService? DownloadService { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        DownloadService.RegisterStream(this, DownloadFromStream);
        DownloadService.RegisterUrl(this, DownloadFromUrl);
    }

    protected virtual async Task DownloadFromStream(DownloadOption option)
    {
        if (option.FileStream == null)
        {
            throw new InvalidOperationException($"the {nameof(option.FileStream)} is null");
        }

#if NET5_0
        // net 5.0 not support
        await Task.CompletedTask;
#elif NET6_0_OR_GREATER
        using var streamRef = new DotNetStreamReference(option.FileStream);
        await InvokeVoidAsync("downloadFileFromStream", option.FileName, streamRef);
#endif
    }

    protected virtual async Task DownloadFromUrl(DownloadOption option)
    {
        if (string.IsNullOrEmpty(option.Url))
        {
            throw new InvalidOperationException($"{nameof(option.Url)} not set");
        }

        await InvokeVoidAsync("downloadFileFromUrl", option.FileName, option.Url);
    }

    protected override async ValueTask DisposeAsync(bool disposing)
    {
        if (disposing)
        {
            DownloadService.UnRegisterStream(this);
            DownloadService.UnRegisterUrl(this);
        }

        await base.DisposeAsync(disposing);
    }
}
