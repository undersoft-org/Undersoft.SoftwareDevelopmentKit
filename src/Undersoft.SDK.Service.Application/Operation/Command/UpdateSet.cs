using Undersoft.SDK.Service.Data.Object;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Undersoft.SDK.Service.Application.Operation.Command;

public class UpdateSet<TStore, TEntity, TDto> : CommandSet<TDto>
    where TEntity : class, IDataObject
    where TDto : class, IDataObject
    where TStore : IDatabaseStore
{
    [JsonIgnore]
    public Func<TDto, Expression<Func<TEntity, bool>>> Predicate { get; }

    [JsonIgnore]
    public Func<TDto, Expression<Func<TEntity, bool>>>[] Conditions { get; }

    public UpdateSet(EventPublishMode publishPattern, TDto input, object key)
        : base(
            CommandMode.Change,
            publishPattern,
            new[] { new Update<TStore, TEntity, TDto>(publishPattern, input, key) }
        ) { }

    public UpdateSet(EventPublishMode publishPattern, TDto[] inputs)
        : base(
            CommandMode.Update,
            publishPattern,
            inputs
                .Select(input => new Update<TStore, TEntity, TDto>(publishPattern, input))
                .ToArray()
        ) { }

    public UpdateSet(
        EventPublishMode publishPattern,
        TDto[] inputs,
        Func<TDto, Expression<Func<TEntity, bool>>> predicate
    )
        : base(
            CommandMode.Update,
            publishPattern,
            inputs
                .Select(
                    input => new Update<TStore, TEntity, TDto>(publishPattern, input, predicate)
                )
                .ToArray()
        )
    {
        Predicate = predicate;
    }

    public UpdateSet(
        EventPublishMode publishPattern,
        TDto[] inputs,
        Func<TDto, Expression<Func<TEntity, bool>>> predicate,
        params Func<TDto, Expression<Func<TEntity, bool>>>[] conditions
    )
        : base(
            CommandMode.Update,
            publishPattern,
            inputs
                .Select(
                    input =>
                        new Update<TStore, TEntity, TDto>(
                            publishPattern,
                            input,
                            predicate,
                            conditions
                        )
                )
                .ToArray()
        )
    {
        Predicate = predicate;
        Conditions = conditions;
    }
}
