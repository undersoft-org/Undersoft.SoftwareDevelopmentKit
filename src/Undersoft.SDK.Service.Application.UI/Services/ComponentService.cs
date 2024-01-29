namespace Undersoft.SDK.Service.Application.Components;

public abstract class ComponentService<TOption>
{
    protected List<(ComponentBase Key, Func<TOption, Task> Callback)> Cache { get; } = new();

    protected async Task Invoke(TOption option, ComponentBase? component = null)
    {
        var (Key, Callback) = component != null
            ? Cache.FirstOrDefault(k => k.Key == component)
            : Cache.FirstOrDefault();
        if (Callback == null)
        {
            throw new InvalidOperationException($"{GetType().Name} not registerd. refer doc https://www.blazor.zone/install-server step 7 for RootComponent");
        }
        await Callback.Invoke(option);
    }

    internal void Register(ComponentBase key, Func<TOption, Task> callback)
    {
        Cache.Add((key, callback));
    }

    internal void UnRegister(ComponentBase key)
    {
        var item = Cache.FirstOrDefault(i => i.Key == key);
        if (item.Key != null) Cache.Remove(item);
    }
}
