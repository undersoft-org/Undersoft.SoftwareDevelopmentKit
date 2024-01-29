using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Undersoft.SDK.Service.Server.Operation.Remote.Command.Notification;
using Command;
using Undersoft.SDK.Service.Data.Store;

public class RemoteDeleted<TStore, TDto, TModel> : RemoteNotification<RemoteCommand<TModel>>
    where TDto : class, IOrigin, IInnerProxy
    where TModel : class, IOrigin, IInnerProxy
    where TStore : IDataServiceStore
{
    [JsonIgnore]
    public Func<TDto, Expression<Func<TDto, bool>>> Predicate { get; }

    public RemoteDeleted(RemoteDelete<TStore, TDto, TModel> command) : base(command)
    {
        Predicate = command.Predicate;
    }
}
