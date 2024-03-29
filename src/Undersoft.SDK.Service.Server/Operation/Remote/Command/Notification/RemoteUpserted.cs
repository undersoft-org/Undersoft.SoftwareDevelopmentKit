﻿using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Undersoft.SDK.Service.Server.Operation.Remote.Command.Notification;

using Command;
using Undersoft.SDK.Service.Data.Store;
using Undersoft.SDK.Service.Server.Operation.Command;

public class RemoteUpserted<TStore, TDto, TModel> : RemoteNotification<RemoteCommand<TModel>>
    where TDto : class, IOrigin, IInnerProxy
    where TModel : class, IOrigin, IInnerProxy
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
