using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Undersoft.SDK.Service.Application.Operation.Notification;

using Command;

using SDK.Service.Data.Store;

public class UpsertedSet<TStore, TEntity, TDto> : NotificationSet<Command<TDto>>
    where TDto : class, IDataObject
    where TEntity : class, IDataObject
    where TStore : IDatabaseStore
{
    [JsonIgnore]
    public Func<TEntity, Expression<Func<TEntity, bool>>> Predicate { get; }

    [JsonIgnore]
    public Func<TEntity, Expression<Func<TEntity, bool>>>[] Conditions { get; }

    public UpsertedSet(UpsertSet<TStore, TEntity, TDto> commands)
        : base(
            commands.PublishMode,
            commands
                .ForOnly(
                    c => c.Entity != null,
                    c => new Upserted<TStore, TEntity, TDto>((Upsert<TStore, TEntity, TDto>)c)
                )
                .ToArray()
        )
    {
        Conditions = commands.Conditions;
        Predicate = commands.Predicate;
    }
}
