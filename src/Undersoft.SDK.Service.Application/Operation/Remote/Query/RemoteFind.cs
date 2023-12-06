using System.Linq.Expressions;


namespace Undersoft.SDK.Service.Application.Operation.Remote.Query;

public class RemoteFind<TStore, TDto, TModel> : RemoteQuery<TStore, TDto, TModel>
    where TDto : class, IDataObject
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
