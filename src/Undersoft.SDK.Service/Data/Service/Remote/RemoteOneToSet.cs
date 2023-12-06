using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;

namespace Undersoft.SDK.Service.Data.Service.Remote;

using Uniques;

public class RemoteOneToSet<TOrigin, TTarget> : ServiceRemote<TOrigin, TTarget, IRemoteSet<TTarget>> where TOrigin : class, IUniqueIdentifiable where TTarget : class, IUniqueIdentifiable
{
    private Func<TTarget, object> targetKey;
    private Func<TOrigin, object> originKey;

    public RemoteOneToSet() : base()
    {
    }
    public RemoteOneToSet(Expression<Func<TOrigin, object>> originkey,
                            Expression<Func<TTarget, object>> targetkey)
                                : base()
    {
        Towards = Towards.ToSet;
        OriginKey = originkey;
        TargetKey = targetkey;

        originKey = originkey.Compile();
        targetKey = targetkey.Compile();
    }

    public override Expression<Func<TTarget, bool>> CreatePredicate(object entity)
    {
        return LinqExtension.GetEqualityExpression(TargetKey, originKey, (TOrigin)entity);
    }
}
