using MediatR;
using Undersoft.SDK.Service.Data.Object;
using System.Linq.Expressions;

namespace Undersoft.SDK.Service.Application.Operation.Command;

public class CreateSetAsync<TStore, TEntity, TDto>
    : CreateSet<TStore, TEntity, TDto>,
        IStreamRequest<Command<TDto>>
    where TEntity : class, IDataObject
    where TDto : class, IDataObject
    where TStore : IDatabaseStore
{
    public CreateSetAsync(EventPublishMode publishPattern, TDto input, object key)
        : base(publishPattern, input, key) { }

    public CreateSetAsync(EventPublishMode publishPattern, TDto[] inputs)
        : base(publishPattern, inputs) { }

    public CreateSetAsync(
        EventPublishMode publishPattern,
        TDto[] inputs,
        Func<TEntity, Expression<Func<TEntity, bool>>> predicate
    ) : base(publishPattern, inputs, predicate) { }
}
