using FluentValidation.Results;
using MediatR;

namespace Undersoft.SDK.Service.Server.Operation.Remote.Command.Handler;

using Notification;
using Undersoft.SDK.Service.Data.Remote.Repository;
using Undersoft.SDK.Service.Data.Store;

public class RemoteCreateHandler<TStore, TDto, TModel>
    : IRequestHandler<RemoteCreate<TStore, TDto, TModel>, RemoteCommand<TModel>>
    where TDto : class, IOrigin, IInnerProxy
    where TModel : class, IOrigin, IInnerProxy
    where TStore : IDataServiceStore
{
    protected readonly IRemoteRepository<TDto> _repository;
    protected readonly IServicer _servicer;

    public RemoteCreateHandler(IServicer servicer, IRemoteRepository<TStore, TDto> repository)
    {
        _repository = repository;
        _servicer = servicer;
    }

    public async Task<RemoteCommand<TModel>> Handle(
        RemoteCreate<TStore, TDto, TModel> request,
        CancellationToken cancellationToken
    )
    {
        if (!request.Result.IsValid)
            return request;
        try
        {
            request.Contract = _repository
                .AddBy(request.Model, request.Predicate);                

            if (request.Contract == null)
                throw new Exception(
                    $"{GetType().Name} "
                        + $"for entity {typeof(TDto).Name} "
                        + $"unable create source"
                );

            _ = _servicer
                .Publish(new RemoteCreated<TStore, TDto, TModel>(request))
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
