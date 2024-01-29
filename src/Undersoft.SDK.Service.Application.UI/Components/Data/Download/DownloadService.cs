namespace Undersoft.SDK.Service.Application.Components;

public class DownloadService
{
    protected List<(IComponent Key, Func<DownloadOption, Task> Callback)> StreamCache { get; } = new();

    protected List<(IComponent Key, Func<DownloadOption, Task> Callback)> UrlCache { get; } = new();

    protected internal virtual void RegisterStream(IComponent key, Func<DownloadOption, Task> callback) => StreamCache.Add((key, callback));

    protected internal virtual void RegisterUrl(IComponent key, Func<DownloadOption, Task> callback) => UrlCache.Add((key, callback));

    protected internal virtual void UnRegisterStream(IComponent key)
    {
        var item = StreamCache.FirstOrDefault(i => i.Key == key);
        if (item.Key != null)
        {
            StreamCache.Remove(item);
        }
    }

    protected internal virtual void UnRegisterUrl(IComponent key)
    {
        var item = UrlCache.FirstOrDefault(i => i.Key == key);
        if (item.Key != null)
        {
            UrlCache.Remove(item);
        }
    }

    public virtual async Task DownloadFromStreamAsync(DownloadOption option)
    {
        var cb = StreamCache.LastOrDefault().Callback;
        if (cb != null)
        {
            await cb.Invoke(option);
        }
    }

    public virtual async Task DownloadFromUrlAsync(DownloadOption option)
    {
        var cb = UrlCache.LastOrDefault().Callback;
        if (cb != null)
        {
            await cb.Invoke(option);
        }
    }
}
