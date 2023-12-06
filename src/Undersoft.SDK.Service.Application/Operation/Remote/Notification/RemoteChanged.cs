using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Undersoft.SDK.Service.Application.Operation.Remote.Notification;

using Command;


using SDK.Service.Data.Store;

public class RemoteChanged<TStore, TDto, TModel> : RemoteNotification<RemoteCommand<TModel>>
    where TDto : class, IDataObject
    where TModel : class, IDataObject
    where TStore : IDataServiceStore
{
    public RemoteChanged(RemoteCommand<TModel> command) : base(command) { }

    public RemoteChanged(RemoteChange<TStore, TDto, TModel> command) : base(command)
    {
        Predicate = command.Predicate;
    }

    [JsonIgnore]
    public Func<TModel, Expression<Func<TDto, bool>>> Predicate { get; }
}
