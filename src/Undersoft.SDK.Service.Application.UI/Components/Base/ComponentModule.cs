using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Undersoft.SDK.Service.Application.Components;

public abstract class ComponentModule : IdComponent, IAsyncDisposable
{
    protected JSModule? Module { get; set; }

    [NotNull]
    protected string? ModulePath { get; set; }

    [NotNull]
    protected string? ModuleName { get; set; }

    protected bool Relative { get; set; }

    protected bool AutoInvokeInit { get; set; } = true;

    protected bool AutoInvokeDispose { get; set; } = true;

    protected DotNetObjectReference<ComponentModule>? Interop { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        OnLoadJSModule();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !string.IsNullOrEmpty(ModulePath))
        {
            Module ??= await JSRuntime.LoadModule(ModulePath, Relative);

            if (AutoInvokeInit)
            {
                await InvokeInitAsync();
            }
        }
    }

    protected virtual void OnLoadJSModule()
    {
        var type = this.GetType();
        var attr = type.GetCustomAttribute<JSModuleAutoLoaderAttribute>(false);
        if (attr != null)
        {
            string? typeName = null;
            ModulePath = attr.Path ?? GetTypeName();
            ModuleName = attr.ModuleName ?? GetTypeName();
            Relative = attr.Relative;
            AutoInvokeDispose = attr.AutoInvokeDispose;
            AutoInvokeInit = attr.AutoInvokeInit;

            if (attr.JSObjectReference)
            {
                Interop = DotNetObjectReference.Create<ComponentModule>(this);
            }

            string GetTypeName()
            {
                typeName ??= type.GetTypeModuleName();
                return typeName;
            }
        }
    }

    protected virtual Task InvokeInitAsync() => InvokeVoidAsync("init", Id);

    protected Task InvokeVoidAsync(string identifier, params object?[]? args) => InvokeVoidAsync(identifier, CancellationToken.None, args);

    protected async Task InvokeVoidAsync(string identifier, TimeSpan timeout, params object?[]? args)
    {
        if (Module != null)
        {
            await Module.InvokeVoidAsync(identifier, timeout, args);
        }
    }

    protected async Task InvokeVoidAsync(string identifier, CancellationToken cancellationToken = default, params object?[]? args)
    {
        if (Module != null)
        {
            await Module.InvokeVoidAsync(identifier, cancellationToken, args);
        }
    }

    protected Task<TValue?> InvokeAsync<TValue>(string identifier, params object?[]? args) => InvokeAsync<TValue?>(identifier, CancellationToken.None, args);

    protected async Task<TValue?> InvokeAsync<TValue>(string identifier, TimeSpan timeout, params object?[]? args)
    {
        TValue? ret = default;
        if (Module != null)
        {
            ret = await Module.InvokeAsync<TValue>(identifier, timeout, args);
        }
        return ret;
    }

    protected async Task<TValue?> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken = default, params object?[]? args)
    {
        TValue? ret = default;
        if (Module != null)
        {
            ret = await Module.InvokeAsync<TValue>(identifier, cancellationToken, args);
        }
        return ret;
    }

    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (Module != null && disposing)
        {
            if (AutoInvokeDispose)
            {
                await Module.InvokeVoidAsync("dispose", Id);
            }

            if (Interop != null)
            {
                Interop.Dispose();
            }

            await Module.DisposeAsync();
            Module = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }
}
