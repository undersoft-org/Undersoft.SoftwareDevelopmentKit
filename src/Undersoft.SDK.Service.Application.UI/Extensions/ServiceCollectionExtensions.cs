using Undersoft.SDK.Service.Application.Components;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresenterAzureSpeech(this IServiceCollection services, Action<
        AzureSpeechOption>? configOptions = null)
    {
        services.AddHttpClient();
        services.AddMemoryCache();

        services.TryAddScoped<RecognizerService>();
        services.AddScoped<IRecognizerProvider, AzureRecognizerProvider>();

        services.TryAddScoped<SynthesizerService>();
        services.AddScoped<ISynthesizerProvider, AzureSynthesizerProvider>();

        services.AddOptionsMonitor<AzureSpeechOption>();

        services.Configure<AzureSpeechOption>(option =>
        {
            configOptions?.Invoke(option);
        });
        return services;
    }
}
