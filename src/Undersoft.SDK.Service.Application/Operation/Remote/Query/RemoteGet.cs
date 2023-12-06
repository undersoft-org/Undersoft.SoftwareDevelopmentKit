using Undersoft.SDK.Series;

using System.Linq.Expressions;

namespace Undersoft.SDK.Service.Application.Operation.Remote.Query;

public class RemoteGet<TStore, TDto, TModel> : RemoteQuery<TStore, TDto, ISeries<TModel>>
    where TDto : class, IDataObject
    where TStore : IDataServiceStore
{
    public RemoteGet(int offset, int limit, params Expression<Func<TDto, object>>[] expanders)
        : base(expanders)
    {
        Offset = offset;
        Limit = limit;
    }

    public RemoteGet(
        int offset,
        int limit,
        SortExpression<TDto> sortTerms,
        params Expression<Func<TDto, object>>[] expanders
    ) : base(sortTerms, expanders)
    {
        Offset = offset;
        Limit = limit;
    }
}
