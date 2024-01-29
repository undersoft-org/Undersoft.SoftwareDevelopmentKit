namespace Undersoft.SDK.Service.Application.Components;

public class AjaxService
{
    private List<(IComponent Key, Func<AjaxOption, Task<string?>> Callback)> Cache { get; } = new();

    private List<(IComponent Key, Func<string, Task> Callback)> GotoCache { get; } = new();

    internal void Register(IComponent key, Func<AjaxOption, Task<string?>> callback) => Cache.Add((key, callback));

    internal void UnRegister(IComponent key)
    {
        var item = Cache.FirstOrDefault(i => i.Key == key);
        if (item.Key != null)
        {
            Cache.Remove(item);
        }
    }

    internal void RegisterGoto(IComponent key, Func<string, Task> callback) => GotoCache.Add((key, callback));

    internal void UnRegisterGoto(IComponent key)
    {
        var item = GotoCache.FirstOrDefault(i => i.Key == key);
        if (item.Key != null)
        {
            GotoCache.Remove(item);
        }
    }

    public async Task<string?> InvokeAsync(AjaxOption option)
    {
        var cb = Cache.FirstOrDefault().Callback;
        return cb == null ? null : await cb.Invoke(option);
    }

    public async Task Goto(string url)
    {
        var cb = GotoCache.FirstOrDefault().Callback;
        if (cb != null)
        {
            await cb.Invoke(url);
        }
    }
}
