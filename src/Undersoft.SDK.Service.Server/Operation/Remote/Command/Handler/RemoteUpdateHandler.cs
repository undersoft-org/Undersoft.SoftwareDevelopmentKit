using FluentValidation.Results;
using MediatR;

namespace Undersoft.SDK.Service.Server.Operation.Remote.Command.Handler;

using Notification;
using Undersoft.SDK.Service.Data.Remote.Repository;
using Undersoft.SDK.Service.Data.Store;

public class RemoteUpdateHandler<TStore, TDto, TModel>
    : IRequestHandler<RemoteUpdate<TStore, TDto, TModel>, RemoteCommand<TModel>>
    where TDto : class, IOrigin, IInnerProxy
    where TModel : class, IOrigin, IInnerProxy
    where TStore : IDataServiceStore
{
    protected readonly IRemoteRepository<TDto> _repository;
    protected readonly IServicer _servicer;

    public RemoteUpdateHandler(IServicer servicer, IRemoteRepository<TStore, TDto> repository)
    {
        _repository = repository;
        _servicer = servicer;
    }

    public async Task<RemoteCommand<TModel>> Handle(
        RemoteUpdate<TStore, TDto, TModel> request,
        CancellationToken cancellationToken
    )
    {
        if (!request.Result.IsValid)
            return request;

        try
        {
            if (request.Predicate == null)
                request.Contract = await _repository.SetBy(request.Model, request.Keys);
            else if (request.Conditions == null)
                request.Contract = await _repository.SetBy(request.Model, request.Predicate);
            else
                request.Contract = await _repository.SetBy(
                    request.Model,
                    request.Predicate,
                    request.Conditions
                );

            if (request.Contract == null)
                throw new Exception(
                    $"{GetType().Name} for entity " + $"{typeof(TDto).Name} unable update source"
                );

            _ = _servicer
                .Publish(new RemoteUpdated<TStore, TDto, TModel>(request))
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
