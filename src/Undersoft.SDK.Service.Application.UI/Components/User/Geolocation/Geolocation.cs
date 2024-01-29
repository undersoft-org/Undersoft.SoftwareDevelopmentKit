namespace Undersoft.SDK.Service.Application.Components;

public static class Geolocation
{
    public static ValueTask<bool> GetLocaltion<TComponent>(JSInterop<TComponent> interop, TComponent component, string callbackMethodName) where TComponent : class => interop.GetGeolocationItemAsync(component, callbackMethodName);

    public static ValueTask<long> WatchPosition<TComponent>(JSInterop<TComponent> interop, TComponent component, string callbackMethodName) where TComponent : class => interop.GetWatchPositionItemAsync(component, callbackMethodName);

    public static ValueTask<bool> ClearWatchPosition<TComponent>(JSInterop<TComponent> interop, long watchID) where TComponent : class => interop.SetClearWatchPositionAsync(watchID);
}
