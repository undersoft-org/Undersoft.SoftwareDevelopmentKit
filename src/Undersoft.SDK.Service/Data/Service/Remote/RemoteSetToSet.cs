using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;

namespace Undersoft.SDK.Service.Data.Service.Remote;

using Uniques;
using Entity;

public class RemoteSetToSet<TOrigin, TTarget, TMiddle> : ServiceRemote<TOrigin, TTarget, TMiddle> where TOrigin : class, IUniqueIdentifiable where TMiddle : class, IUniqueIdentifiable where TTarget : class, IUniqueIdentifiable
{
    private Expression<Func<TTarget, object>> targetKey;
    private Func<TMiddle, object> middleKey;
    private Func<TOrigin, IEnumerable<TMiddle>> middleSet;

    public RemoteSetToSet() : base()
    {
    }
    public RemoteSetToSet(Expression<Func<TOrigin, IEnumerable<TMiddle>>> middleset,
                               Expression<Func<TMiddle, object>> middlekey,
                               Expression<Func<TTarget, object>> targetkey) : base()
    {
        Towards = Towards.SetToSet;
        MiddleSet = middleset;
        MiddleKey = middlekey;
        TargetKey = targetkey;

        middleKey = middlekey.Compile();
        targetKey = targetkey;
        middleSet = middleset.Compile();

        Predicate = (o) =>
        {
            var ids = (IEnumerable<TMiddle>)o.Proxy[MiddleSet.GetMemberName()];

            return LinqExtension.GetWhereInExpression(TargetKey, ids?.Select(middleKey));
        };
    }

    public override Expression<Func<TTarget, bool>> CreatePredicate(object entity)
    {
        var ids = (IEnumerable<TMiddle>)((IInnerProxy)entity).Proxy[MiddleSet.GetMemberName()];

        return LinqExtension.GetWhereInExpression(TargetKey, ids?.Select(middleKey));
    }

}
