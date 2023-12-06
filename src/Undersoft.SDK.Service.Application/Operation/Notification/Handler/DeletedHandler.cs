using FluentValidation.Results;
using MediatR;

namespace Undersoft.SDK.Service.Application.Operation.Notification.Handler;
using Logging;
using Undersoft.SDK.Service.Data.Event;

using Undersoft.SDK.Service.Data.Repository;
using SDK.Service.Data.Store;

public class DeletedHandler<TStore, TEntity, TDto>
    : INotificationHandler<Deleted<TStore, TEntity, TDto>>
    where TDto : class, IDataObject
    where TEntity : class, IDataObject
    where TStore : IDatabaseStore
{
    protected readonly IStoreRepository<TEntity> _repository;
    protected readonly IStoreRepository<Event> _eventStore;

    public DeletedHandler() { }

    public DeletedHandler(
        IStoreRepository<IReportStore, TEntity> repository,
        IStoreRepository<IEventStore, Event> eventStore
    )
    {
        _repository = repository;
        _eventStore = eventStore;
    }

    public virtual Task Handle(
        Deleted<TStore, TEntity, TDto> request,
        CancellationToken cancellationToken
    )
    {
        return Task.Run(
            async () =>
            {
                try
                {
                    if (_eventStore.Add(request) == null)
                        throw new Exception(
                            $"{GetType().Name} "
                                + $"for entity {typeof(TEntity).Name} unable add event"
                        );

                    if (request.Command.PublishMode == EventPublishMode.PropagateCommand)
                    {
                        TEntity result = null;
                        if (request.Command.Keys != null)
                            result = await _repository.Delete(request.Command.Keys);
                        else if (request.EventData == null && request.Predicate != null)
                            result = await _repository.Delete(request.Predicate);
                        else
                            result = await _repository.DeleteBy(
                                request.EventData,
                                request.Predicate
                            );

                        if (result == null)
                            throw new Exception(
                                $"{GetType().Name} "
                                    + $"for entity {typeof(TEntity).Name} unable delete report"
                            );

                        request.PublishStatus = EventPublishStatus.Complete;
                    }
                }
                catch (Exception ex)
                {
                    request.Command.Result.Errors.Add(
                        new ValidationFailure(string.Empty, ex.Message)
                    );
                    this.Failure<Domainlog>(ex.Message, request.Command.ErrorMessages, ex);
                    request.PublishStatus = EventPublishStatus.Error;
                }
            },
            cancellationToken
        );
    }
}
