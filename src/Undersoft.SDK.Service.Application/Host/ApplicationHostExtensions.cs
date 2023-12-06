using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Undersoft.SDK.Service.Application;

using Undersoft.SDK.Service.Data.Repository;
using Service.Host;

public static class ApplicationHostExtensions
{
    public static IApplicationHostSetup UseApplicationSetup(this IApplicationBuilder app, IWebHostEnvironment env, bool useSwagger = true)
    {
        return new ApplicationHostSetup(app, env, useSwagger);
    }
    public static IApplicationHostSetup UseApplicationSetup(this IApplicationBuilder app, IWebHostEnvironment env, string[] apiVersions)
    {
        return new ApplicationHostSetup(app, env, apiVersions);
    }
    public static IApplicationBuilder UseDefaultProvider(this IApplicationBuilder app)
    {
        new ApplicationHostSetup(app).UseDefaultProvider();
        return app;
    }

    public static IApplicationBuilder UseInternalProvider(this IApplicationBuilder app)
    {
        new ApplicationHostSetup(app).UseInternalProvider();
        return app;
    }

    public static IApplicationBuilder RebuildProviders(this IApplicationBuilder app)
    {
        new ApplicationHostSetup(app).RebuildProviders();
        return app;
    }

    public static async Task LoadOpenDataEdms(this ApplicationHostSetup app)
    {
        await Task.Run(() =>
        {
            RepositoryManager.Clients.ForEach((client) =>
            {
                client.BuildMetadata();
            });

            ServiceHostSetup.AddOpenDataServiceImplementations();
            app.RebuildProviders();
        });
    }
}