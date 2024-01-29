namespace Undersoft.SDK.Service.Application.Components;

public class TitleService
{
    private List<(IComponent Key, Func<string, ValueTask> Callback)> Cache { get; } = new();

    public async ValueTask SetTitle(string title)
    {
        var cb = Cache.FirstOrDefault().Callback;
        if (cb != null)
        {
            await cb.Invoke(title);
        }
    }

    internal void Register(IComponent key, Func<string, ValueTask> callback) => Cache.Add((key, callback));

    internal void UnRegister(IComponent key)
    {
        var item = Cache.FirstOrDefault(i => i.Key == key);
        if (item.Key != null) Cache.Remove(item);
    }
}
