using Undersoft.SDK.Service.Data.Repository;
using Undersoft.SDK.Service.Data.Service;
using Undersoft.SDK.Uniques;
using System;

namespace Undersoft.SDK.Service.Data.Repository.Client;

public interface IRepositoryClient
    : IRepositoryContextPool,
        IUnique,
        IDisposable,
        IAsyncDisposable
{
    OpenDataService Context { get; }

    Uri Route { get; }

    TContext GetContext<TContext>() where TContext : OpenDataService;

    object CreateContext(Type contextType, Uri serviceRoot);
    TContext CreateContext<TContext>(Uri serviceRoot) where TContext : OpenDataService;

    void BuildMetadata();
}

public interface IRepositoryClient<TContext>
    : IRepositoryContextPool<TContext>,
        IRepositoryClient where TContext : class
{
    new TContext Context { get; }

    TContext CreateContext(Uri serviceRoot);
}
