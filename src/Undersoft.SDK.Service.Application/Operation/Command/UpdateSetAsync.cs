using MediatR;
using System.Linq.Expressions;

namespace Undersoft.SDK.Service.Application.Operation.Command;

using Undersoft.SDK.Service.Data.Event;

using Undersoft.SDK.Service.Data.Object;
using SDK.Service.Data.Store;

public class UpdateSetAsync<TStore, TEntity, TDto>
    : UpdateSet<TStore, TEntity, TDto>,
        IStreamRequest<Command<TDto>>
    where TEntity : class, IDataObject
    where TDto : class, IDataObject
    where TStore : IDatabaseStore
{
    public UpdateSetAsync(EventPublishMode publishPattern, TDto input, object key)
        : base(publishPattern, input, key) { }

    public UpdateSetAsync(EventPublishMode publishPattern, TDto[] inputs)
        : base(publishPattern, inputs) { }

    public UpdateSetAsync(
        EventPublishMode publishPattern,
        TDto[] inputs,
        Func<TDto, Expression<Func<TEntity, bool>>> predicate
    ) : base(publishPattern, inputs, predicate) { }

    public UpdateSetAsync(
        EventPublishMode publishPattern,
        TDto[] inputs,
        Func<TDto, Expression<Func<TEntity, bool>>> predicate,
        params Func<TDto, Expression<Func<TEntity, bool>>>[] conditions
    ) : base(publishPattern, inputs, predicate, conditions) { }
}
