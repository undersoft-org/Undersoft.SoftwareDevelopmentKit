using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Undersoft.SDK.Service.Application.Components;

public abstract class ComponentModule2 : IdComponent, IAsyncDisposable
{
    protected JSModule? Module { get; set; }

    [NotNull]
    protected string? ModulePath { get; set; }

    [NotNull]
    protected string? ModuleName { get; set; }

    protected bool Relative { get; set; }

    protected bool JSObjectReference { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        OnLoadJSModule();
    }

    protected virtual void OnLoadJSModule()
    {
        var type = this.GetType();
        var inherited = type.GetCustomAttribute<JSModuleNotInheritedAttribute>() == null;
        if (inherited)
        {
            var attr = type.GetCustomAttribute<JSModuleAutoLoaderAttribute>();
            if (attr != null)
            {
                string? typeName = null;
                ModulePath = attr.Path ?? GetTypeName().ToLowerInvariant();
                ModuleName = attr.ModuleName ?? GetTypeName();
                JSObjectReference = attr.JSObjectReference;
                Relative = attr.Relative;

                string GetTypeName()
                {
                    typeName ??= type.GetTypeModuleName();
                    return typeName;
                }
            }
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !string.IsNullOrEmpty(ModulePath))
        {
            Module ??= JSObjectReference
                ? await JSRuntime.LoadModule2(ModulePath, this, Relative)
                : await JSRuntime.LoadModule2(ModulePath, Relative);
        }

        await ModuleInvokeVoidAsync(firstRender);
    }

    protected virtual async Task ModuleInvokeVoidAsync(bool firstRender)
    {
        if (firstRender)
        {
            await ModuleInitAsync();
        }
        else
        {
            await ModuleExecuteAsync();
        }
    }

    protected virtual Task ModuleInitAsync() => InvokeInitAsync(Id);

    protected virtual Task ModuleExecuteAsync() => Task.CompletedTask;

    protected Task InvokeInitAsync(params object?[]? args) => InvokeVoidAsync("init", args);

    protected Task InvokeExecuteAsync(params object?[]? args) => InvokeVoidAsync("execute", args);

    protected Task InvokeVoidAsync(string identifier, params object?[]? args) => InvokeVoidAsync(identifier, CancellationToken.None, args);

    protected async Task InvokeVoidAsync(string identifier, TimeSpan timeout, params object?[]? args)
    {
        if (Module != null)
        {
            await Module.InvokeVoidAsync($"{ModuleName}.{identifier}", timeout, args);
        }
    }

    protected async Task InvokeVoidAsync(string identifier, CancellationToken cancellationToken = default, params object?[]? args)
    {
        if (Module != null)
        {
            await Module.InvokeVoidAsync($"{ModuleName}.{identifier}", cancellationToken, args);
        }
    }

    protected Task<TValue?> InvokeAsync<TValue>(string identifier, params object?[]? args) => InvokeAsync<TValue?>(identifier, CancellationToken.None, args);

    protected async Task<TValue?> InvokeAsync<TValue>(string identifier, TimeSpan timeout, params object?[]? args)
    {
        TValue? ret = default;
        if (Module != null)
        {
            ret = await Module.InvokeAsync<TValue>($"{ModuleName}.{identifier}", timeout, args);
        }
        return ret;
    }

    protected async Task<TValue?> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken = default, params object?[]? args)
    {
        TValue? ret = default;
        if (Module != null)
        {
            ret = await Module.InvokeAsync<TValue>($"{ModuleName}.{identifier}", cancellationToken, args);
        }
        return ret;
    }

    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (Module != null && disposing)
        {
            await Module.InvokeVoidAsync($"{ModuleName}.dispose", Id);
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
