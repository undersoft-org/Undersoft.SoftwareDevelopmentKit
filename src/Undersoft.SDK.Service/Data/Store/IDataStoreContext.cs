using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;

namespace Undersoft.SDK.Service.Data.Store;

using Uniques;

public interface IDataStoreContext<TStore> : IDataStoreContext where TStore : IDatabaseStore { }

public interface IDataStoreContext : IResettableService, IDisposable, IAsyncDisposable
{
    object EntitySet<TEntity>() where TEntity : class, IUniqueIdentifiable;

    object EntitySet(Type entityType);

    Task<int> Save(bool asTransaction, CancellationToken token = default);
}
