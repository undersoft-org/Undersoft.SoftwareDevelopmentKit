using Microsoft.EntityFrameworkCore.ChangeTracking;
using Undersoft.SDK.Service.Data.Mapper;
using Undersoft.SDK.Service.Data.Repository.Client.Remote;
using Undersoft.SDK.Service.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Undersoft.SDK.Service.Data.Repository
{
    public interface IRepository : IRepositoryContext
    {
        Type ElementType { get; }

        Expression Expression { get; }

        IQueryProvider Provider { get; }

        IDataMapper Mapper { get; }

        CancellationToken Cancellation { get; set; }

        IEnumerable<IRemoteObject> RemoteObjects { get; set; }

        void LoadRemote(object entity);

        Task LoadRemoteAsync(object entity);

        void LoadRelated(EntityEntry entry, RelatedType relatedType);

        void LoadRelatedAsync(EntityEntry entry, RelatedType relatedType, CancellationToken token);
    }
}