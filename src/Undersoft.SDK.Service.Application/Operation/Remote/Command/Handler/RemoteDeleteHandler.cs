using FluentValidation.Results;
using MediatR;

namespace Undersoft.SDK.Service.Application.Operation.Remote.Command.Handler;

using Notification;



public class RemoteDeleteHandler<TStore, TDto, TModel>
    : IRequestHandler<RemoteDelete<TStore, TDto, TModel>, RemoteCommand<TModel>>
    where TDto : class, IDataObject
    where TModel : class, IDataObject
    where TStore : IDataServiceStore
{
    protected readonly IRemoteRepository<TDto> _repository;
    protected readonly IServicer _umaker;

    public RemoteDeleteHandler(IServicer umaker, IRemoteRepository<TStore, TDto> repository)
    {
        _repository = repository;
        _umaker = umaker;
    }

    public async Task<RemoteCommand<TModel>> Handle(
        RemoteDelete<TStore, TDto, TModel> request,
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
                    if (request.Keys != null)
                        request.Contract = await _repository.Delete(request.Keys);
                    else if (request.Model == null && request.Predicate != null)
                        request.Contract = await _repository.Delete(request.Predicate);
                    else
                        request.Contract = await _repository.DeleteBy(
                            request.Model,
                            request.Predicate
                        );

                    if (request.Contract == null)
                        throw new Exception(
                            $"{GetType().Name} for entity"
                                + $" {typeof(TDto).Name} unable delete source"
                        );

                    _ = _umaker
                        .Publish(new RemoteDeleted<TStore, TDto, TModel>(request))
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
