using MediatR;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Undersoft.SDK.Service.Server.Operation.Query;

using Undersoft.SDK.Service.Data.Object;
using Undersoft.SDK.Service.Data.Query;
using Undersoft.SDK.Service.Data.Repository;
using Undersoft.SDK.Service.Data.Store;

public abstract class Query<TStore, TEntity, TResult> : IRequest<TResult>, IQuery<TEntity>
    where TEntity : class, IOrigin, IInnerProxy
    where TStore : IDataServerStore
{
    public int Offset { get; set; } = 0;

    public int Limit { get; set; } = 0;

    public int Count { get; set; } = 0;

    public Delegate Processings { get; set; }

    public Func<IRepository<TEntity>, IQueryable<TEntity>> Transformations 
        => (Func<IRepository<TEntity>, IQueryable<TEntity>>)Processings;

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

    public Query(Func<IRepository<TEntity>, IQueryable<TEntity>> transformations)
    {
        Processings = transformations;
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
