using MediatR;

namespace Undersoft.SDK.Service;

using Invoking;
using Microsoft.Extensions.DependencyInjection;
using Undersoft.SDK.Service.Data.Repository.Client;
using Undersoft.SDK.Service.Data.Repository.Source;

public class Servicer : ServiceManager, IServicer, IMediator
{
    new bool disposedValue;
    protected IMediator mediator;

    public Servicer() : base() { }

    protected override void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                base.Dispose(true);
            }
            disposedValue = true;
        }
    }

    protected IMediator Mediator => mediator ??= GetService<IMediator>();

    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(
        IStreamRequest<TResponse> request,
        CancellationToken cancellationToken = default
    )
    {
        return Mediator.CreateStream(request, cancellationToken);
    }

    public IAsyncEnumerable<object> CreateStream(
        object request,
        CancellationToken cancellationToken = default
    )
    {
        return mediator.CreateStream(request, cancellationToken);
    }

    public Lazy<R> Deserve<T, R>(Func<T, R> function)
        where T : class
        where R : class
    {
        return new Lazy<R>(function.Invoke(GetService<T>()));
    }

    public override async ValueTask DisposeAsyncCore()
    {
        await base.DisposeAsyncCore().ConfigureAwait(false);
    }

    public async Task Publish(
        object notification,
        CancellationToken cancellationToken = default
    )
    {
        await Serve<IMediator>(async (m) => await m.Publish(notification, cancellationToken));
    }

    public async Task Publish<TNotification>(
        TNotification notification,
        CancellationToken cancellationToken = default
    ) where TNotification : INotification
    {
        await Serve<IMediator>(async (m) => await m.Publish(notification, cancellationToken));
    }

    public Task<R> Run<T, R>(Func<T, Task<R>> function) where T : class
    {
        return Task.Run(async () => await function.Invoke(GetService<T>()));
    }

    public Task Run<T>(Func<T, Task> function) where T : class
    {
        return Task.Run(async () => await function.Invoke(GetService<T>()));
    }

    public Task Run<T>(string methodname, params object[] parameters) where T : class
    {
        Invoker deputy = new Invoker(EnsureGetRootService<T>(), methodname);
        return deputy.InvokeAsync(parameters);
    }

    public Task<R> Run<T, R>(string methodname, params object[] parameters) where T : class
    {
        Invoker deputy = new Invoker(EnsureGetRootService<T>(), methodname);
        return deputy.InvokeAsync<R>(parameters);
    }

    public async Task Save(bool asTransaction = false)
    {
        await SaveEndpoints(true);
        await SaveClients(true);
    }

    public async Task<int> SaveClient(IRepositoryClient client, bool asTransaction = false)
    {
        return await client.Save(asTransaction);
    }

    public async Task<int> SaveClients(bool asTransaction = false)
    {
        int changes = 0;
        for (int i = 0; i < Clients.Count; i++)
        {
            changes += await SaveClient(Clients[i], asTransaction);
        }

        return changes;
    }

    public async Task<int> SaveEndpoint(
        IRepositorySource endpoint,
        bool asTransaction = false
    )
    {
        return await endpoint.Save(asTransaction);
    }

    public async Task<int> SaveEndpoints(bool asTransaction = false)
    {
        int changes = 0;
        for (int i = 0; i < Sources.Count; i++)
        {
            changes += await SaveEndpoint(Sources[i], asTransaction);
        }

        return changes;
    }

    public async Task<TResponse> Send<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default
    )
    {
        return await Mediator.Send(request, cancellationToken);
    }

    public async Task<object> Send(
        object request,
        CancellationToken cancellationToken = default
    )
    {
        return await Mediator.Send(request, cancellationToken);
    }

    public Task<R> Serve<T, R>(Func<T, Task<R>> function) where T : class
    {
        return Task.Run(async () =>
        {
            using (Servicer us = new Servicer())
            {
                return await function.Invoke(us.GetService<T>());
            }
        });
    }

    public Task Serve<T>(Func<T, Task> function) where T : class
    {
        return Task.Run(async () =>
        {
            using (Servicer us = new Servicer())
            {
                await function.Invoke(us.GetService<T>());
            }
        });
    }

    public Task Serve<T>(string methodname, params object[] parameters) where T : class
    {
        return Task.Run(() =>
        {
            using (Servicer us = new Servicer())
            {
                Invoker deputy = new Invoker(us.EnsureGetRootService<T>(), methodname);
                return deputy.Invoke(parameters);
            }
        });
    }

    public Task<R> Serve<T, R>(string methodname, params object[] parameters) where T : class
    {
        return Task.Run(async () =>
        {
            using (Servicer us = new Servicer())
            {
                Invoker deputy = new Invoker(us.EnsureGetRootService<T>(), methodname);
                return await deputy.InvokeAsync<R>(parameters);
            }
        });
    }

    public T CallService<T>() where T : class
    {
        return GetManager().GetService<T>();
    }

    public T CallObject<T>() where T : class
    {
        return GetRegistry().GetObject<T>();
    }


}
