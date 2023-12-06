using MediatR;

namespace Undersoft.SDK.Service.Application.Operation.Notification.Handler;
using Logging;
using Undersoft.SDK.Service.Data.Event;

using Undersoft.SDK.Service.Data.Repository;
using SDK.Service.Data.Store;
using Series;

public class UpdatedSetHandler<TStore, TEntity, TDto>
    : INotificationHandler<UpdatedSet<TStore, TEntity, TDto>>
    where TDto : class, IDataObject
    where TEntity : class, IDataObject
    where TStore : IDatabaseStore
{
    protected readonly IStoreRepository<Event> _eventStore;
    protected readonly IStoreRepository<TEntity> _repository;

    public UpdatedSetHandler() { }

    public UpdatedSetHandler(
        IStoreRepository<IReportStore, TEntity> repository,
        IStoreRepository<IEventStore, Event> eventStore
    )
    {
        _repository = repository;
        _eventStore = eventStore;
    }

    public virtual Task Handle(
        UpdatedSet<TStore, TEntity, TDto> request,
        CancellationToken cancellationToken
    )
    {
        return Task.Run(
            () =>
            {
                try
                {
                    request.ForOnly(
                        d => !d.Command.IsValid,
                        d =>
                        {
                            request.Remove(d);
                        }
                    );

                    _eventStore.AddAsync(request).ConfigureAwait(true);

                    if (request.PublishMode == EventPublishMode.PropagateCommand)
                    {
                        ISeries<TEntity> entities;
                        if (request.Predicate == null)
                            entities = _repository
                                .SetBy(request.Select(d => d.Command.Contract))
                                .ToCatalog();
                        else if (request.Conditions == null)
                            entities = _repository
                                .SetBy(request.Select(d => d.Command.Contract), request.Predicate)
                                .ToCatalog();
                        else
                            entities = _repository
                                .SetBy(
                                    request.Select(d => d.Command.Contract),
                                    request.Predicate,
                                    request.Conditions
                                )
                                .ToCatalog();

                        request.ForEach(
                            (r) =>
                            {
                                _ = entities.ContainsKey(r.AggregateId)
                                    ? (r.PublishStatus = EventPublishStatus.Complete)
                                    : (r.PublishStatus = EventPublishStatus.Uncomplete);
                            }
                        );
                    }
                }
                catch (Exception ex)
                {
                    this.Failure<Domainlog>(
                        ex.Message,
                        request.Select(r => r.Command.ErrorMessages).ToArray(),
                        ex
                    );
                    request.ForEach((r) => r.PublishStatus = EventPublishStatus.Error);
                }
            },
            cancellationToken
        );
    }
}
