namespace Undersoft.SDK.Service.Application.Components;

internal class DefaultDispatchService<TEntry> : IDispatchService<TEntry>
{
    public void Dispatch(DispatchEntry<TEntry> payload)
    {
        lock (locker)
        {
            Cache.ForEach(cb =>
            {
                cb(payload);
            });
        }
    }

    public void Subscribe(Func<DispatchEntry<TEntry>, Task> callback)
    {
        lock (locker)
        {
            Cache.Add(callback);
        }
    }

    public void UnSubscribe(Func<DispatchEntry<TEntry>, Task> callback)
    {
        lock (locker)
        {
            Cache.Remove(callback);
        }
    }

    private List<Func<DispatchEntry<TEntry>, Task>> Cache { get; } = new(50);

    private readonly object locker = new();
}
