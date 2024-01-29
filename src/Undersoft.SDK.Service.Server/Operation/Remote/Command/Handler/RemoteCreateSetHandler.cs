using MediatR;

namespace Undersoft.SDK.Service.Server.Operation.Remote.Command.Handler;

using Notification;
using Undersoft.SDK.Service.Data.Remote.Repository;
using Undersoft.SDK.Service.Data.Store;

public class RemoteCreateSetHandler<TStore, TDto, TModel>
    : IRequestHandler<RemoteCreateSet<TStore, TDto, TModel>, RemoteCommandSet<TModel>>
    where TDto : class, IOrigin, IInnerProxy
    where TModel : class, IOrigin, IInnerProxy
    where TStore : IDataServiceStore
{
    protected readonly IRemoteRepository<TDto> _repository;
    protected readonly IServicer _uservice;

    public RemoteCreateSetHandler(IServicer uservice, IRemoteRepository<TStore, TDto> repository)
    {
        _uservice = uservice;
        _repository = repository;
    }

    public virtual async Task<RemoteCommandSet<TModel>> Handle(
        RemoteCreateSet<TStore, TDto, TModel> request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            IEnumerable<TDto> entities;
            if (request.Predicate == null)
                entities = _repository.AddBy(request.ForOnly(d => d.IsValid, d => d.Model));
            else
                entities = _repository.AddBy(
                    request.ForOnly(d => d.IsValid, d => d.Model),
                    request.Predicate
                );

            entities
                .ForEach(
                    (e, x) =>
                    {
                        request[x].Contract = e;
                    }
                );

              await _uservice
                .Publish(new RemoteCreatedSet<TStore, TDto, TModel>(request));                
        }
        catch (Exception ex)
        {
            this.Failure<Applog>(ex.Message, request.Select(r => r.ErrorMessages).ToArray(), ex);
        }
        return request;
    }
}
