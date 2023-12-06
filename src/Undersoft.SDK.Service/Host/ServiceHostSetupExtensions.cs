using Microsoft.Extensions.Hosting;

namespace Undersoft.SDK.Service.Host
{
    public static class ServiceHostSetupExtensions
    {
        public static IServiceHostSetup UseAppSetup(this IHostBuilder app, IHostEnvironment env)
        {
            return new ServiceHostSetup(app, env);
        }

        public static IHostBuilder UseInternalProvider(this IHostBuilder app)
        {
            new ServiceHostSetup(app).UseAdvancedProvider();
            return app;
        }

        public static IHostBuilder RebuildProviders(this IHostBuilder app)
        {
            new ServiceHostSetup(app).RebuildProviders();
            return app;
        }
    }
}