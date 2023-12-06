using MediatR;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Undersoft.SDK.Service.Application.Operation.Query;


using Undersoft.SDK.Service.Data.Object;
using Undersoft.SDK.Service.Data.Query;

public abstract class Query<TStore, TEntity, TResult> : IRequest<TResult>, IQuery<TEntity>
    where TEntity : class, IDataObject
    where TStore : IDatabaseStore
{
    public int Offset { get; set; } = 0;

    public int Limit { get; set; } = 0;

    public int Count { get; set; } = 0;

    public object[] Keys { get; }

    [JsonIgnore]
    public SortExpression<TEntity> Sort { get; }

    [JsonIgnore]
    public Expression<Func<TEntity, object>>[] Expanders { get; }

    [JsonIgnore]
    public Expression<Func<TEntity, bool>> Predicate { get; }

    public object Input => new object[] { Keys, Predicate, Expanders };

    public object Output => new object[] { Keys, Predicate, Expanders };

    public Query(object[] keys)
    {
        Keys = keys;
    }

    public Query(object[] keys, params Expression<Func<TEntity, object>>[] expanders)
    {
        Keys = keys;
        Expanders = expanders;
    }

    public Query(Expression<Func<TEntity, bool>> predicate)
    {
        Predicate = predicate;
    }

    public Query(params Expression<Func<TEntity, object>>[] expanders)
    {
        Expanders = expanders;
    }

    public Query(
        Expression<Func<TEntity, bool>> predicate,
        params Expression<Func<TEntity, object>>[] expanders
    )
    {
        Predicate = predicate;
        Expanders = expanders;
    }

    public Query(
        SortExpression<TEntity> sortTerms,
        params Expression<Func<TEntity, object>>[] expanders
    )
    {
        Sort = sortTerms;
        Expanders = expanders;
    }

    public Query(
        Expression<Func<TEntity, bool>> predicate,
        SortExpression<TEntity> sortTerms,
        params Expression<Func<TEntity, object>>[] expanders
    )
    {
        Predicate = predicate;
        Sort = sortTerms;
        Expanders = expanders;
    }
}
