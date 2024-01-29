using MediatR;

using Undersoft.SDK.Service.Data.Object;
using System.Linq.Expressions;
using System.Text.Json.Serialization;
using Undersoft.SDK.Service.Data.Store;

namespace Undersoft.SDK.Service.Server.Operation.Remote.Query;

public abstract class RemoteQuery<TStore, TDto, TResult> : IRequest<TResult>, IRemoteQuery<TDto>
    where TDto : class, IOrigin, IInnerProxy
    where TStore : IDataServiceStore
{
    public int Offset { get; set; } = 0;

    public int Limit { get; set; } = 0;

    public int Count { get; set; } = 0;

    public Delegate Processings { get; set; }

    public object[] Keys { get; }

    [JsonIgnore]
    public SortExpression<TDto> Sort { get; }

    [JsonIgnore]
    public Expression<Func<TDto, object>>[] Expanders { get; }

    [JsonIgnore]
    public Expression<Func<TDto, bool>> Predicate { get; }

    public object Input => new object[] { Keys, Predicate, Expanders };

    public object Output => new object[] { Keys, Predicate, Expanders };

    public RemoteQuery(object[] keys)
    {
        Keys = keys;
    }

    public RemoteQuery(object[] keys, params Expression<Func<TDto, object>>[] expanders)
    {
        Keys = keys;
        Expanders = expanders;
    }

    public RemoteQuery(Expression<Func<TDto, bool>> predicate)
    {
        Predicate = predicate;
    }

    public RemoteQuery(params Expression<Func<TDto, object>>[] expanders)
    {
        Expanders = expanders;
    }

    public RemoteQuery(
        Expression<Func<TDto, bool>> predicate,
        params Expression<Func<TDto, object>>[] expanders
    )
    {
        Predicate = predicate;
        Expanders = expanders;
    }

    public RemoteQuery(
        SortExpression<TDto> sortTerms,
        params Expression<Func<TDto, object>>[] expanders
    )
    {
        Sort = sortTerms;
        Expanders = expanders;
    }

    public RemoteQuery(
        Expression<Func<TDto, bool>> predicate,
        SortExpression<TDto> sortTerms,
        params Expression<Func<TDto, object>>[] expanders
    )
    {
        Predicate = predicate;
        Sort = sortTerms;
        Expanders = expanders;
    }
}
