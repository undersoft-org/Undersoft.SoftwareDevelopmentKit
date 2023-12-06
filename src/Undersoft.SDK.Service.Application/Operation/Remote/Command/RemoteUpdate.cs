using System.Linq.Expressions;
using System.Text.Json.Serialization;
using Undersoft.SDK.Service.Application.Operation.Command;

namespace Undersoft.SDK.Service.Application.Operation.Remote.Command;

public class RemoteUpdate<TStore, TDto, TModel> : RemoteCommand<TModel>
    where TDto : class, IDataObject
    where TModel : class, IDataObject
    where TStore : IDataServiceStore
{
    [JsonIgnore]
    public Func<TDto, Expression<Func<TDto, bool>>> Predicate { get; }

    [JsonIgnore]
    public Func<TDto, Expression<Func<TDto, bool>>>[] Conditions { get; }

    public RemoteUpdate(EventPublishMode publishPattern, TModel input, params object[] keys)
        : base(CommandMode.Update, publishPattern, input, keys) { }

    public RemoteUpdate(
        EventPublishMode publishPattern,
        TModel input,
        Func<TDto, Expression<Func<TDto, bool>>> predicate
    ) : base(CommandMode.Update, publishPattern, input)
    {
        Predicate = predicate;
    }

    public RemoteUpdate(
        EventPublishMode publishPattern,
        TModel input,
        Func<TDto, Expression<Func<TDto, bool>>> predicate,
        params Func<TDto, Expression<Func<TDto, bool>>>[] conditions
    ) : base(CommandMode.Update, publishPattern, input)
    {
        Predicate = predicate;
        Conditions = conditions;
    }
}
