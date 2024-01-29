using Microsoft.EntityFrameworkCore.ChangeTracking;
using Undersoft.SDK.Service.Data.Query;
using System.Linq.Expressions;
using Undersoft.SDK.Service.Data.Object;
using Undersoft.SDK.Service.Data.Repository.Pagination;

namespace Undersoft.SDK.Service.Data.Repository
{
    public interface IRepository<TEntity>
        : IRepositoryMapper<TEntity>,
            IRepositoryQuery<TEntity>,
            IRepositoryCommand<TEntity>,
            IRepositoryMappedCommand<TEntity>,
            IRepositoryIndexer<TEntity>,
            IPage<TEntity>,
            IRepository,
            IOrderedQueryable<TEntity>,
            IEnumerable<TEntity> where TEntity : class, IOrigin, IInnerProxy
    {
        IQueryable<TEntity> Query { get; }

        IQueryable<TEntity> AsQueryable();

        void LoadRemotesEvent(object sender, EntityEntryEventArgs e);

        TEntity Sign(TEntity entity);
        T Sign<T>(T entity) where T : IUniqueIdentifiable;

        TEntity Stamp(TEntity entity);
        T Stamp<T>(T entity) where T : IUniqueIdentifiable;
    }
}
