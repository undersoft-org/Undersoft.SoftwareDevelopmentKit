using FluentValidation.Results;
using MediatR;

namespace Undersoft.SDK.Service.Server.Operation.Command.Handler;

using Notification;
using Undersoft.SDK.Service.Data.Store;
using Undersoft.SDK.Service.Data.Store.Repository;

public class ChangeHandler<TStore, TEntity, TDto>
    : IRequestHandler<Change<TStore, TEntity, TDto>, Command<TDto>>
    where TDto : class, IOrigin, IInnerProxy
    where TEntity : class, IOrigin, IInnerProxy
    where TStore : IDataServerStore
{
    protected readonly IStoreRepository<TEntity> _repository;
    protected readonly IServicer _servicer;

    public ChangeHandler(IServicer servicer, IStoreRepository<TStore, TEntity> repository)
    {
        _servicer = servicer;
        _repository = repository;
    }

    public virtual async Task<Command<TDto>> Handle(
        Change<TStore, TEntity, TDto> request,
        CancellationToken cancellationToken
    )
    {
        if (!request.Result.IsValid)
            return request;
        try
        {
            if (request.Keys != null)
                request.Entity = await _repository
                    .PatchBy(request.Contract, request.Keys)
                    .ConfigureAwait(false);
            else
                request.Entity = await _repository
                    .PatchBy(request.Contract, request.Predicate)
                    .ConfigureAwait(false);

            if (request.Entity == null)
                throw new Exception(
                    $"{GetType().Name} for entity " + $"{typeof(TEntity).Name} unable patch source"
                );

            _ = _servicer
                .Publish(new Changed<TStore, TEntity, TDto>(request), cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            request.Result.Errors.Add(new ValidationFailure(string.Empty, ex.Message));
            this.Failure<Applog>(ex.Message, request.ErrorMessages, ex);
        }

        return request;
    }
}
