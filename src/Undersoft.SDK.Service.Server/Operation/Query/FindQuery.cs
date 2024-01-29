using Undersoft.SDK.Uniques;
using System.Linq.Expressions;

namespace Undersoft.SDK.Service.Server.Operation.Query;

using Undersoft.SDK;
using Undersoft.SDK.Service.Data.Object;
using Undersoft.SDK.Service.Data.Store;

public class FindQuery<TStore, TEntity, TDto> : Query<TStore, TEntity, IQueryable<TDto>>
    where TEntity : class, IOrigin, IInnerProxy
    where TStore : IDataServerStore
    where TDto : class, IIdentifiable
{
    public FindQuery(params object[] keys) : base(keys) { }

    public FindQuery(object[] keys, params Expression<Func<TEntity, object>>[] expanders)
        : base(keys, expanders) { }

    public FindQuery(Expression<Func<TEntity, bool>> predicate) : base(predicate) { }

    public FindQuery(
        Expression<Func<TEntity, bool>> predicate,
        params Expression<Func<TEntity, object>>[] expanders
    ) : base(predicate, expanders) { }
}
