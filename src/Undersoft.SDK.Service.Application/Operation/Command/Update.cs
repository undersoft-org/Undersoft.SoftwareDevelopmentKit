using Undersoft.SDK.Service.Data.Object;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Undersoft.SDK.Service.Application.Operation.Command;

public class Update<TStore, TEntity, TDto> : Command<TDto>
    where TEntity : class, IDataObject
    where TDto : class, IDataObject
    where TStore : IDatabaseStore
{
    [JsonIgnore]
    public Func<TEntity, Expression<Func<TEntity, bool>>> Predicate { get; }

    [JsonIgnore]
    public Func<TEntity, Expression<Func<TEntity, bool>>>[] Conditions { get; }

    public Update(EventPublishMode publishPattern, TDto input, params object[] keys)
        : base(CommandMode.Update, publishPattern, input, keys) { }

    public Update(
        EventPublishMode publishPattern,
        TDto input,
        Func<TEntity, Expression<Func<TEntity, bool>>> predicate
    ) : base(CommandMode.Update, publishPattern, input)
    {
        Predicate = predicate;
    }

    public Update(
        EventPublishMode publishPattern,
        TDto input,
        Func<TEntity, Expression<Func<TEntity, bool>>> predicate,
        params Func<TEntity, Expression<Func<TEntity, bool>>>[] conditions
    ) : base(CommandMode.Update, publishPattern, input)
    {
        Predicate = predicate;
        Conditions = conditions;
    }
}
