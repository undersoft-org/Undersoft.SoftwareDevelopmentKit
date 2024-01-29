using Undersoft.SDK.Service.Application.Localization;
using Undersoft.SDK.Service.Application.Localization.Json;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;

namespace Microsoft.Extensions.DependencyInjection;

internal static class JsonLocalizationServiceCollectionExtensions
{
    public static IServiceCollection AddJsonLocalization(this IServiceCollection services, Action<JsonLocalizationOptions>? localizationConfigure = null)
    {
        services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
        services.TryAddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));
        services.TryAddTransient<IStringLocalizer, StringLocalizer>();
        services.TryAddSingleton<ILocalizationResolve, NullLocalizationResolve>();
        if (localizationConfigure != null)
        {
            services.Configure(localizationConfigure);
        }
        return services;
    }
}
