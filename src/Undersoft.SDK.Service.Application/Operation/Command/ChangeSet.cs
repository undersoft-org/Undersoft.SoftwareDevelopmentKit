using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Undersoft.SDK.Service.Application.Operation.Command;

using Undersoft.SDK.Service.Data.Event;

using Undersoft.SDK.Service.Data.Object;
using SDK.Service.Data.Store;

public class ChangeSet<TStore, TEntity, TDto> : CommandSet<TDto>
    where TEntity : class, IDataObject
    where TDto : class, IDataObject
    where TStore : IDatabaseStore
{
    [JsonIgnore]
    public Func<TDto, Expression<Func<TEntity, bool>>> Predicate { get; }

    public ChangeSet(EventPublishMode publishPattern, TDto input, object key)
        : base(
            CommandMode.Change,
            publishPattern,
            new[] { new Change<TStore, TEntity, TDto>(publishPattern, input, key) }
        ) { }

    public ChangeSet(EventPublishMode publishPattern, TDto[] inputs)
        : base(
            CommandMode.Change,
            publishPattern,
            inputs.Select(c => new Change<TStore, TEntity, TDto>(publishPattern, c, c.Id)).ToArray()
        ) { }

    public ChangeSet(
        EventPublishMode publishPattern,
        TDto[] inputs,
        Func<TDto, Expression<Func<TEntity, bool>>> predicate
    )
        : base(
            CommandMode.Change,
            publishPattern,
            inputs
                .Select(c => new Change<TStore, TEntity, TDto>(publishPattern, c, predicate))
                .ToArray()
        )
    {
        Predicate = predicate;
    }
}
