using Undersoft.SDK.Series;
using Undersoft.SDK.Service.Data.Service;
using System;

namespace Undersoft.SDK.Service.Data.Repository.Client
{
    public interface IRepositoryClients : ISeries<IRepositoryClient>
    {
        IRepositoryClient this[OpenDataService context] { get; set; }
        IRepositoryClient this[string contextName] { get; set; }
        IRepositoryClient this[Type contextType] { get; set; }

        IRepositoryClient<TContext> Add<TContext>(IRepositoryClient<TContext> repoSource) where TContext : OpenDataService;
        IRepositoryClient Get(Type contextType);
        IRepositoryClient<TContext> Get<TContext>() where TContext : OpenDataService;
        long GetKey(IRepositoryClient item);
        IRepositoryClient New(Type contextType, Uri route);
        IRepositoryClient<TContext> New<TContext>(Uri route) where TContext : OpenDataService;
        IRepositoryClient<TContext> Put<TContext>(IRepositoryClient<TContext> repoSource) where TContext : OpenDataService;
        bool Remove<TContext>() where TContext : OpenDataService;
        bool TryAdd(Type contextType, IRepositoryClient repoSource);
        bool TryAdd<TContext>(IRepositoryClient<TContext> repoSource) where TContext : OpenDataService;
        bool TryGet(Type contextType, out IRepositoryClient repoSource);
        bool TryGet<TContext>(out IRepositoryClient<TContext> repoSource) where TContext : OpenDataService;
    }
}