using Undersoft.SDK.Uniques;
using System.Linq.Expressions;

namespace Undersoft.SDK.Service.Application.Operation.Query;

using Undersoft.SDK;

using Undersoft.SDK.Service.Data.Object;

public class FindQuery<TStore, TEntity, TDto> : Query<TStore, TEntity, IQueryable<TDto>>
    where TEntity : class, IDataObject
    where TStore : IDatabaseStore
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
