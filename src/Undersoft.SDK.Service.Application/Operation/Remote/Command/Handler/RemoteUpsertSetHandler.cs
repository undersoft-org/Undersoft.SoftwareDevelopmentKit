using MediatR;

namespace Undersoft.SDK.Service.Application.Operation.Command.Handler;
using Logging;
using Notification;
using Undersoft.SDK.Service.Application.Operation.Remote.Command;
using Undersoft.SDK.Service.Data.Repository;
using SDK.Service.Data.Store;



public class RemoteUpsertSetHandler<TStore, TDto, TModel>
    : IRequestHandler<RemoteUpsertSet<TStore, TDto, TModel>, RemoteCommandSet<TModel>>
    where TDto : class, IDataObject
    where TModel : class, IDataObject
    where TStore : IDataServiceStore
{
    protected readonly IRemoteRepository<TDto> _repository;
    protected readonly IServicer _uservice;

    public RemoteUpsertSetHandler(IServicer uservice, IRemoteRepository<TStore, TDto> repository)
    {
        _uservice = uservice;
        _repository = repository;
    }

    public virtual async Task<RemoteCommandSet<TModel>> Handle(
        RemoteUpsertSet<TStore, TDto, TModel> request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            IEnumerable<TDto> entities;
            if (request.Conditions == null)
                entities = _repository.PutBy(
                    request.ForOnly(d => d.IsValid, d => d.Model),
                    request.Predicate
                );
            else
                entities = _repository.PutBy(
                    request.ForOnly(d => d.IsValid, d => d.Model),
                    request.Predicate,
                    request.Conditions
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
                .Publish(new RemoteUpsertedSet<TStore, TDto, TModel>(request))
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this.Failure<Domainlog>(ex.Message, request.Select(r => r.ErrorMessages).ToArray(), ex);
        }
        return request;
    }
}
