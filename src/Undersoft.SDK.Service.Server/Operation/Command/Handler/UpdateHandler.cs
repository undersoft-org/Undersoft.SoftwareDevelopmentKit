using FluentValidation.Results;
using MediatR;

namespace Undersoft.SDK.Service.Server.Operation.Command.Handler;
using Logging;
using Notification;
using Undersoft.SDK.Service.Data.Store;
using Undersoft.SDK.Service.Data.Store.Repository;

public class UpdateHandler<TStore, TEntity, TDto>
    : IRequestHandler<Update<TStore, TEntity, TDto>, Command<TDto>>
    where TDto : class, IOrigin, IInnerProxy
    where TEntity : class, IOrigin, IInnerProxy
    where TStore : IDataServerStore
{
    protected readonly IStoreRepository<TEntity> _repository;
    protected readonly IServicer _servicer;

    public UpdateHandler(IServicer servicer, IStoreRepository<TStore, TEntity> repository)
    {
        _repository = repository;
        _servicer = servicer;
    }

    public async Task<Command<TDto>> Handle(
        Update<TStore, TEntity, TDto> request,
        CancellationToken cancellationToken
    )
    {
        if (!request.Result.IsValid)
            return request;

        try
        {
            if (request.Predicate == null)
                request.Entity = await _repository.SetBy(request.Contract, request.Keys);
            else if (request.Conditions == null)
                request.Entity = await _repository.SetBy(request.Contract, request.Predicate);
            else
                request.Entity = await _repository.SetBy(
                    request.Contract,
                    request.Predicate,
                    request.Conditions
                );

            if (request.Entity == null)
                throw new Exception(
                    $"{GetType().Name} for entity " + $"{typeof(TEntity).Name} unable update source"
                );

            _ = _servicer
                .Publish(new Updated<TStore, TEntity, TDto>(request))
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
