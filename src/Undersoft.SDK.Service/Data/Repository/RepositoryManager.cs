using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Undersoft.SDK.Service.Data.Mapper;
using Undersoft.SDK.Service.Data.Service;
using Undersoft.SDK.Service.Data.Store;

namespace Undersoft.SDK.Service.Data.Repository;

using Client;
using Entity;

using Undersoft.SDK.Service.Data.Object;
using Source;

public class RepositoryManager : Catalog<IDataStoreContext>, IDisposable, IAsyncDisposable, IRepositoryManager
{
    private new bool disposedValue;
    protected IDataMapper mapper;

    protected static IRepositorySources Sources { get; set; }
    public static IRepositoryClients Clients { get; set; }

    protected IServiceManager Services { get; init; }

    public IDataMapper Mapper
    {
        get => mapper ??= GetMapper();
    }

    static RepositoryManager()
    {
        Sources = new RepositorySources();
        Clients = new RepositoryClients();
    }
    public RepositoryManager() : base()
    {
    }

    public IStoreRepository<TEntity> use<TStore, TEntity>() where TEntity : class, IDataObject where TStore : IDatabaseStore
    {
        return Use<TStore, TEntity>();
    }
    public IStoreRepository<TEntity> use<TEntity>() where TEntity : class, IDataObject
    {
        return Use<TEntity>();
    }

    public IStoreRepository<TEntity> Use<TEntity>()
    where TEntity : class, IDataObject
    {
        return Use<TEntity>(DataStoreRegistry.GetContexts<TEntity>().FirstOrDefault());
    }
    public IStoreRepository<TEntity> Use<TEntity>(Type contextType)
        where TEntity : class, IDataObject
    {
        return (IStoreRepository<TEntity>)Services.GetService(typeof(IStoreRepository<,>)
                                                 .MakeGenericType(DataStoreRegistry
                                                 .Stores[contextType],
                                                  typeof(TEntity)));
    }
    public IStoreRepository<TEntity> Use<TStore, TEntity>()
       where TEntity : class, IDataObject where TStore : IDatabaseStore
    {
        return Services.GetService<IStoreRepository<TStore, TEntity>>();
    }

    public IRemoteRepository<TDto> load<TStore, TDto>() where TDto : class, IDataObject where TStore : IDataServiceStore
    {
        return Load<TStore, TDto>();
    }
    public IRemoteRepository<TDto> load<TDto>() where TDto : class, IDataObject
    {
        return Load<TDto>();
    }

    public IRemoteRepository<TDto> Load<TDto>() where TDto : class, IDataObject
    {
        return Load<TDto>(OpenDataServiceRegistry.GetContextTypes<TDto>().FirstOrDefault());
    }
    public IRemoteRepository<TDto> Load<TDto>(Type contextType)
       where TDto : class, IDataObject
    {
        return (IRemoteRepository<TDto>)Services.GetService(typeof(IRemoteRepository<,>)
                                                 .MakeGenericType(OpenDataServiceRegistry
                                                 .Stores[contextType],
                                                  typeof(TDto)));
    }
    public IRemoteRepository<TDto> Load<TStore, TDto>() where TDto : class, IDataObject where TStore : IDataServiceStore
    {
        var result = Services.GetService<IRemoteRepository<TStore, TDto>>();
        return result;
    }

    public IRepositorySource GetSource<TStore, TEntity>()
    where TEntity : class, IDataObject
    {
        var contextType = DataStoreRegistry.GetContext<TStore, TEntity>();
        return Sources.Get(contextType);
    }

    public IRepositoryClient GetClient<TStore, TEntity>()
    where TEntity : class, IDataObject
    {
        var contextType = OpenDataServiceRegistry.GetContextType<TStore, TEntity>();

        return Clients.Get(contextType);
    }

    public static void AddClientPool(Type contextType, int poolSize, int minSize = 1)
    {
        if (TryGetClient(contextType, out IRepositoryClient client))
        {
            client.PoolSize = poolSize;
            client.CreatePool();
        }
    }

    public Task AddClientPools()
    {
        return Task.Run(() =>
        {
            foreach (var client in GetClients())
            {
                client.CreatePool();
            }
        });
    }

    public static IRepositoryClient CreateClient(IRepositoryClient client)
    {
        Type repotype = typeof(RepositoryClient<>).MakeGenericType(client.ContextType);
        return (IRepositoryClient)repotype.New(client);
    }
    public static IRepositoryClient<TContext> CreateClient<TContext>(IRepositoryClient<TContext> client)
        where TContext : OpenDataService
    {
        return new RepositoryClient<TContext>(client);
    }
    public static IRepositoryClient<TContext> CreateClient<TContext>(Uri serviceRoot) where TContext : OpenDataService
    {
        return new RepositoryClient<TContext>(serviceRoot);
    }
    public static IRepositoryClient CreateClient(Type contextType, Uri serviceRoot)
    {
        return new RepositoryClient(contextType, serviceRoot);
    }

    public static IRepositoryClient AddClient(IRepositoryClient client)
    {
        if (Clients == null)
            Clients = ServiceManager.GetObject<IRepositoryClients>();
        Clients.Add(client);
        return client;
    }

    public static bool TryGetClient<TContext>(out IRepositoryClient<TContext> source) where TContext : OpenDataService
    {
        return Clients.TryGet(out source);
    }
    public static bool TryGetClient(Type contextType, out IRepositoryClient source)
    {
        return Clients.TryGet(contextType, out source);
    }

    public Task AddEndpointPools()
    {
        return Task.Run(() =>
        {
            foreach (var source in Sources)
            {
                source.CreatePool();
            }
        });
    }

    public static void AddSourcePool(Type contextType, int poolSize, int minSize = 1)
    {
        if (TryGetSource(contextType, out IRepositorySource source))
        {
            source.PoolSize = poolSize;
            source.CreatePool();
        }
    }

    public static IRepositorySource<TContext> CreateSource<TContext>(DbContextOptions<TContext> options) where TContext : DataStoreContext
    {
        return new RepositorySource<TContext>(options);
    }
    public static IRepositorySource CreateSource(IRepositorySource source)
    {
        Type repotype = typeof(RepositorySource<>).MakeGenericType(source.ContextType);
        return (IRepositorySource)repotype.New(source);
    }
    public static IRepositorySource<TContext> CreateSource<TContext>(IRepositorySource<TContext> source)
        where TContext : DataStoreContext
    {
        return typeof(RepositorySource<TContext>).New<IRepositorySource<TContext>>(source);
    }
    public static IRepositorySource CreateSource(DbContextOptions options)
    {
        return new RepositorySource(options);
    }

    public static IRepositorySource AddSource(IRepositorySource source)
    {
        if (Sources == null)
            Sources = ServiceManager.GetObject<IRepositorySources>();
        Sources.Add(source);
        return source;
    }

    public static bool TryGetSource<TContext>(out IRepositorySource<TContext> source) where TContext : DbContext
    {
        return Sources.TryGet(out source);
    }
    public static bool TryGetSource(Type contextType, out IRepositorySource source)
    {
        return Sources.TryGet(contextType, out source);
    }

    public IEnumerable<IRepositorySource> GetSources()
    {
        return Sources;
    }

    public IEnumerable<IRepositoryClient> GetClients()
    {
        return Clients;
    }

    public static IDataMapper CreateMapper(params Profile[] profiles)
    {
        DataMapper.AddProfiles(profiles);
        return ServiceManager.GetObject<IDataMapper>();
    }
    public static IDataMapper CreateMapper<TProfile>() where TProfile : Profile
    {
        DataMapper.AddProfiles(typeof(TProfile).New<TProfile>());
        return ServiceManager.GetObject<IDataMapper>();
    }

    public static IDataMapper GetMapper()
    {
        return ServiceManager.GetObject<IDataMapper>();
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                base.Dispose(true);
            }
            disposedValue = true;
        }
    }

    public override async ValueTask DisposeAsyncCore()
    {
        await base.DisposeAsyncCore().ConfigureAwait(false);
    }
}
