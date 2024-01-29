using FluentValidation.Results;
using MediatR;

namespace Undersoft.SDK.Service.Server.Operation.Command.Handler;

using Logging;
using Undersoft.SDK.Service.Data.Remote.Repository;
using Undersoft.SDK.Service.Data.Store;
using Undersoft.SDK.Service.Server.Operation.Remote.Command;
using Undersoft.SDK.Service.Server.Operation.Remote.Command.Notification;

public class RemoteUpsertHandler<TStore, TDto, TModel>
    : IRequestHandler<RemoteUpsert<TStore, TDto, TModel>, RemoteCommand<TModel>>
    where TDto : class, IOrigin, IInnerProxy
    where TModel : class, IOrigin, IInnerProxy
    where TStore : IDataServiceStore
{
    protected readonly IRemoteRepository<TDto> _repository;
    protected readonly IServicer _umaker;

    public RemoteUpsertHandler(IServicer umaker, IRemoteRepository<TStore, TDto> repository)
    {
        _repository = repository;
        _umaker = umaker;
    }

    public async Task<RemoteCommand<TModel>> Handle(
        RemoteUpsert<TStore, TDto, TModel> request,
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
                        request.Contract = await _repository.PutBy(
                            request.Model,
                            request.Predicate,
                            request.Conditions
                        );
                    else
                        request.Contract = await _repository.PutBy(
                            request.Model,
                            request.Predicate
                        );

                    if (request.Contract == null)
                        throw new Exception(
                            $"{GetType().Name} "
                                + $"for entity {typeof(TDto).Name} unable renew source"
                        );

                    _ = _umaker
                        .Publish(new RemoteUpserted<TStore, TDto, TModel>(request))
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
