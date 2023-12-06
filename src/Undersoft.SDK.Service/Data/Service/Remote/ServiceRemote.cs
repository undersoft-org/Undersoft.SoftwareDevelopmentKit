using Undersoft.SDK.Service.Data.Service;
using Undersoft.SDK.Service.Data.Store;
using Undersoft.SDK.Uniques;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Undersoft.SDK.Service.Data.Service.Remote;

using Instant.Rubrics;
using Undersoft.SDK;

public abstract class ServiceRemote<TOrigin, TTarget, TMiddle> : ServiceRemoteBase, IServiceRemote<TOrigin, TTarget, TMiddle>
    where TOrigin : class, IOrigin where TTarget : class, IOrigin
{
    public ServiceRemote()
    {
        var key = typeof(TTarget).Name.UniqueBytes64();
        var seed = typeof(TOrigin).FullName.UniqueKey32();
        serialcode = new Uscn(key, seed);
        Name = typeof(TOrigin).Name + '_' + typeof(TTarget).Name;

        OpenDataServiceRegistry.Links.Add(TypeId, this);

        OpenDataServiceRegistry.Links.Add(typeof(TTarget).Name.UniqueKey64(seed), this);

        ServiceManager.GetManager().Registry.AddObject<IServiceRemote<TOrigin, TTarget>>(this);
    }

    public virtual string Name { get; set; }

    public virtual Expression<Func<TOrigin, object>> OriginKey { get; set; }
    public virtual Expression<Func<TMiddle, object>> MiddleKey { get; set; }
    public virtual Expression<Func<TTarget, object>> TargetKey { get; set; }

    public virtual Func<TOrigin, Expression<Func<TTarget, bool>>> Predicate { get; set; }

    public virtual Expression<Func<TOrigin, IEnumerable<TMiddle>>> MiddleSet { get; set; }

    public abstract Expression<Func<TTarget, bool>> CreatePredicate(object entity);

    public override MemberRubric RemoteMember => DataStoreRegistry.GetRemoteMember<TOrigin, TTarget>();

}
