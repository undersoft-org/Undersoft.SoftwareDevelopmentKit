using FluentValidation.Results;
using MediatR;

namespace Undersoft.SDK.Service.Application.Operation.Remote.Command.Handler;

using Notification;
using Undersoft.SDK;
using Undersoft.SDK.Service.Application.Operation.Command;

public class RemoteExecuteHandler<TStore, TDto, TModel>
    : IRequestHandler<RemoteExecute<TStore, TDto, TModel>, ActionCommand<TModel>>
    where TDto : class, IOrigin
    where TModel : class, IOrigin
    where TStore : IDataServiceStore
{
    protected readonly IRemoteRepository<TDto> _repository;
    protected readonly IServicer _servicer;

    public RemoteExecuteHandler(IServicer servicer, IRemoteRepository<TStore, TDto> repository)
    {
        _repository = repository;
        _servicer = servicer;
    }

    public async Task<ActionCommand<TModel>> Handle(
        RemoteExecute<TStore, TDto, TModel> request,
        CancellationToken cancellationToken
    )
    {
        if (!request.Result.IsValid)
            return request;
        try
        {
            request.Response = (await _repository
                .ExecuteAsync<TModel>(request.Data,  request.Kind));
                

            if (request.Response == null)
                throw new Exception(
                    $"{GetType().Name} "
                        + $"for entity {typeof(TDto).Name} "
                        + $"unable create source"
                );

            await _servicer.Publish(new RemoteExecuted<TStore, TDto, TModel>(request)).ConfigureAwait(false);            
        }
        catch (Exception ex)
        {
            request.Result.Errors.Add(new ValidationFailure(string.Empty, ex.Message));
            this.Failure<Applog>(ex.Message, request.ErrorMessages, ex);
        }
        return request;
    }
}
