using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Undersoft.SDK.Service.Application.Operation.Remote.Notification;
using Command;
using SDK.Service.Data.Store;



public class RemoteDeleted<TStore, TDto, TModel> : RemoteNotification<RemoteCommand<TModel>>
    where TDto : class, IDataObject
    where TModel : class, IDataObject
    where TStore : IDataServiceStore
{
    [JsonIgnore]
    public Func<TDto, Expression<Func<TDto, bool>>> Predicate { get; }

    public RemoteDeleted(RemoteDelete<TStore, TDto, TModel> command) : base(command)
    {
        Predicate = command.Predicate;
    }
}
