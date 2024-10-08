using Microsoft.OData.Edm;
using Undersoft.SDK.Service.Data.Client;

namespace Undersoft.SDK.Service.Data.Repository.Client;

public interface IRepositoryClient
    : IRepositoryContextPool,
        IUnique,
        IDisposable,
        IAsyncDisposable
{
    DataClientContext Context { get; }
    bool Pooled { get; }
    Uri Route { get; }

    Task<IEdmModel> BuildMetadata();
    object CreateContext(Type contextType, Uri serviceRoot);
    TContext CreateContext<TContext>(Uri serviceRoot) where TContext : DataClientContext;
    void CreatePool<TContext>();
    TContext GetContext<TContext>() where TContext : DataClientContext;
}

public interface IRepositoryClient<TContext>
    : IRepositoryContextPool<TContext>,
        IRepositoryClient where TContext : class
{
    new TContext Context { get; }

    TContext CreateContext(Uri serviceRoot);
}
