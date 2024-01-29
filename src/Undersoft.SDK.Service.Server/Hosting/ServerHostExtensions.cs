using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Undersoft.SDK.Service.Server.Hosting;

using Service.Hosting;
using Undersoft.SDK.Service.Data.Repository;

public static class ServerHostExtensions
{
    public static IServerHostSetup UseServerSetup(this IApplicationBuilder app)
    {
        return new ServerHostSetup(app);
    }
    public static IServerHostSetup UseServerSetup(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        return new ServerHostSetup(app, env);
    }
    public static IApplicationBuilder UseDefaultProvider(this IApplicationBuilder app)
    {
        new ServerHostSetup(app).UseDefaultProvider();
        return app;
    }

    public static IApplicationBuilder UseInternalProvider(this IApplicationBuilder app)
    {
        new ServerHostSetup(app).UseInternalProvider();
        return app;
    }

    public static IApplicationBuilder RebuildProviders(this IApplicationBuilder app)
    {
        new ServerHostSetup(app).RebuildProviders();
        return app;
    }

    public static async Task LoadOpenDataEdms(this ServerHostSetup app)
    {
        await Task.Run(() =>
        {
            Task.WaitAll(app.Manager.GetClients().ForEach((client) =>
            {
                return client.BuildMetadata();
            }).Commit());

            app.Manager.Registry.AddOpenDataRemoteImplementations();
            app.RebuildProviders();
        });
    }
}