using Undersoft.SDK.Service.Data.Object;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Undersoft.SDK.Service.Application.Operation.Command;

public class CreateSet<TStore, TEntity, TDto> : CommandSet<TDto>
    where TEntity : class, IDataObject
    where TDto : class, IDataObject
    where TStore : IDatabaseStore
{
    [JsonIgnore]
    public Func<TEntity, Expression<Func<TEntity, bool>>> Predicate { get; }

    public CreateSet(EventPublishMode publishPattern, TDto input, object key)
        : base(
            CommandMode.Create,
            publishPattern,
            new[] { new Create<TStore, TEntity, TDto>(publishPattern, input, key) }
        ) { }

    public CreateSet(EventPublishMode publishPattern, TDto[] inputs)
        : base(
            CommandMode.Create,
            publishPattern,
            inputs
                .Select(input => new Create<TStore, TEntity, TDto>(publishPattern, input))
                .ToArray()
        ) { }

    public CreateSet(
        EventPublishMode publishPattern,
        TDto[] inputs,
        Func<TEntity, Expression<Func<TEntity, bool>>> predicate
    )
        : base(
            CommandMode.Create,
            publishPattern,
            inputs
                .Select(
                    input => new Create<TStore, TEntity, TDto>(publishPattern, input, predicate)
                )
                .ToArray()
        )
    {
        Predicate = predicate;
    }
}
