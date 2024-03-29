﻿using Undersoft.SDK.Series;
using System.Linq.Expressions;

namespace Undersoft.SDK.Service.Server.Operation.Query;

using Undersoft.SDK.Service.Data.Object;
using Undersoft.SDK.Service.Data.Query;
using Undersoft.SDK.Service.Data.Store;

public class Filter<TStore, TEntity, TDto> : Query<TStore, TEntity, ISeries<TDto>>
    where TEntity : class, IOrigin, IInnerProxy
    where TStore : IDataServerStore
{
    public Filter(int offset, int limit, Expression<Func<TEntity, bool>> predicate)
        : base(predicate)
    {
        Offset = offset;
        Limit = limit;
    }

    public Filter(
        int offset,
        int limit,
        Expression<Func<TEntity, bool>> predicate,
        params Expression<Func<TEntity, object>>[] expanders
    ) : base(predicate, expanders)
    {
        Offset = offset;
        Limit = limit;
    }

    public Filter(
        int offset,
        int limit,
        Expression<Func<TEntity, bool>> predicate,
        SortExpression<TEntity> sortTerms,
        params Expression<Func<TEntity, object>>[] expanders
    ) : base(predicate, sortTerms, expanders)
    {
        Offset = offset;
        Limit = limit;
    }
}
