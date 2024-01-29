using System.Linq.Expressions;

namespace Undersoft.SDK.Service.Data.Remote;

public class RemoteSetToSet<TOrigin, TTarget> : RemoteRelation<TOrigin, TTarget>
    where TOrigin : class, IOrigin, IInnerProxy
    where TTarget : class, IOrigin, IInnerProxy
{
    private Expression<Func<TTarget, object>> _targetKey;
    private Func<IRemoteLink<TOrigin, TTarget>, object> _joinKey;
    private Func<TOrigin, IEnumerable<IRemoteLink<TOrigin, TTarget>>> _middleSet;

    public RemoteSetToSet() : base() { }

    public RemoteSetToSet(
        Expression<Func<IRemoteLink<TOrigin, TTarget>, object>> joinKey,
        Expression<Func<TTarget, object>> targetKey
    ) : base()
    {
        Towards = Towards.SetToSet;
        JoinKey = joinKey;
        TargetKey = targetKey;

        _joinKey = joinKey.Compile();
        _targetKey = targetKey;

        Predicate = (o) => CreatePredicate(o);
    }

    public override Expression<Func<TTarget, bool>> CreatePredicate(object entity)
    {
        var innerProxy = (IInnerProxy)entity;
        var joinRubric = innerProxy.Proxy.Rubrics
            .Where(r => r.RubricType == typeof(IRemoteLink<TOrigin, TTarget>))
            .FirstOrDefault();

        if (joinRubric == null)
            return null;

        return LinqExtension.GetWhereInExpression(
            TargetKey,
            ((IEnumerable<IRemoteLink<TOrigin, TTarget>>)innerProxy[joinRubric.RubricId])?.Select(_joinKey)
        );
    }
}
