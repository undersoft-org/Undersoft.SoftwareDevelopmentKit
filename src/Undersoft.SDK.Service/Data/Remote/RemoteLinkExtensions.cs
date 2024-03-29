﻿using System.Linq.Expressions;

namespace Undersoft.SDK.Service.Data.Remote;
using Undersoft.SDK.Service.Data.Client;
using Uniques;

public static class RemoteLinkExtensions
{
    public static OpenDataContext RemoteSetToSet<TOrigin, TTarget>(this OpenDataContext context,
                                                             Expression<Func<IRemoteLink<TOrigin, TTarget>, object>> middlekey,
                                                             Expression<Func<TTarget, object>> targetkey)
                                                          where TOrigin : class, IOrigin, IInnerProxy
                                                          where TTarget : class, IOrigin, IInnerProxy
    {
        var remote = new RemoteSetToSet<TOrigin, TTarget>(middlekey, targetkey);
        context.Remotes.Put(remote.TypeId, remote);
        context.Remotes.Put(typeof(TOrigin), remote);
        return context;
    }

    public static OpenDataContext RemoteOneToSet<TOrigin, TTarget>(this OpenDataContext context,
                                                             Expression<Func<TOrigin, object>> originkey,
                                                             Expression<Func<TTarget, object>> targetkey)
                                                         where TOrigin : class, IUniqueIdentifiable
                                                         where TTarget : class, IUniqueIdentifiable
    {

        var remote = new RemoteOneToSet<TOrigin, TTarget>(originkey, targetkey);
        context.Remotes.Put(remote.TypeId, remote);
        context.Remotes.Put(typeof(TOrigin), remote);
        return context;
    }

    public static OpenDataContext RemoteOneToOne<TOrigin, TTarget>(this OpenDataContext context,
                                                            Expression<Func<TOrigin, object>> originkey,
                                                             Expression<Func<TTarget, object>> targetkey)
                                                        where TOrigin : class, IUniqueIdentifiable
                                                        where TTarget : class, IUniqueIdentifiable
    {
        var remote = new RemoteOneToOne<TOrigin, TTarget>(originkey, targetkey);
        context.Remotes.Put(remote.TypeId, remote);
        context.Remotes.Put(typeof(TOrigin), remote);
        return context;
    }

}

