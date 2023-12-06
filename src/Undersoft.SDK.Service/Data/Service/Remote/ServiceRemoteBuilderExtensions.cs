using Microsoft.OData.Edm;
using System.Linq.Expressions;

namespace Undersoft.SDK.Service.Data.Service.Remote;

using Uniques;

public static class ServiceRemoteBuilderExtensions
{
    public static IEdmModel RemoteSetToSet<TOrigin, TMiddle, TTarget>(this IEdmModel builder,
                                                             Expression<Func<TOrigin, IEnumerable<TMiddle>>> middleSet,
                                                             Expression<Func<TMiddle, object>> middlekey,
                                                             Expression<Func<TTarget, object>> targetkey)
                                                          where TOrigin : class, IUniqueIdentifiable
                                                          where TTarget : class, IUniqueIdentifiable
                                                           where TMiddle : class, IUniqueIdentifiable
    {
        new RemoteSetToSet<TOrigin, TTarget, TMiddle>(middleSet, middlekey, targetkey);
        return builder;
    }

    public static IEdmModel RemoteOneToSet<TOrigin, TTarget>(this IEdmModel builder,
                                                             Expression<Func<TOrigin, object>> originkey,
                                                             Expression<Func<TTarget, object>> targetkey)
                                                         where TOrigin : class, IUniqueIdentifiable
                                                         where TTarget : class, IUniqueIdentifiable
    {
        new RemoteOneToSet<TOrigin, TTarget>(originkey, targetkey);
        return builder;
    }

    public static IEdmModel RemoteOneToOne<TOrigin, TTarget>(this IEdmModel builder,
                                                            Expression<Func<TOrigin, object>> originkey,
                                                             Expression<Func<TTarget, object>> targetkey)
                                                        where TOrigin : class, IUniqueIdentifiable
                                                        where TTarget : class, IUniqueIdentifiable
    {
        new RemoteOneToOne<TOrigin, TTarget>(originkey, targetkey);
        return builder;
    }

}

