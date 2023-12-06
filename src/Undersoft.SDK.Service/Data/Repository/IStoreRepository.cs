using Microsoft.EntityFrameworkCore.Storage;
using Undersoft.SDK.Series;
using Undersoft.SDK.Service.Data.Query;
using Undersoft.SDK.Service.Data.Repository.Pagination;
using Undersoft.SDK.Uniques;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Undersoft.SDK.Service.Data.Repository
{
    public interface IStoreRepository<TStore, TEntity> : IStoreRepository<TEntity> where TEntity : class, IOrigin
    {
    }

    public interface IStoreRepository<TEntity> : IRepository<TEntity> where TEntity : class, IOrigin
    {
        IDbContextTransaction BeginTransaction();
        Task<IDbContextTransaction> BeginTransactionAsync();
        void CommitTransaction(IDbContextTransaction transaction);
        Task CommitTransaction(Task<IDbContextTransaction> transaction);
    }
}