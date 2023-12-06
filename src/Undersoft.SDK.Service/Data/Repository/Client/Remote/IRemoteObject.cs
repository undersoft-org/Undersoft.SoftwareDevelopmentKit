using Microsoft.EntityFrameworkCore.ChangeTracking;
using Undersoft.SDK.Service.Data.Store;
using System.Threading.Tasks;

namespace Undersoft.SDK.Service.Data.Repository.Client.Remote;

using Entity;
using Undersoft.SDK;
using Undersoft.SDK.Service.Data.Repository;
using Undersoft.SDK.Service.Data.Service.Remote;

public interface IRemoteObject<TStore, TOrigin> : IRemoteObject<TOrigin> where TOrigin : class, IOrigin where TStore : IDataServiceStore
{
}

public interface IRemoteObject<TOrigin> : IRemoteObject where TOrigin : class, IOrigin
{
}

public interface IRemoteObject : IRepository, IServiceRemote
{
    bool IsLinked { get; set; }

    IRepository Host { get; set; }

    void Load(object origin);

    Task LoadAsync(object origin);

    void LinkTrigger(object sender, EntityEntryEventArgs e);

    IRemoteSynchronizer Synchronizer { get; }
}
