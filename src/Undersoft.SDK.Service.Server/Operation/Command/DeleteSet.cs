﻿using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Undersoft.SDK.Service.Server.Operation.Command;

using Undersoft.SDK.Service.Data.Object;

using Undersoft.SDK.Service.Data.Event;
using Undersoft.SDK.Service.Data.Store;

public class DeleteSet<TStore, TEntity, TDto> : CommandSet<TDto>
    where TEntity : class, IOrigin, IInnerProxy
    where TDto : class, IOrigin, IInnerProxy
    where TStore : IDataServerStore
{
    [JsonIgnore]
    public Func<TEntity, Expression<Func<TEntity, bool>>> Predicate { get; }

    public DeleteSet(EventPublishMode publishPattern, object key)
        : base(
            CommandMode.Create,
            publishPattern,
            new[] { new Delete<TStore, TEntity, TDto>(publishPattern, key) }
        ) { }

    public DeleteSet(EventPublishMode publishPattern, TDto input, object key)
        : base(
            CommandMode.Create,
            publishPattern,
            new[] { new Delete<TStore, TEntity, TDto>(publishPattern, input, key) }
        ) { }

    public DeleteSet(EventPublishMode publishPattern, TDto[] inputs)
        : base(
            CommandMode.Delete,
            publishPattern,
            inputs
                .Select(input => new Delete<TStore, TEntity, TDto>(publishPattern, input))
                .ToArray()
        ) { }

    public DeleteSet(
        EventPublishMode publishPattern,
        TDto[] inputs,
        Func<TEntity, Expression<Func<TEntity, bool>>> predicate
    )
        : base(
            CommandMode.Delete,
            publishPattern,
            inputs
                .Select(
                    input => new Delete<TStore, TEntity, TDto>(publishPattern, input, predicate)
                )
                .ToArray()
        )
    {
        Predicate = predicate;
    }
}
