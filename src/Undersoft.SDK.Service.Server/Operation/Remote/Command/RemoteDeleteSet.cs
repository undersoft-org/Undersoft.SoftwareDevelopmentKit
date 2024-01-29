using System.Linq.Expressions;
using System.Text.Json.Serialization;
using Undersoft.SDK.Service.Server.Operation.Command;
using Undersoft.SDK.Service.Data.Event;
using Undersoft.SDK.Service.Data.Store;

namespace Undersoft.SDK.Service.Server.Operation.Remote.Command;

public class RemoteDeleteSet<TStore, TDto, TModel> : RemoteCommandSet<TModel>
    where TDto : class, IOrigin, IInnerProxy
    where TModel : class, IOrigin, IInnerProxy
    where TStore : IDataServiceStore
{
    [JsonIgnore]
    public Func<TDto, Expression<Func<TDto, bool>>> Predicate { get; }

    public RemoteDeleteSet(EventPublishMode publishPattern, object key)
        : base(
            CommandMode.Create,
            publishPattern,
            new[] { new RemoteDelete<TStore, TDto, TModel>(publishPattern, key) }
        )
    { }

    public RemoteDeleteSet(EventPublishMode publishPattern, TModel input, object key)
        : base(
            CommandMode.Create,
            publishPattern,
            new[] { new RemoteDelete<TStore, TDto, TModel>(publishPattern, input, key) }
        )
    { }

    public RemoteDeleteSet(EventPublishMode publishPattern, TModel[] inputs)
        : base(
            CommandMode.Delete,
            publishPattern,
            inputs
                .Select(input => new RemoteDelete<TStore, TDto, TModel>(publishPattern, input))
                .ToArray()
        )
    { }

    public RemoteDeleteSet(
        EventPublishMode publishPattern,
        TModel[] inputs,
        Func<TDto, Expression<Func<TDto, bool>>> predicate
    )
        : base(
            CommandMode.Delete,
            publishPattern,
            inputs
                .Select(
                    input => new RemoteDelete<TStore, TDto, TModel>(publishPattern, input, predicate)
                )
                .ToArray()
        )
    {
        Predicate = predicate;
    }
}
