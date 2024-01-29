using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProtoBuf.Grpc.Server;
using Swashbuckle.AspNetCore;
using Undersoft.SDK.Service.Server;

namespace Undersoft.SDK.Service.Server.Hosting;

using Logging;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Routing;
using ProtoBuf.Grpc.Reflection;
using Quartz.Impl.AdoJobStore.Common;
using Series;
using Undersoft.SDK.Service.Data.Store;

public class ServerHostSetup : IServerHostSetup
{
    protected static bool defaultProvider;

    protected readonly IApplicationBuilder _builder;
    protected readonly IWebHostEnvironment _environment;
    protected readonly IServiceManager _manager;

    public ServerHostSetup(IApplicationBuilder application)
    {
        _builder = application;
        _manager = _builder.ApplicationServices.GetService<IServiceManager>();
    }

    public ServerHostSetup(IApplicationBuilder application, IWebHostEnvironment environment)
        : this(application)
    {
        _environment = environment;
    }

    public IServerHostSetup RebuildProviders()
    {
        if (defaultProvider)
        {
            UseDefaultProvider();
        }
        else
        {
            UseInternalProvider();
        }

        return this;
    }

    public IServerHostSetup UseEndpoints(bool useRazorPages = false)
    {
        _builder.UseEndpoints(endpoints =>
        {
            var method = typeof(GrpcEndpointRouteBuilderExtensions)
                .GetMethods()
                .Where(m => m.Name.Contains("MapGrpcService"))
                .FirstOrDefault()
                .GetGenericMethodDefinition();

            ISeries<Type> serviceContracts = GrpcDataServerRegistry.ServiceContracts;

            if (serviceContracts.Any())
            {
                foreach (var serviceContract in serviceContracts)
                {
                    method
                        .MakeGenericMethod(serviceContract)
                        .Invoke(endpoints, new object[] { endpoints });            

                }

                endpoints.MapCodeFirstGrpcReflectionService();
            }

            _manager.Registry.MergeServices();

            endpoints.MapControllers();

            if (useRazorPages)
            {
                endpoints.MapRazorPages();
                endpoints.MapFallbackToFile("/index.html");
            }
        });

        return this;
    }

    public IServerHostSetup MapFallbackToFile(string filePath)
    {
        _builder.UseEndpoints(endpoints =>
        {
            endpoints.MapFallbackToFile(filePath);
        });

        return this;
    }

    public IServerHostSetup UseServiceClients()
    {
        this.LoadOpenDataEdms().ConfigureAwait(true);
        return this;
    }

    public IServerHostSetup UseDataMigrations()
    {
        using (IServiceScope scope = _manager.CreateScope())
        {
            try
            {
                scope.ServiceProvider
                    .GetRequiredService<IServicer>()
                    .GetSources()
                    .ForEach(e => ((IDataStoreContext)e.Context).Database.Migrate());
            }
            catch (Exception ex)
            {
                this.Error<Applog>(
                    "DataServer migration initial create - unable to connect the database engine",
                    null,
                    ex
                );
            }
        }

        return this;
    }

    public IServerHostSetup UseDefaultProvider()
    {
        _manager.Registry.MergeServices(true);
        ServiceManager.SetProvider(_builder.ApplicationServices);
        defaultProvider = true;
        return this;
    }

    public IServerHostSetup UseInternalProvider()
    {
        _manager.Registry.MergeServices(true);
        _builder.ApplicationServices = _manager.BuildInternalProvider();
        return this;
    }

    public IServerHostSetup UseServiceServer(string[] apiVersions = null)
    {
        UseHeaderForwarding();

        if (_environment.IsDevelopment())
            _builder.UseDeveloperExceptionPage();

        _builder
            .UseHttpsRedirection()
            .UseODataBatching()
            .UseODataQueryRequest()
            .UseDefaultFiles()
            .UseStaticFiles()
            .UseRouting()
            .UseCors();

        if (apiVersions != null)
            UseSwaggerSetup(apiVersions);

        _builder.UseAuthentication().UseAuthorization();

        UseJwtMiddleware();

        UseEndpoints();

        return this;
    }

    public IServerHostSetup UseCustomSetup(Action<IServerHostSetup> action)
    {
        action(this);

        return this;
    }

    public IServerHostSetup UseSwaggerSetup(string[] apiVersions)
    {
        if (_builder == null)
        {
            throw new ArgumentNullException(nameof(_builder));
        }

        var ao = _manager.Configuration.Identity;

        _builder
            .UseSwagger()
            .UseSwaggerUI(options =>
            {
               options.SwaggerEndpoint($"/swagger/v1/swagger.json", ao.ServiceName);
                //options.SwaggerEndpoint($"{ao.ServiceBaseUrl}/swagger/v1/swagger.json", ao.ServiceName);
                //s.OAuthClientId(ao.SwaggerClientId);
                //s.OAuthAppName(ao.ApiName);
            });
        return this;
    } 

    public IServerHostSetup UseHeaderForwarding()
    {
        var forwardingOptions = new ForwardedHeadersOptions()
        {
            ForwardedHeaders = ForwardedHeaders.All
        };

        forwardingOptions.KnownNetworks.Clear();
        forwardingOptions.KnownProxies.Clear();

        _builder.UseForwardedHeaders(forwardingOptions);

        return this;
    }

    public IServerHostSetup UseJwtMiddleware()
    {
        _builder.UseMiddleware<ServerHostJwtMiddleware>();

        return this;
    }

    public IServiceManager Manager => _manager;

    protected IApplicationBuilder Application => _builder;

    protected IWebHostEnvironment LocalEnvironment => _environment;
}
