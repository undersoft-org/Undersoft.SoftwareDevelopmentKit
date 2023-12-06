using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Undersoft.SDK.Service.Application.Operation.Command;

using Undersoft.SDK.Service.Data.Event;

using SDK.Service.Data.Store;

public class Upsert<TStore, TEntity, TDto> : Command<TDto>
    where TDto : class, IDataObject
    where TEntity : class, IDataObject
    where TStore : IDatabaseStore
{
    [JsonIgnore]
    public Func<TEntity, Expression<Func<TEntity, bool>>> Predicate { get; }

    [JsonIgnore]
    public Func<TEntity, Expression<Func<TEntity, bool>>>[] Conditions { get; }

    public Upsert(
        EventPublishMode publishPattern,
        TDto input,
        Func<TEntity, Expression<Func<TEntity, bool>>> predicate
    ) : base(CommandMode.Upsert, publishPattern, input)
    {
        Predicate = predicate;
    }

    public Upsert(
        EventPublishMode publishPattern,
        TDto input,
        Func<TEntity, Expression<Func<TEntity, bool>>> predicate,
        params Func<TEntity, Expression<Func<TEntity, bool>>>[] conditions
    ) : base(CommandMode.Upsert, publishPattern, input)
    {
        Predicate = predicate;
        Conditions = conditions;
    }
}
