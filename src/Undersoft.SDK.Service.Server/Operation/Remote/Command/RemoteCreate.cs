using System.Linq.Expressions;
using System.Text.Json.Serialization;
using Undersoft.SDK.Service.Server.Operation.Command;
using Undersoft.SDK.Service.Data.Event;
using Undersoft.SDK.Service.Data.Store;

namespace Undersoft.SDK.Service.Server.Operation.Remote.Command;

public class RemoteCreate<TStore, TDto, TModel> : RemoteCommand<TModel>
    where TDto : class, IOrigin, IInnerProxy
    where TModel : class, IOrigin, IInnerProxy
    where TStore : IDataServiceStore
{
    [JsonIgnore]
    public Func<TDto, Expression<Func<TDto, bool>>> Predicate { get; }

    public RemoteCreate(EventPublishMode publishPattern, TModel input)
        : base(CommandMode.Create, publishPattern, input)
    {
        input.AutoId();
    }

    public RemoteCreate(EventPublishMode publishPattern, TModel input, object key)
        : base(CommandMode.Create, publishPattern, input)
    {
        input.SetId(key);
    }

    public RemoteCreate(
        EventPublishMode publishPattern,
        TModel input,
        Func<TDto, Expression<Func<TDto, bool>>> predicate
    ) : base(CommandMode.Create, publishPattern, input)
    {
        input.AutoId();
        Predicate = predicate;
    }
}
