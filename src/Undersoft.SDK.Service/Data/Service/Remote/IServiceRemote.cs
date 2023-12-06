using Undersoft.SDK.Instant;
using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Undersoft.SDK.Service.Data.Service.Remote;

using Uniques;
using Instant.Rubrics;
using Undersoft.SDK;

public interface IServiceRemote : IUnique
{
    Towards Towards { get; set; }

    MemberRubric RemoteMember { get; }
}

public interface IServiceRemote<TOrigin, TTarget> : IServiceRemote where TOrigin : class, IOrigin where TTarget : class, IOrigin
{
    Expression<Func<TOrigin, object>> OriginKey { get; set; }
    Expression<Func<TTarget, object>> TargetKey { get; set; }

    Func<TOrigin, Expression<Func<TTarget, bool>>> Predicate { get; set; }

    Expression<Func<TTarget, bool>> CreatePredicate(object entity);
}

public interface IServiceRemote<TOrigin, TTarget, TMiddle> : IServiceRemote<TOrigin, TTarget> where TOrigin : class, IOrigin where TTarget : class, IOrigin
{
    Expression<Func<TMiddle, object>> MiddleKey { get; set; }

    Expression<Func<TOrigin, IEnumerable<TMiddle>>> MiddleSet { get; set; }
}
