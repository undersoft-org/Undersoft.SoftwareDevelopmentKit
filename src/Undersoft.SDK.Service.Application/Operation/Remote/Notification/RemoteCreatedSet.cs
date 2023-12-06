using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Undersoft.SDK.Service.Application.Operation.Remote.Notification;

using Command;


using SDK.Service.Data.Store;

public class RemoteCreatedSet<TStore, TDto, TModel> : RemoteNotificationSet<RemoteCommand<TModel>>
    where TDto : class, IDataObject
    where TModel : class, IDataObject
    where TStore : IDataServiceStore
{
    [JsonIgnore]
    public Func<TDto, Expression<Func<TDto, bool>>> Predicate { get; }

    public RemoteCreatedSet(RemoteCreateSet<TStore, TDto, TModel> commands)
        : base(
            commands.PublishMode,
            commands
                .ForOnly(
                    c => c.Contract != null,
                    c => new RemoteCreated<TStore, TDto, TModel>((RemoteCreate<TStore, TDto, TModel>)c)
                )
                .ToArray()
        )
    {
        Predicate = commands.Predicate;
    }
}
