using Undersoft.SDK.Service.Application.Localization.Json;
using Undersoft.SDK.Service.Application.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Globalization;
using Undersoft.SDK.Service.Application.Components;

namespace Microsoft.Extensions.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationComponents(this IServiceCollection services, Action<ApplicationOptions>? configureOptions = null, Action<JsonLocalizationOptions>? localizationConfigure = null)
    {
        services.AddMemoryCache();
        services.AddHttpClient();

        services.AddAuthorizationCore();
        services.AddJsonLocalization(localizationConfigure);
        services.TryAddSingleton<ICacheManager, CacheManager>();

        services.TryAddSingleton<IComponentIdGenerator, DefaultIdGenerator>();
        services.TryAddSingleton(typeof(IDispatchService<>), typeof(DefaultDispatchService<>));
        services.TryAddSingleton(typeof(ILookupService), typeof(NullLookupService));

        services.TryAddTransient<ITableExcelExport, DefaultExcelExport>();
        services.TryAddScoped(typeof(IDataService<>), typeof(NullDataService<>));
        services.AddScoped<TabItemTextOptions>();

        services.AddScoped<DialogService>();
        services.AddScoped<MessageService>();
        services.AddScoped<ToastService>();
        services.AddScoped<SwalService>();
        services.AddScoped<FullScreenService>();
        services.AddScoped<PrintService>();
        services.AddScoped<TitleService>();
        services.AddScoped<DownloadService>();
        services.AddScoped<WebClientService>();
        services.AddScoped<AjaxService>();
        services.AddScoped(typeof(DragDropService<>));
        services.AddScoped<ClipboardService>();
        services.AddScoped<ResizeNotificationService>();

        services.TryAddScoped<IIPLocatorProvider, DefaultIPLocatorProvider>();
        services.TryAddScoped<IReconnectorProvider, ReconnectorProvider>();

        services.ConfigurePresenterOption(configureOptions);
        services.ConfigureIPLocatorOption();

        services.AddTabItemBindOptions();
        services.AddIconTheme();
        return services;
    }

    private static IServiceCollection ConfigurePresenterOption(this IServiceCollection services, Action<ApplicationOptions>? configureOptions = null)
    {
        services.AddOptionsMonitor<ApplicationOptions>();
        services.Configure<ApplicationOptions>(op =>
        {
            if (op.DefaultCultureInfo != null)
            {
                var culture = new CultureInfo(op.DefaultCultureInfo);
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
            }

            SetFallbackCulture();

            configureOptions?.Invoke(op);

            [ExcludeFromCodeCoverage]
            void SetFallbackCulture()
            {
                if (string.IsNullOrEmpty(CultureInfo.CurrentUICulture.Name))
                {
                    var culture = new CultureInfo(op.FallbackCulture);
                    CultureInfo.CurrentCulture = culture;
                    CultureInfo.CurrentUICulture = culture;
                }
            }
        });
        return services;
    }

    public static IServiceCollection ConfigureIPLocatorOption(this IServiceCollection services, Action<IPLocatorOption>? locatorAction = null)
    {
        services.AddOptionsMonitor<IPLocatorOption>();
        if (locatorAction != null)
        {
            services.Configure(locatorAction);
        }
        return services;
    }

    public static IServiceCollection ConfigureJsonLocalizationOptions(this IServiceCollection services, Action<JsonLocalizationOptions> localizationConfigure)
    {
        services.Configure(localizationConfigure);
        return services;
    }

    public static IServiceCollection AddOptionsMonitor<TOptions>(this IServiceCollection services) where TOptions : class
    {
        services.AddOptions();
        services.TryAddSingleton<IOptionsChangeTokenSource<TOptions>, ConfigurationChangeTokenSource<TOptions>>();
        services.TryAddSingleton<IConfigureOptions<TOptions>, ConfigureOptions<TOptions>>();
        return services;
    }

    static IServiceCollection AddTabItemBindOptions(this IServiceCollection services)
    {
        services.AddOptionsMonitor<TabItemBindOptions>();
        return services;
    }

    public static IServiceCollection ConfigureTabItemMenuBindOptions(this IServiceCollection services, Action<TabItemBindOptions> configureOptions)
    {
        services.Configure(configureOptions);
        return services;
    }

    static IServiceCollection AddIconTheme(this IServiceCollection services)
    {
        services.TryAddSingleton<IIconTheme, DefaultIconTheme>();
        services.AddOptionsMonitor<IconThemeOptions>();
        return services;
    }

    public static IServiceCollection ConfigureIconThemeOptions(this IServiceCollection services, Action<IconThemeOptions> configureOptions)
    {
        services.Configure(configureOptions);
        return services;
    }
}
