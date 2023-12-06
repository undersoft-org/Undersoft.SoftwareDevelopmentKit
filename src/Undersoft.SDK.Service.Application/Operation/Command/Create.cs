using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Undersoft.SDK.Service.Application.Operation.Command;

using Undersoft.SDK.Service.Data.Event;

using Undersoft.SDK.Service.Data.Object;
using SDK.Service.Data.Store;

public class Create<TStore, TEntity, TDto> : Command<TDto>
    where TEntity : class, IDataObject
    where TDto : class, IDataObject
    where TStore : IDatabaseStore
{
    [JsonIgnore]
    public Func<TEntity, Expression<Func<TEntity, bool>>> Predicate { get; }

    public Create(EventPublishMode publishPattern, TDto input)
        : base(CommandMode.Create, publishPattern, input)
    {
        input.AutoId();
    }

    public Create(EventPublishMode publishPattern, TDto input, object key)
        : base(CommandMode.Create, publishPattern, input)
    {
        input.SetId(key);
    }

    public Create(
        EventPublishMode publishPattern,
        TDto input,
        Func<TEntity, Expression<Func<TEntity, bool>>> predicate
    ) : base(CommandMode.Create, publishPattern, input)
    {
        input.AutoId();
        Predicate = predicate;
    }
}
