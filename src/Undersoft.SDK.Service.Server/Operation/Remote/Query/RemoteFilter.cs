using Undersoft.SDK.Series;

using System.Linq.Expressions;
using Undersoft.SDK.Service.Data.Store;

namespace Undersoft.SDK.Service.Server.Operation.Remote.Query;

public class RemoteFilter<TStore, TDto, TModel> : RemoteQuery<TStore, TDto, ISeries<TModel>>
    where TDto : class, IOrigin, IInnerProxy
    where TStore : IDataServiceStore
{
    public RemoteFilter(int offset, int limit, Expression<Func<TDto, bool>> predicate)
        : base(predicate)
    {
        Offset = offset;
        Limit = limit;
    }

    public RemoteFilter(
        int offset,
        int limit,
        Expression<Func<TDto, bool>> predicate,
        params Expression<Func<TDto, object>>[] expanders
    ) : base(predicate, expanders)
    {
        Offset = offset;
        Limit = limit;
    }

    public RemoteFilter(
        int offset,
        int limit,
        Expression<Func<TDto, bool>> predicate,
        SortExpression<TDto> sortTerms,
        params Expression<Func<TDto, object>>[] expanders
    ) : base(predicate, sortTerms, expanders)
    {
        Offset = offset;
        Limit = limit;
    }
}
