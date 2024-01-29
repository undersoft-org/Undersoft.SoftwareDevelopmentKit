using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Undersoft.SDK.Service.Server.Operation.Command;

using Undersoft.SDK.Service.Data.Object;

using Undersoft.SDK.Service.Data.Event;
using Undersoft.SDK.Service.Data.Store;

public class Change<TStore, TEntity, TDto> : Command<TDto>
    where TEntity : class, IOrigin, IInnerProxy
    where TDto : class, IOrigin, IInnerProxy
    where TStore : IDataServerStore
{
    [JsonIgnore]
    public Func<TDto, Expression<Func<TEntity, bool>>> Predicate { get; }

    public Change(EventPublishMode publishMode, TDto input, params object[] keys)
        : base(CommandMode.Change, publishMode, input, keys) { }

    public Change(
        EventPublishMode publishMode,
        TDto input,
        Func<TDto, Expression<Func<TEntity, bool>>> predicate
    ) : base(CommandMode.Change, publishMode, input)
    {
        Predicate = predicate;
    }
}
