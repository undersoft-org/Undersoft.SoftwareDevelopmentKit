using MediatR;
using Undersoft.SDK.Series;
using System.Linq.Expressions;

namespace Undersoft.SDK.Service.Application.Operation.Query;


using Undersoft.SDK.Service.Data.Object;
using Undersoft.SDK.Service.Data.Query;

public class FilterAsync<TStore, TEntity, TDto>
    : Filter<TStore, TEntity, ISeries<TDto>>,
        IStreamRequest<TDto>
    where TEntity : class, IDataObject
    where TStore : IDatabaseStore
{
    public FilterAsync(int offset, int limit, Expression<Func<TEntity, bool>> predicate)
        : base(offset, limit, predicate) { }

    public FilterAsync(
        int offset,
        int limit,
        Expression<Func<TEntity, bool>> predicate,
        params Expression<Func<TEntity, object>>[] expanders
    ) : base(offset, limit, predicate, expanders) { }

    public FilterAsync(
        int offset,
        int limit,
        Expression<Func<TEntity, bool>> predicate,
        SortExpression<TEntity> sortTerms,
        params Expression<Func<TEntity, object>>[] expanders
    ) : base(offset, limit, predicate, sortTerms, expanders) { }
}
