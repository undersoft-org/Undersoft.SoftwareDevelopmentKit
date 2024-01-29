using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Undersoft.SDK.Service.Application.Components;

public class AzureSynthesizerProvider : ISynthesizerProvider, IAsyncDisposable
{
    private DotNetObjectReference<AzureSynthesizerProvider>? Interop { get; set; }

    private IJSObjectReference? Module { get; set; }

    [NotNull]
    private SynthesizerOption? Option { get; set; }

    private AzureSpeechOption SpeechOption { get; }

    private IJSRuntime JSRuntime { get; }

    private HttpClient Client { get; }

    private IMemoryCache Cache { get; }

    private ILogger Logger { get; }

    public AzureSynthesizerProvider(IOptionsMonitor<AzureSpeechOption> options, IJSRuntime runtime, IHttpClientFactory factory, IMemoryCache cache, ILogger<AzureSynthesizerProvider> logger)
    {
        Cache = cache;
        JSRuntime = runtime;
        SpeechOption = options.CurrentValue;
        Logger = logger;
        Client = factory.CreateClient();
        Client.BaseAddress = new Uri(string.Format(SpeechOption.AuthorizationTokenUrl, SpeechOption.Region));
        if (SpeechOption.Timeout > 0)
        {
            Client.Timeout = TimeSpan.FromMilliseconds(SpeechOption.Timeout);
        }
        Client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", SpeechOption.SubscriptionKey);
    }

    public async Task InvokeAsync(SynthesizerOption option)
    {
        Option = option;
        if (Module == null)
        {
            Module = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/BootstrapBlazor.AzureSpeech/js/speech.js");
        }
        Interop ??= DotNetObjectReference.Create(this);

        if (Option.MethodName == "bb_azure_speech_synthesizerOnce" && !string.IsNullOrEmpty(Option.Text))
        {
            var token = await ExchangeToken();
            await Module.InvokeVoidAsync(Option.MethodName, Interop, nameof(Callback), token, SpeechOption.Region, Option.SpeechSynthesisLanguage, Option.SpeechSynthesisVoiceName, Option.Text);
        }
        else if (Option.MethodName == "bb_azure_close_synthesizer")
        {
            await Module.InvokeVoidAsync(Option.MethodName, Interop, nameof(Callback));
        }
    }

    private Task<string> ExchangeToken() => Cache.GetOrCreateAsync(SpeechOption.SubscriptionKey, async entry =>
    {
        var url = string.Format(SpeechOption.AuthorizationTokenUrl, SpeechOption.Region);
        var ret = "";
        try
        {
            Logger.LogInformation($"request url: {url}");
            var result = await Client.PostAsJsonAsync<string>(url, "");
            if (result.IsSuccessStatusCode)
            {
                ret = await result.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(ret))
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(9);
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "ExchangeToken");
        }
        return ret;
    })!;

    [JSInvokable]
    public async Task Callback(SynthesizerStatus status)
    {
        Logger.LogInformation("SynthesizerStatus: {Status}", status);
        if (Option.Callback != null)
        {
            await Option.Callback(status);
        }
    }

    private async ValueTask DisposeAsync(bool disposing)
    {
        if (disposing)
        {
            if (Interop != null)
            {
                Interop.Dispose();
            }
            if (Module is not null)
            {
                await Module.DisposeAsync();
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }
}
