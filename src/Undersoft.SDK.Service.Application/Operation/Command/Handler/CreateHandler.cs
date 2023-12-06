using FluentValidation.Results;
using MediatR;

namespace Undersoft.SDK.Service.Application.Operation.Command.Handler;
using Logging;
using Notification;

using Undersoft.SDK.Service.Data.Repository;
using SDK.Service.Data.Store;

public class CreateHandler<TStore, TEntity, TDto>
    : IRequestHandler<Create<TStore, TEntity, TDto>, Command<TDto>>
    where TDto : class, IDataObject
    where TEntity : class, IDataObject
    where TStore : IDatabaseStore
{
    protected readonly IStoreRepository<TEntity> _repository;
    protected readonly IServicer _servicer;

    public CreateHandler(IServicer servicer, IStoreRepository<TStore, TEntity> repository)
    {
        _repository = repository;
        _servicer = servicer;
    }

    public async Task<Command<TDto>> Handle(
        Create<TStore, TEntity, TDto> request,
        CancellationToken cancellationToken
    )
    {
        if (!request.Result.IsValid)
            return request;
        try
        {
            request.Entity = await _repository
                .AddBy(request.Contract, request.Predicate)
                .ConfigureAwait(false);

            if (request.Entity == null)
                throw new Exception(
                    $"{GetType().Name} "
                        + $"for entity {typeof(TEntity).Name} "
                        + $"unable create source"
                );

            _ = _servicer
                .Publish(new Created<TStore, TEntity, TDto>(request))
                .ConfigureAwait(false);
            ;
        }
        catch (Exception ex)
        {
            request.Result.Errors.Add(new ValidationFailure(string.Empty, ex.Message));
            this.Failure<Applog>(ex.Message, request.ErrorMessages, ex);
        }
        return request;
    }
}
