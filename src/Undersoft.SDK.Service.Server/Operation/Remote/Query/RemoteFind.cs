using System.Linq.Expressions;
using Undersoft.SDK.Service.Data.Store;

namespace Undersoft.SDK.Service.Server.Operation.Remote.Query;

public class RemoteFind<TStore, TDto, TModel> : RemoteQuery<TStore, TDto, TModel>
    where TDto : class, IOrigin, IInnerProxy
    where TStore : IDataServiceStore
{
    public RemoteFind(params object[] keys) : base(keys) { }

    public RemoteFind(object[] keys, params Expression<Func<TDto, object>>[] expanders)
        : base(keys, expanders) { }

    public RemoteFind(Expression<Func<TDto, bool>> predicate) : base(predicate) { }

    public RemoteFind(
        Expression<Func<TDto, bool>> predicate,
        params Expression<Func<TDto, object>>[] expanders
    ) : base(predicate, expanders) { }
}
