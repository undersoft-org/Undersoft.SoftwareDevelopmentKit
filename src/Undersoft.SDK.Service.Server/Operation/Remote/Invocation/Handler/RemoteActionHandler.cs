using FluentValidation.Results;
using MediatR;

namespace Undersoft.SDK.Service.Server.Operation.Remote.Invocation.Handler;

using Microsoft.AspNetCore.Http;
using Notification;
using Undersoft.SDK;
using Undersoft.SDK.Service.Server.Operation.Command;
using Undersoft.SDK.Service.Server.Operation.Invocation;
using Undersoft.SDK.Service.Server.Operation.Remote.Invocation;
using Undersoft.SDK.Service.Data.Store;
using Undersoft.SDK.Service.Data.Remote.Repository;

public class RemoteActionHandler<TStore, TService, TModel>
    : IRequestHandler<RemoteAction<TStore, TService, TModel>, Invocation<TModel>>
    where TService : class
    where TModel : class, IOrigin, IInnerProxy
    where TStore : IDataServiceStore
{
    protected readonly IRemoteRepository<TModel> _repository;
    protected readonly IServicer _servicer;

    public RemoteActionHandler(IServicer servicer, IRemoteRepository<TStore, TModel> repository)
    {
        _repository = repository;
        _servicer = servicer;
    }

    public async Task<Invocation<TModel>> Handle(
        RemoteAction<TStore, TService, TModel> request,
        CancellationToken cancellationToken
    )
    {
        if (!request.Result.IsValid)
            return request;
        try
        {
            request.Response = 
                     await _repository.Action(request.Method, request.Arguments)
                    
            ;

            if (request.Response == null)
                throw new Exception(
                    $"{GetType().Name} "
                        + $"for entity {typeof(TModel).Name} "
                        + $"unable create source"
                );

            await _servicer
                .Publish(new RemoteActionInvoked<TStore, TService, TModel>(request))
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
