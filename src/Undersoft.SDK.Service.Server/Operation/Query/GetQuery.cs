using Undersoft.SDK.Service.Data.Object;
using System.Linq.Expressions;
using Undersoft.SDK.Service.Data.Repository;
using Undersoft.SDK.Service.Data.Store;

namespace Undersoft.SDK.Service.Server.Operation.Query;

public class GetQuery<TStore, TEntity, TDto> : Query<TStore, TEntity, IQueryable<TDto>>
    where TEntity : class, IOrigin, IInnerProxy
    where TStore : IDataServerStore
{
    public GetQuery(Func<IRepository<TEntity>, IQueryable<TEntity>> transformations)
        : base(transformations) { }

    public GetQuery(params Expression<Func<TEntity, object>>[] expanders) : base(expanders) { }

    public GetQuery(
        SortExpression<TEntity> sortTerms,
        params Expression<Func<TEntity, object>>[] expanders
    ) : base(sortTerms, expanders) { }

    public GetQuery(
        Expression<Func<TEntity, bool>> predicate,
        SortExpression<TEntity> sortTerms,
        params Expression<Func<TEntity, object>>[] expanders
    ) : base(predicate, sortTerms, expanders) { }
}
