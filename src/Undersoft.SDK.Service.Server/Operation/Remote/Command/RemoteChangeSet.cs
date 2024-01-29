using System.Linq.Expressions;
using System.Text.Json.Serialization;
using Undersoft.SDK.Service.Server.Operation.Command;
using Undersoft.SDK.Service.Data.Event;
using Undersoft.SDK.Service.Data.Store;

namespace Undersoft.SDK.Service.Server.Operation.Remote.Command;

public class RemoteChangeSet<TStore, TDto, TModel> : RemoteCommandSet<TModel>
    where TDto : class, IOrigin, IInnerProxy
    where TModel : class, IOrigin, IInnerProxy
    where TStore : IDataServiceStore
{
    [JsonIgnore]
    public Func<TModel, Expression<Func<TDto, bool>>> Predicate { get; }

    public RemoteChangeSet(EventPublishMode publishPattern, TModel input, object key)
        : base(
            CommandMode.Change,
            publishPattern,
            new[] { new RemoteChange<TStore, TDto, TModel>(publishPattern, input, key) }
        )
    { }

    public RemoteChangeSet(EventPublishMode publishPattern, TModel[] inputs)
        : base(
            CommandMode.Change,
            publishPattern,
            inputs.Select(c => new RemoteChange<TStore, TDto, TModel>(publishPattern, c, c.Id)).ToArray()
        )
    { }

    public RemoteChangeSet(
        EventPublishMode publishPattern,
        TModel[] inputs,
        Func<TModel, Expression<Func<TDto, bool>>> predicate
    )
        : base(
            CommandMode.Change,
            publishPattern,
            inputs
                .Select(c => new RemoteChange<TStore, TDto, TModel>(publishPattern, c, predicate))
                .ToArray()
        )
    {
        Predicate = predicate;
    }
}
