using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;

namespace Undersoft.SDK.Service.Configuration;

using Undersoft.SDK.Security;
using Undersoft.SDK.Service.Data.Client;
using Undersoft.SDK.Service.Data.Repository;
using Undersoft.SDK.Service.Data.Store;

public class ServiceConfiguration : IServiceConfiguration
{
    private IConfiguration config;
    public IServiceCollection Services;

    private AccountServerOptions _identity;
    public AccountServerOptions Identity => _identity ??= GetOpenApiConfiguration();

    private RepositoryOptions _repository;
    public RepositoryOptions Repositories => _repository ??= GetRepositoryConfiguration(); 

    public string this[string key]
    {
        get => config[key];
        set => config[key] = value;
    }

    public ServiceConfiguration()
    {
        config = ServiceConfigurationHelper.BuildConfiguration();
    }

    public ServiceConfiguration(IConfiguration config)
    {
        this.config = config;
    }

    public ServiceConfiguration(IServiceCollection services)
    {
        config = ServiceConfigurationHelper.BuildConfiguration();
        Services = services;
    }

    public ServiceConfiguration(IConfiguration config, IServiceCollection services)
    {
        this.config = config;
        Services = services;
    }

    public IServiceConfiguration Configure<TOptions>(string sectionName) where TOptions : class
    {
        Services.Configure<TOptions>(config.GetSection(sectionName));
        return this;
    }

    public IServiceConfiguration Configure<TOptions>(
        string sectionName,
        Action<BinderOptions> configureOptions
    ) where TOptions : class
    {
        Services.Configure<TOptions>(config.GetSection(sectionName), configureOptions);
        return this;
    }

    public IServiceConfiguration Configure<TOptions>(Action<TOptions> configureOptions)
        where TOptions : class
    {
        Services.Configure(configureOptions);
        return this;
    }

    public string Version => config["ServiceVersion"];
    public string Title => config["Title"];
    public string Description => config["Description"];

    public string DataServiceRoutes(string name)
    {
        return config.GetSection("StoreRoutes")[name];
    }

    public IConfigurationSection Repository()
    {
        return config.GetSection("Repository");
    }

    public IConfigurationSection DataCacheLifeTime()
    {
        return config.GetSection("DataCache");
    }

    public IEnumerable<IConfigurationSection> Sources()
    {
        return config.GetSection("Repository").GetSection("Sources").GetChildren();
    }

    public IEnumerable<IConfigurationSection> Clients()
    {
        return Repository().GetSection("Clients").GetChildren();
    }

    public IConfigurationSection Source(string name)
    {
        return Repository()?.GetSection("Sources")?.GetSection(name);
    }

    public string SourceConnectionString(string name)
    {
        return Source(name)["ConnectionString"];
    }

    public string ClientConnectionString(string name)
    {
        return Client(name)["ConnectionString"];
    }

    public string SourceConnectionString(IConfigurationSection endpoint)
    {
        string connStr = Environment.GetEnvironmentVariable(endpoint.Key);
        if (!string.IsNullOrEmpty(connStr))
        {
            Console.WriteLine($"Connection string for {endpoint.Key} from environment variable");
        }
        var result = connStr ?? endpoint["ConnectionString"];
        return result;
    }

    public string ClientConnectionString(IConfigurationSection client)
    {
        return client["ConnectionString"];
    }

    public IConfigurationSection Client(string name)
    {
        return Repository()?.GetSection("Clients")?.GetSection(name);
    }

    public StoreProvider SourceProvider(string name)
    {
        Enum.TryParse(Source(name)["StoreProvider"], out StoreProvider result);
        return result;
    }

    public ClientProvider ClientProvider(string name)
    {
        Enum.TryParse(Client(name)["ClientProvider"], out ClientProvider result);
        return result;
    }

    public StoreProvider SourceProvider(IConfigurationSection source)
    {
        Enum.TryParse(source["StoreProvider"], out StoreProvider result);
        return result;
    }

    public ClientProvider ClientProvider(IConfigurationSection client)
    {
        Enum.TryParse(client["ClientProvider"], out ClientProvider result);
        return result;
    }

    public int SourcePoolSize(IConfigurationSection endpoint)
    {
        return endpoint.GetValue<int>("PoolSize");
    }

    public int ClientPoolSize(IConfigurationSection client)
    {
        return client.GetValue<int>("PoolSize");
    }

    public IEnumerable<IConfigurationSection> GetChildren()
    {
        return config.GetChildren();
    }

    public IChangeToken GetReloadToken()
    {
        return config.GetReloadToken();
    }

    public IConfigurationSection GetSection(string key)
    {
        return config.GetSection(key);
    }

    public IConfigurationSection AccountServer()
    {
        return config.GetSection("AccountServer");
    }

    public string IdentityServerBaseUrl()
    {
        return AccountServer().GetValue<string>("ServerBaseUrl");
    }

    public string IdentityServiceName()
    {
        return AccountServer().GetValue<string>("ServiceName");
    }

    public string[] IdentityServerScopes()
    {
        return AccountServer()?.GetValue<string[]>("Scopes");
    }

    public string[] IdentityServerClaims()
    {
        return AccountServer()?.GetValue<string[]>("Claims");
    }

    public string[] IdentityServerRoles()
    {
        return AccountServer()?.GetValue<string[]>("Roles");
    }

    public AccountServerOptions GetAccountServerConfiguration()
    {
        var identity = new AccountServerOptions();
        config.Bind("AccountServer", identity);
        return identity;
    }

    public AccountServerOptions GetOpenApiConfiguration()
    {
        var identity = new AccountServerOptions();
        config.Bind("AccountServer", identity);
        return identity;
    }

    public RepositoryOptions GetRepositoryConfiguration()
    {
        var options = new RepositoryOptions();
        config.Bind("Repository", options);
        return options;
    }
}
