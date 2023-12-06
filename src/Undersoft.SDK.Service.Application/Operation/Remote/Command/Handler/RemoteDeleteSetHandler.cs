using MediatR;

namespace Undersoft.SDK.Service.Application.Operation.Remote.Command.Handler;

using Notification;



public class RemoteDeleteSetHandler<TStore, TDto, TModel>
    : IRequestHandler<RemoteDeleteSet<TStore, TDto, TModel>, RemoteCommandSet<TModel>>
    where TDto : class, IDataObject
    where TModel : class, IDataObject
    where TStore : IDataServiceStore
{
    protected readonly IRemoteRepository<TDto> _repository;
    protected readonly IServicer _uservice;

    public RemoteDeleteSetHandler(IServicer uservice, IRemoteRepository<TStore, TDto> repository)
    {
        _uservice = uservice;
        _repository = repository;
    }

    public virtual async Task<RemoteCommandSet<TModel>> Handle(
        RemoteDeleteSet<TStore, TDto, TModel> request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            IEnumerable<TDto> entities;
            if (request.Predicate == null)
                entities = _repository.DeleteBy(request.ForOnly(d => d.IsValid, d => d.Model));
            else
                entities = _repository.DeleteBy(
                    request.ForOnly(d => d.IsValid, d => d.Model),
                    request.Predicate
                );

            await entities
                .ForEachAsync(
                    (e) =>
                    {
                        request[e.Id].Contract = e;
                    }
                )
                .ConfigureAwait(false);

            _ = _uservice
                .Publish(new RemoteDeletedSet<TStore, TDto, TModel>(request))
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this.Failure<Domainlog>(ex.Message, request.Select(r => r.ErrorMessages).ToArray(), ex);
        }
        return request;
    }
}
