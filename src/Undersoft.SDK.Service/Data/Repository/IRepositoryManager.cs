using Microsoft.EntityFrameworkCore;
using Undersoft.SDK.Service.Data.Client;
using Undersoft.SDK.Service.Data.Mapper;
using Undersoft.SDK.Service.Data.Object;
using Undersoft.SDK.Service.Data.Remote.Repository;
using Undersoft.SDK.Service.Data.Repository.Client;
using Undersoft.SDK.Service.Data.Repository.Source;
using Undersoft.SDK.Service.Data.Store;
using Undersoft.SDK.Service.Data.Store.Repository;

namespace Undersoft.SDK.Service.Data.Repository
{
    public interface IRepositoryManager
    {
        IDataMapper Mapper { get; }

        IRepositoryClient AddClient(IRepositoryClient client);
        void AddClientPool(Type contextType, int poolSize, int minSize = 1);
        Task AddClientPools();
        Task AddEndpointPools();
        IRepositorySource AddSource(IRepositorySource source);
        void AddSourcePool(Type contextType, int poolSize, int minSize = 1);
        IDataMapper CreateMapper(params MapperProfile[] profiles);
        IDataMapper CreateMapper<TProfile>() where TProfile : MapperProfile;
        ValueTask DisposeAsyncCore();
        IRepositoryClient GetClient<TStore, TEntity>() where TEntity : class, IOrigin, IInnerProxy;
        IEnumerable<IRepositoryClient> GetClients();
        IDataMapper GetMapper();
        IRepositorySource GetSource<TStore, TEntity>() where TEntity : class, IOrigin, IInnerProxy;
        IEnumerable<IRepositorySource> GetSources();
        IRemoteRepository<TDto> load<TDto>() where TDto : class, IOrigin, IInnerProxy;
        IRemoteRepository<TDto> Load<TDto>() where TDto : class, IOrigin, IInnerProxy;
        IRemoteRepository<TDto> Load<TDto>(Type contextType) where TDto : class, IOrigin, IInnerProxy;
        IRemoteRepository<TDto> load<TStore, TDto>()
            where TStore : IDataServiceStore
            where TDto : class, IOrigin, IInnerProxy;
        IRemoteRepository<TDto> Load<TStore, TDto>()
            where TStore : IDataServiceStore
            where TDto : class, IOrigin, IInnerProxy;
        bool TryGetClient(Type contextType, out IRepositoryClient source);
        bool TryGetClient<TContext>(out IRepositoryClient<TContext> source) where TContext : OpenDataContext;
        bool TryGetSource(Type contextType, out IRepositorySource source);
        bool TryGetSource<TContext>(out IRepositorySource<TContext> source) where TContext : DbContext;
        IStoreRepository<TEntity> use<TEntity>() where TEntity : class, IOrigin, IInnerProxy;
        IStoreRepository<TEntity> Use<TEntity>() where TEntity : class, IOrigin, IInnerProxy;
        IStoreRepository<TEntity> Use<TEntity>(Type contextType) where TEntity : class, IOrigin, IInnerProxy;
        IStoreRepository<TEntity> use<TStore, TEntity>()
            where TStore : IDataServerStore
            where TEntity : class, IOrigin, IInnerProxy;
        IStoreRepository<TEntity> Use<TStore, TEntity>()
            where TStore : IDataServerStore
            where TEntity : class, IOrigin, IInnerProxy;
    }
}