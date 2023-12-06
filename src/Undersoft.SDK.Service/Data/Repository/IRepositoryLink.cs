using Undersoft.SDK.Service.Data.Store;

namespace Undersoft.SDK.Service.Data.Repository;

using Entity;
using Undersoft.SDK;
using Undersoft.SDK.Service.Data.Repository.Client.Remote;
using Undersoft.SDK.Service.Data.Service.Remote;

public interface IRepositoryLink<TStore, TOrigin, TTarget> : IRemoteRepository<TStore, TTarget>,
                 IServiceRemote<TOrigin, TTarget>, IRemoteObject<TStore, TOrigin>
                 where TOrigin : class, IOrigin where TTarget : class, IOrigin where TStore : IDataServiceStore
{
}
