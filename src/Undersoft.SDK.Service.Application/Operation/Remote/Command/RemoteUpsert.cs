using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Undersoft.SDK.Service.Application.Operation.Command;

using Undersoft.SDK.Service.Application.Operation.Remote.Command;

using Undersoft.SDK.Service.Data.Event;

using SDK.Service.Data.Store;

public class RemoteUpsert<TStore, TDto, TModel> : RemoteCommand<TModel>
    where TDto : class, IDataObject
    where TModel : class, IDataObject
    where TStore : IDataServiceStore
{
    [JsonIgnore]
    public Func<TDto, Expression<Func<TDto, bool>>> Predicate { get; }

    [JsonIgnore]
    public Func<TDto, Expression<Func<TDto, bool>>>[] Conditions { get; }

    public RemoteUpsert(
        EventPublishMode publishPattern,
        TModel input,
        Func<TDto, Expression<Func<TDto, bool>>> predicate
    ) : base(CommandMode.Upsert, publishPattern, input)
    {
        Predicate = predicate;
    }

    public RemoteUpsert(
        EventPublishMode publishPattern,
        TModel input,
        Func<TDto, Expression<Func<TDto, bool>>> predicate,
        params Func<TDto, Expression<Func<TDto, bool>>>[] conditions
    ) : base(CommandMode.Upsert, publishPattern, input)
    {
        Predicate = predicate;
        Conditions = conditions;
    }
}
