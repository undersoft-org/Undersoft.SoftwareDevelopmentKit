using Microsoft.JSInterop;

namespace Undersoft.SDK.Service.Application.Components;

public class WebClientService : IDisposable
{
    private TaskCompletionSource<bool>? ReturnTask { get; set; }

    private readonly IJSRuntime _runtime;

    private readonly NavigationManager _navigation;

    private JSInterop<WebClientService>? Interop { get; set; }

    private ClientInfo? Client { get; set; }

    public WebClientService(IJSRuntime runtime, NavigationManager navigation) => (_runtime, _navigation) = (runtime, navigation);

    public async Task<ClientInfo> GetClientInfo()
    {
        Interop ??= new JSInterop<WebClientService>(_runtime);
        await Interop.InvokeVoidAsync(this, null, "webClient", "ip.axd", nameof(SetData));

        Client = new ClientInfo()
        {
            RequestUrl = _navigation.Uri
        };

        ReturnTask = new TaskCompletionSource<bool>();
        await ReturnTask.Task;

        return Client;
    }

    [JSInvokable]
    public void SetData(string id, string ip, string os, string browser, string device, string language, string engine, string agent)
    {
        if (Client != null)
        {
            Client.Id = id;
            Client.Ip = ip;
            Client.OS = os;
            Client.Browser = browser;
            Client.Device = WebClientService.ParseDeviceType(device);
            Client.Language = language;
            Client.Engine = engine;
            Client.UserAgent = agent;
        }
        ReturnTask?.TrySetResult(true);
    }

    private static WebClientDeviceType ParseDeviceType(string device)
    {
        var ret = WebClientDeviceType.PC;
        if (Enum.TryParse<WebClientDeviceType>(device, true, out var d))
        {
            ret = d;
        }
        return ret;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Interop?.Dispose();
            Interop = null;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

public class ClientInfo
{
    public string? Id { get; set; }

    public string? Ip { get; set; }

    public string? City { get; set; }

    public string? Browser { get; set; }

    public string? OS { get; set; }

    public WebClientDeviceType Device { get; set; }

    public string? Language { get; set; }

    public string? RequestUrl { get; set; }

    public string? UserAgent { get; set; }

    public string? Engine { get; set; }
}
