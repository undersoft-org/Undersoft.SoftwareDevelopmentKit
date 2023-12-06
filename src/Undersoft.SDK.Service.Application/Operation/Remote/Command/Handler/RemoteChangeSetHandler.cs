using MediatR;

namespace Undersoft.SDK.Service.Application.Operation.Remote.Command.Handler;

using Notification;



public class RemoteChangeSetHandler<TStore, TDto, TModel>
    : IRequestHandler<RemoteChangeSet<TStore, TDto, TModel>, RemoteCommandSet<TModel>>
    where TDto : class, IDataObject
    where TModel : class, IDataObject
    where TStore : IDataServiceStore
{
    protected readonly IRemoteRepository<TDto> _repository;
    protected readonly IServicer _servicer;

    public RemoteChangeSetHandler(IServicer servicer, IRemoteRepository<TStore, TDto> repository)
    {
        _servicer = servicer;
        _repository = repository;
    }

    public virtual async Task<RemoteCommandSet<TModel>> Handle(
        RemoteChangeSet<TStore, TDto, TModel> request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            IEnumerable<TDto> dtos;
            if (request.Predicate == null)
                dtos = _repository.Patch(request.ForOnly(d => d.IsValid, d => d.Model));
            else
                dtos = _repository.Patch(
                    request.ForOnly(d => d.IsValid, d => d.Model),
                    request.Predicate
                );

            await dtos
                .ForEachAsync(
                    (e) =>
                    {
                        request[e.Id].Contract = e;
                    }
                )
                .ConfigureAwait(false);

            _ = _servicer
                .Publish(new RemoteChangedSet<TStore, TDto, TModel>(request))
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this.Failure<Domainlog>(ex.Message, request.Select(r => r.Output).ToArray(), ex);
        }
        return request;
    }
}
