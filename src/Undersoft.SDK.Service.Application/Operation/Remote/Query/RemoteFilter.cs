using Undersoft.SDK.Series;

using System.Linq.Expressions;

namespace Undersoft.SDK.Service.Application.Operation.Remote.Query;

public class RemoteFilter<TStore, TDto, TModel> : RemoteQuery<TStore, TDto, ISeries<TModel>>
    where TDto : class, IDataObject
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
