using Microsoft.JSInterop;
using Undersoft.SDK.Service.Application.Extensions;

namespace Undersoft.SDK.Service.Application.Components;

public class JSInterop<TValue> : IDisposable where TValue : class
{
    private IJSRuntime JSRuntime { get; }

    private DotNetObjectReference<TValue>? _objRef;

    public JSInterop(IJSRuntime jsRuntime)
    {
        JSRuntime = jsRuntime;
    }

    public async ValueTask InvokeVoidAsync(string identifier, TValue value, params object?[]? args)
    {
        _objRef = DotNetObjectReference.Create(value);
#if NET5_0
        var paras = new List<object>()
        {
            _objRef
        };
#else
        var paras = new List<object?>()
        {
            _objRef
        };
#endif
        if (args != null)
        {
            paras.AddRange(args!);
        }
        await JSRuntime.InvokeVoidAsync(identifier: identifier, paras.ToArray());
    }

    public async ValueTask InvokeVoidAsync(TValue value, object? el, string func, params object[] args)
    {
        _objRef = DotNetObjectReference.Create(value);
        var paras = new List<object>()
        {
            _objRef
        };
        paras.AddRange(args);
        await JSRuntime.InvokeVoidAsync(el, func, paras.ToArray());
    }

    internal ValueTask<bool> GetGeolocationItemAsync(TValue value, string callbackMethodName)
    {
        _objRef = DotNetObjectReference.Create(value);
        return JSRuntime.InvokeAsync<bool>("$.bb_geo_getCurrnetPosition", _objRef, callbackMethodName);
    }

    internal ValueTask<long> GetWatchPositionItemAsync(TValue value, string callbackMethodName)
    {
        _objRef = DotNetObjectReference.Create(value);
        return JSRuntime.InvokeAsync<long>("$.bb_geo_watchPosition", _objRef, callbackMethodName);
    }

    internal ValueTask<bool> SetClearWatchPositionAsync(long watchid)
    {
        return JSRuntime.InvokeAsync<bool>("$.bb_geo_clearWatchLocation", watchid);
    }

    internal ValueTask CheckNotifyPermissionAsync(TValue value, string? callbackMethodName = null, bool requestPermission = true)
    {
        _objRef = DotNetObjectReference.Create(value);
        return JSRuntime.InvokeVoidAsync("$.bb_notify_checkPermission", _objRef, callbackMethodName ?? "", requestPermission);
    }

    internal ValueTask<bool> Dispatch(TValue value, NotificationItem model, string? callbackMethodName = null)
    {
        _objRef = DotNetObjectReference.Create(value);
        return JSRuntime.InvokeAsync<bool>("$.bb_notify_display", _objRef, callbackMethodName, model);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_objRef != null)
            {
                _objRef.Dispose();
                _objRef = null;
            }
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
