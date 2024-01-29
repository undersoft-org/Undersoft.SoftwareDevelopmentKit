using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Undersoft.SDK.Service.Server.Operation.Command.Notification;

using Command;
using Undersoft.SDK.Service.Data.Store;

public class Updated<TStore, TEntity, TDto> : Notification<Command<TDto>>
    where TDto : class, IOrigin, IInnerProxy
    where TEntity : class, IOrigin, IInnerProxy
    where TStore : IDataServerStore
{
    public Updated(Update<TStore, TEntity, TDto> command) : base(command)
    {
        Predicate = command.Predicate;
        Conditions = command.Conditions;
    }

    [JsonIgnore]
    public Func<TEntity, Expression<Func<TEntity, bool>>>[] Conditions { get; }

    [JsonIgnore]
    public Func<TEntity, Expression<Func<TEntity, bool>>> Predicate { get; }
}
