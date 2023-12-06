using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Undersoft.SDK.Service.Application.Operation.Remote.Notification;

using Command;
using SDK.Service.Data.Store;



public class RemoteUpdated<TStore, TDto, TModel> : RemoteNotification<RemoteCommand<TModel>>
    where TDto : class, IDataObject
    where TModel : class, IDataObject
    where TStore : IDataServiceStore
{
    public RemoteUpdated(RemoteUpdate<TStore, TDto, TModel> command) : base(command)
    {
        Predicate = command.Predicate;
        Conditions = command.Conditions;
    }

    [JsonIgnore]
    public Func<TDto, Expression<Func<TDto, bool>>>[] Conditions { get; }

    [JsonIgnore]
    public Func<TDto, Expression<Func<TDto, bool>>> Predicate { get; }
}
