using FluentValidation.Results;
using MediatR;

namespace Undersoft.SDK.Service.Server.Operation.Command.Handler;
using Logging;
using Notification;
using Undersoft.SDK.Service.Data.Store;
using Undersoft.SDK.Service.Data.Store.Repository;

public class UpsertHandler<TStore, TEntity, TDto>
    : IRequestHandler<Upsert<TStore, TEntity, TDto>, Command<TDto>>
    where TDto : class, IOrigin, IInnerProxy
    where TEntity : class, IOrigin, IInnerProxy
    where TStore : IDataServerStore
{
    protected readonly IStoreRepository<TEntity> _repository;
    protected readonly IServicer _umaker;

    public UpsertHandler(IServicer umaker, IStoreRepository<TStore, TEntity> repository)
    {
        _repository = repository;
        _umaker = umaker;
    }

    public async Task<Command<TDto>> Handle(
        Upsert<TStore, TEntity, TDto> request,
        CancellationToken cancellationToken
    )
    {
        return await Task.Run(
            async () =>
            {
                if (!request.Result.IsValid)
                    return request;

                try
                {
                    if (request.Conditions != null)
                        request.Entity = await _repository.PutBy(
                            request.Contract,
                            request.Predicate,
                            request.Conditions
                        );
                    else
                        request.Entity = await _repository.PutBy(request.Contract, request.Predicate);

                    if (request.Entity == null)
                        throw new Exception(
                            $"{GetType().Name} "
                                + $"for entity {typeof(TEntity).Name} unable renew source"
                        );

                    _ = _umaker
                        .Publish(new Upserted<TStore, TEntity, TDto>(request))
                        .ConfigureAwait(false);
                    ;
                }
                catch (Exception ex)
                {
                    request.Result.Errors.Add(new ValidationFailure(string.Empty, ex.Message));
                    this.Failure<Applog>(ex.Message, request.ErrorMessages, ex);
                }

                return request;
            },
            cancellationToken
        );
    }
}
