using Undersoft.SDK.Service.Data.Object;
using System.Linq.Expressions;

namespace Undersoft.SDK.Service.Application.Operation.Query;

public class GetQuery<TStore, TEntity, TDto> : Query<TStore, TEntity, IQueryable<TDto>>
    where TEntity : class, IDataObject
    where TStore : IDatabaseStore
{
    public GetQuery(params Expression<Func<TEntity, object>>[] expanders) : base(expanders) { }

    public GetQuery(
        SortExpression<TEntity> sortTerms,
        params Expression<Func<TEntity, object>>[] expanders
    ) : base(sortTerms, expanders) { }

    public GetQuery(Expression<Func<TEntity, bool>> predicate,
    SortExpression<TEntity> sortTerms,
        params Expression<Func<TEntity, object>>[] expanders
    ) : base(predicate, sortTerms, expanders) { }
}
