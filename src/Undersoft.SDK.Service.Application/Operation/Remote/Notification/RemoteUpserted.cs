using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Undersoft.SDK.Service.Application.Operation.Notification;

using Command;
using Undersoft.SDK.Service.Application.Operation.Remote.Command;
using Undersoft.SDK.Service.Application.Operation.Remote.Notification;

using Undersoft.SDK.Service.Data.Entity;

using SDK.Service.Data.Store;

public class RemoteUpserted<TStore, TDto, TModel> : RemoteNotification<RemoteCommand<TModel>>
    where TDto : class, IDataObject
    where TModel : class, IDataObject
    where TStore : IDataServiceStore
{
    [JsonIgnore]
    public Func<TDto, Expression<Func<TDto, bool>>> Predicate { get; }

    [JsonIgnore]
    public Func<TDto, Expression<Func<TDto, bool>>>[] Conditions { get; }

    public RemoteUpserted(RemoteUpsert<TStore, TDto, TModel> command) : base(command)
    {
        Conditions = command.Conditions;
        Predicate = command.Predicate;
    }
}
