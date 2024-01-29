namespace Undersoft.SDK.Service.Application.Components;

public static class BrowserNotification
{
    public static ValueTask CheckPermission<TComponent>(JSInterop<TComponent> interop, TComponent component, string? callbackMethodName = null, bool requestPermission = true) where TComponent : class => interop.CheckNotifyPermissionAsync(component, callbackMethodName, requestPermission);

    public static async Task<bool> Dispatch<TComponent>(JSInterop<TComponent> interop, TComponent component, NotificationItem model, string? callbackMethodName = null) where TComponent : class
    {
        var ret = await interop.Dispatch(component, model, callbackMethodName);
        return ret;
    }
}
