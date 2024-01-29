using System.Linq.Expressions;

namespace Undersoft.SDK.Service.Server.Operation.Query;

using Undersoft.SDK.Service.Data.Object;
using Undersoft.SDK.Service.Data.Store;

public class Find<TStore, TEntity, TDto> : Query<TStore, TEntity, TDto>
    where TEntity : class, IOrigin, IInnerProxy
    where TStore : IDataServerStore
{
    public Find(params object[] keys) : base(keys) { }

    public Find(object[] keys, params Expression<Func<TEntity, object>>[] expanders)
        : base(keys, expanders) { }

    public Find(Expression<Func<TEntity, bool>> predicate) : base(predicate) { }

    public Find(
        Expression<Func<TEntity, bool>> predicate,
        params Expression<Func<TEntity, object>>[] expanders
    ) : base(predicate, expanders) { }
}
