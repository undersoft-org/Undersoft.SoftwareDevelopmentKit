using Undersoft.SDK.Series;
using System.Linq.Expressions;

namespace Undersoft.SDK.Service.Server.Operation.Query;

using Undersoft.SDK.Service.Data.Object;
using Undersoft.SDK.Service.Data.Query;
using Undersoft.SDK.Service.Data.Store;

public class Get<TStore, TEntity, TDto> : Query<TStore, TEntity, ISeries<TDto>>
    where TDto : class
    where TEntity : class, IOrigin, IInnerProxy
    where TStore : IDataServerStore
{
    public Get(int offset, int limit, params Expression<Func<TEntity, object>>[] expanders)
        : base(expanders)
    {
        Offset = offset;
        Limit = limit;
    }

    public Get(
        int offset,
        int limit,
        SortExpression<TEntity> sortTerms,
        params Expression<Func<TEntity, object>>[] expanders
    ) : base(sortTerms, expanders)
    {
        Offset = offset;
        Limit = limit;
    }

    public Get(
       int offset,
       int limit,
       Expression<Func<TEntity, bool>> predicate,
       SortExpression<TEntity> sortTerms,
       params Expression<Func<TEntity, object>>[] expanders
   ) : base(predicate, sortTerms, expanders)
    {
        Offset = offset;
        Limit = limit;
    }
}
