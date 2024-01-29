using System.Linq.Expressions;
using System.Text.Json.Serialization;
using Undersoft.SDK.Service.Server.Operation.Command;
using Undersoft.SDK.Service.Data.Event;
using Undersoft.SDK.Service.Data.Store;

namespace Undersoft.SDK.Service.Server.Operation.Remote.Command;

public class RemoteDelete<TStore, TDto, TModel> : RemoteCommand<TModel>
    where TDto : class, IOrigin, IInnerProxy
    where TModel : class, IOrigin, IInnerProxy
    where TStore : IDataServiceStore
{
    [JsonIgnore]
    public Func<TDto, Expression<Func<TDto, bool>>> Predicate { get; }

    public RemoteDelete(EventPublishMode publishPattern, TModel input)
        : base(CommandMode.Delete, publishPattern, input) { }

    public RemoteDelete(
        EventPublishMode publishPattern,
        TModel input,
        Func<TDto, Expression<Func<TDto, bool>>> predicate
    ) : base(CommandMode.Delete, publishPattern, input)
    {
        Predicate = predicate;
    }

    public RemoteDelete(
        EventPublishMode publishPattern,
        Func<TDto, Expression<Func<TDto, bool>>> predicate
    ) : base(CommandMode.Delete, publishPattern)
    {
        Predicate = predicate;
    }

    public RemoteDelete(EventPublishMode publishPattern, params object[] keys)
        : base(CommandMode.Delete, publishPattern, keys) { }
}
