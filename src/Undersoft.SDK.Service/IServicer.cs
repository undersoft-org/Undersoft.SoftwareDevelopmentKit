using MediatR;

namespace Undersoft.SDK.Service;

using Undersoft.SDK.Service.Data.Repository;

public interface IServicer : IServiceManager, IRepositoryManager, IDisposable
{
    Task Publish(object notification, CancellationToken cancellationToken = default);
    Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification;

    Task<R> Run<T, R>(Func<T, Task<R>> function) where T : class;
    Task<R> Run<T, R>(string methodname, params object[] parameters) where T : class;

    Task Run<T>(Func<T, Task> function) where T : class;
    Task Run<T>(string methodname, params object[] parameters) where T : class;
    Task Save(bool asTransaction = false);

    Task<object> Send(object request, CancellationToken cancellationToken = default);
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

    IAsyncEnumerable<TResponse> CreateStream<TResponse>(
        IStreamRequest<TResponse> request,
        CancellationToken cancellationToken = default
    );

    IAsyncEnumerable<object> CreateStream(
        object request,
        CancellationToken cancellationToken = default
    );

    Task<R> Serve<T, R>(Func<T, Task<R>> function) where T : class;
    Task<R> Serve<T, R>(string methodname, params object[] parameters) where T : class;

    Task Serve<T>(Func<T, Task> function) where T : class;
    Task Serve<T>(string methodname, params object[] parameters) where T : class;

    T CallService<T>() where T : class;

    T CallObject<T>() where T : class;
}