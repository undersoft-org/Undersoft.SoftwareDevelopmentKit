using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Undersoft.SDK.Service.Configuration;

public static class ServiceConfigutrationExtensions
{
    public static IServiceConfiguration BuildConfiguration(this IConfiguration config)
    {
        return new ServiceConfiguration(config);
    }

    public static IServiceConfiguration BuildConfiguration(
        this IConfiguration config,
        IServiceCollection services
    )
    {
        var _config = new ServiceConfiguration(config);
        _config.Services = services;
        return _config;
    }
}
