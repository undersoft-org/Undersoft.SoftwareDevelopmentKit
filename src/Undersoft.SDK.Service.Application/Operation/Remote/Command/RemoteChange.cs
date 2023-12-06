using System.Linq.Expressions;
using System.Text.Json.Serialization;
using Undersoft.SDK.Service.Application.Operation.Command;



namespace Undersoft.SDK.Service.Application.Operation.Remote.Command;

public class RemoteChange<TStore, TDto, TModel> : RemoteCommand<TModel>
    where TDto : class, IDataObject
    where TModel : class, IDataObject
    where TStore : IDataServiceStore
{
    [JsonIgnore]
    public Func<TModel, Expression<Func<TDto, bool>>> Predicate { get; }

    public RemoteChange(EventPublishMode publishMode, TModel input, params object[] keys)
        : base(CommandMode.Change, publishMode, input, keys) { }

    public RemoteChange(
        EventPublishMode publishMode,
        TModel input,
        Func<TModel, Expression<Func<TDto, bool>>> predicate
    ) : base(CommandMode.Change, publishMode, input)
    {
        Predicate = predicate;
    }
}
