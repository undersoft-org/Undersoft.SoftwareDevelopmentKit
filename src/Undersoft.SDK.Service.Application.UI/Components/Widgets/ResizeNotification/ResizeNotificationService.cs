using System.Collections.Concurrent;

namespace Undersoft.SDK.Service.Application.Components;

public class ResizeNotificationService
{
    private ConcurrentDictionary<object, Func<BreakPoint, Task>> Cache { get; } = new();

    public void Subscribe(object target, Func<BreakPoint, Task> callback) => Cache.AddOrUpdate(target, k => callback, (k, v) => callback);

    public void Unsubscribe(object target) => Cache.TryRemove(target, out _);

    internal async Task InvokeAsync(BreakPoint breakPoint)
    {
        foreach (var cb in Cache.Values)
        {
            await cb(breakPoint);
        }
    }
}
