using MediatR;
using System.Linq.Expressions;

namespace Undersoft.SDK.Service.Application.Operation.Query;


using Undersoft.SDK.Service.Data.Object;
using Undersoft.SDK.Service.Data.Query;

public class GetAsync<TStore, TEntity, TDto> : Get<TStore, TEntity, TDto>, IStreamRequest<TDto>
    where TEntity : class, IDataObject
    where TStore : IDatabaseStore
{
    public GetAsync(int offset, int limit, params Expression<Func<TEntity, object>>[] expanders)
        : base(offset, limit, expanders) { }

    public GetAsync(
        int offset,
        int limit,
        SortExpression<TEntity> sortTerms,
        params Expression<Func<TEntity, object>>[] expanders
    ) : base(offset, limit, sortTerms, expanders) { }
}
