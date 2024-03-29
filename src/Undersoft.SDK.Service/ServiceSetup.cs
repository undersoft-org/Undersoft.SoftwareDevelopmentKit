﻿using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Undersoft.SDK.Service;

using Configuration;
using Data.Cache;
using Data.Mapper;
using Microsoft.IdentityModel.Tokens;
using Undersoft.SDK.Security;
using Undersoft.SDK.Service.Data.Client;
using Undersoft.SDK.Service.Data.Repository;
using Undersoft.SDK.Service.Data.Repository.Client;
using Undersoft.SDK.Service.Data.Repository.Source;
using Undersoft.SDK.Service.Data.Store;

public partial class ServiceSetup : IServiceSetup
{
    protected string[] apiVersions = new string[1] { "1" };
    protected Assembly[] Assemblies;

    protected IServiceConfiguration configuration => manager.Configuration;
    protected IServiceManager manager { get; }
    protected IServiceRegistry registry => manager.Registry;
    protected IServiceCollection services => registry.Services;

    public ServiceSetup(IServiceCollection services)
    {
        manager = new ServiceManager(services);
        registry.MergeServices(true);
    }

    public ServiceSetup(IServiceCollection services, IConfiguration configuration) : this(services)
    {
        manager.Configuration = new ServiceConfiguration(configuration, services);
    }

    public IServiceRegistry Services => registry;

    public IServiceManager Manager => manager;

    public IServiceSetup AddCaching()
    {
        registry.AddObject(RootCache.Catalog);

        Type[] stores = new Type[]
        {
            typeof(IEntryStore),
            typeof(IReportStore),
            typeof(IEventStore),
            typeof(IDataStore),
            typeof(IAccountStore)
        };
        foreach (Type item in stores)
        {
            AddStoreCache(item);
        }

        return this;
    }

    public virtual IServiceSetup AddSourceProviderConfiguration()
    {
        ServiceManager.AddRootObject<ISourceProviderConfiguration>(new ServiceSourceProviderConfiguration(registry));

        return this;
    }

    public void AddJsonSerializerDefaults()
    {
#if NET6_0
        var newopts = new JsonSerializerOptions();
        newopts.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        newopts.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        newopts.NumberHandling = JsonNumberHandling.AllowReadingFromString;
        newopts.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

        var fld = (
            typeof(JsonSerializerOptions).GetField(
                "s_defaultOptions",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic
            )
        );

        var opt = (JsonSerializerOptions)fld.GetValue(newopts);
        if (opt == null)
            fld.SetValue(newopts, newopts);
        else
            manager.Mapper.Map(newopts, opt);
#endif
#if NET7_0
        var flds = typeof(JsonSerializerOptions).GetRuntimeFields();
        flds.Single(f => f.Name == "_defaultIgnoreCondition")
            .SetValue(JsonSerializerOptions.Default, JsonIgnoreCondition.WhenWritingNull);
        flds.Single(f => f.Name == "_referenceHandler")
            .SetValue(JsonSerializerOptions.Default, ReferenceHandler.IgnoreCycles);
#endif
    }

    public IServiceSetup AddLogging()
    {
        registry.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
        return this;
    }

    public IServiceSetup AddPropertyInjection()
    {
        manager.AddPropertyInjection();

        return this;
    }

    public IServiceSetup AddImplementations()
    {
        registry.AddScoped<IServicer, Servicer>();
        registry.AddScoped<IAuthorization, Authorization>();

        AddDomainImplementations();

        registry.MergeServices(true);

        return this;
    }

    public IServiceSetup AddMapper<TProfile>() where TProfile : MapperProfile
    {
        AddMapper(new DataMapper(typeof(TProfile).New<TProfile>()));

        return this;
    }

    public IServiceSetup AddMapper(params MapperProfile[] profiles)
    {
        AddMapper(new DataMapper(profiles));

        return this;
    }

    public IServiceSetup AddMapper(IDataMapper mapper)
    {
        registry.AddObject(mapper);
        registry.AddObject<IDataMapper>(mapper);
        registry.AddScoped<IMapper, DataMapper>();

        return this;
    }

    public IServiceSetup AddRepositoryClients()
    {        
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        Type[] serviceTypes = assemblies.SelectMany(a => a.DefinedTypes).Select(t => t.UnderlyingSystemType).ToArray();
        return AddRepositoryClients(serviceTypes);
    }

    public IServiceSetup AddRepositoryClients(Type[] serviceTypes)
    {
        IServiceConfiguration config = configuration;

        IEnumerable<IConfigurationSection> clients = config.Clients();
        RepositoryClients repoClients = new RepositoryClients();

        services.AddSingleton(registry.AddObject<IRepositoryClients>(repoClients).Value);

        foreach (IConfigurationSection client in clients)
        {
            ClientProvider provider = config.ClientProvider(client);
            string connectionString = config.ClientConnectionString(client).Trim();
            int poolsize = config.ClientPoolSize(client);
            Type contextType = serviceTypes
                .Where(t => t.FullName.Contains(client.Key))                
                .FirstOrDefault();

            if (
                (provider == ClientProvider.None)
                || (connectionString == null)
                || (contextType == null)
            )
                continue;

            string routePrefix = AddDataClientPrefix(contextType).Trim();
            if (!connectionString.EndsWith('/'))
                connectionString += "/";

            if (routePrefix.StartsWith('/'))
                routePrefix = routePrefix.Substring(1);

            routePrefix = provider.ToString().ToLower() + (!routePrefix.IsNullOrEmpty() ? ("/" + routePrefix) : "");

            string _connectionString = $"{connectionString}{routePrefix}";

            Type iRepoType = typeof(IRepositoryClient<>).MakeGenericType(contextType);
            Type repoType = typeof(RepositoryClient<>).MakeGenericType(contextType);

            IRepositoryClient repoClient = (IRepositoryClient)
                repoType.New(provider, _connectionString);

            Type storeDbType = typeof(OpenDataClient<>).MakeGenericType(
                OpenDataRegistry.GetLinkedStoreType(contextType)
            );
            Type storeRepoType = typeof(RepositoryClient<>).MakeGenericType(storeDbType);

            IRepositoryClient storeClient = (IRepositoryClient)storeRepoType.New(repoClient);

            Type istoreRepoType = typeof(IRepositoryClient<>).MakeGenericType(storeDbType);
            Type ipoolRepoType = typeof(IRepositoryContextPool<>).MakeGenericType(storeDbType);
            Type ifactoryRepoType = typeof(IRepositoryContextFactory<>).MakeGenericType(
                storeDbType
            );
            Type idataRepoType = typeof(IRepositoryContext<>).MakeGenericType(storeDbType);

            repoClient.PoolSize = poolsize;

            IRepositoryClient globalClient = manager.AddClient(repoClient);

            registry.AddObject(iRepoType, globalClient);
            registry.AddObject(repoType, globalClient);

            registry.AddObject(istoreRepoType, storeClient);
            registry.AddObject(ipoolRepoType, storeClient);
            registry.AddObject(ifactoryRepoType, storeClient);
            registry.AddObject(idataRepoType, storeClient);
            registry.AddObject(storeRepoType, storeClient);

            manager.AddClientPool(globalClient.ContextType, poolsize);
        }

        return this;
    }

    public virtual IServiceSetup ConfigureServices(Type[] clientTypes = null)
    {
        Assemblies = AppDomain.CurrentDomain.GetAssemblies();

        AddLogging();

        AddMapper(new DataMapper());        

        AddJsonSerializerDefaults();

        if (clientTypes != null)
            AddRepositoryClients(clientTypes);
        else
            AddRepositoryClients();   

        AddImplementations();

        AddCaching();

        return this;
    }

    public IServiceSetup MergeServices()
    {
        registry.MergeServices();

        return this;
    }

    public IServiceSetup AddStoreCache(Type tstore)
    {
        Type idatacache = typeof(IStoreCache<>).MakeGenericType(tstore);
        Type datacache = typeof(StoreCache<>).MakeGenericType(tstore);

        object cache = datacache.New(registry.GetObject<IDataCache>());

        registry.AddObject(idatacache, cache);
        registry.AddObject(datacache, cache);

        return this;
    }

    public IServiceSetup AddStoreCache<TStore>()
    {
        return AddStoreCache(typeof(TStore));
    }

    private string AddDataClientPrefix(Type contextType, string routePrefix = null)
    {
        Type iface = OpenDataRegistry.GetLinkedStoreType(contextType);
        return GetStoreRoutes(iface, routePrefix);
    }

    protected string GetStoreRoutes(Type iface, string routePrefix = null)
    {
        if (iface == typeof(IEntryStore))
        {
            return StoreRoutes.EntryStoreRoute;
        }
        else if (iface == typeof(IEventStore))
        {
            return StoreRoutes.EventStoreRoute;
        }
        else if (iface == typeof(IReportStore))
        {
            return StoreRoutes.ReportStoreRoute;
        }
        else if (iface == typeof(IDataStore))
        {
            return StoreRoutes.DataStoreRoute;
        }
        else if (iface == typeof(IAccountStore))
        {
            return StoreRoutes.AuthStoreRoute;
        }
        else
        {
            return StoreRoutes.DataStoreRoute;
        }
    }
}
