namespace Undersoft.SDK.Service.Application.Components;

public class JSModule : IAsyncDisposable
{
    [NotNull]
    protected IJSObjectReference? Module { get; }

    public JSModule(IJSObjectReference? jSObjectReference)
    {
        Module = jSObjectReference ?? throw new ArgumentNullException(nameof(jSObjectReference));
    }

    public virtual ValueTask InvokeVoidAsync(string identifier, params object?[]? args) => InvokeVoidAsync(identifier, CancellationToken.None, args);

    public virtual ValueTask InvokeVoidAsync(string identifier, TimeSpan timeout, params object?[]? args)
    {
        using CancellationTokenSource? cancellationTokenSource = ((timeout == Timeout.InfiniteTimeSpan) ? null : new CancellationTokenSource(timeout));
        CancellationToken cancellationToken = cancellationTokenSource?.Token ?? CancellationToken.None;
        return InvokeVoidAsync(identifier, cancellationToken, args);
    }

    public virtual async ValueTask InvokeVoidAsync(string identifier, CancellationToken cancellationToken = default, params object?[]? args)
    {
        var paras = new List<object?>();
        if (args != null)
        {
            paras.AddRange(args);
        }
        await InvokeVoidAsync();

        [ExcludeFromCodeCoverage]
        async ValueTask InvokeVoidAsync()
        {
            try
            {
                await Module.InvokeVoidAsync(identifier, cancellationToken, paras.ToArray());
            }
#if NET6_0_OR_GREATER
            catch (JSDisconnectedException) { }
#endif
#if DEBUG
#else
            catch (JSException) { }
            catch (AggregateException) { }
            catch (InvalidOperationException) { }
#endif
            catch (TaskCanceledException) { }
        }
    }

    public virtual ValueTask<TValue> InvokeAsync<TValue>(string identifier, params object?[]? args) => InvokeAsync<TValue>(identifier, CancellationToken.None, args);

    public virtual ValueTask<TValue> InvokeAsync<TValue>(string identifier, TimeSpan timeout, params object?[]? args)
    {
        using CancellationTokenSource? cancellationTokenSource = ((timeout == Timeout.InfiniteTimeSpan) ? null : new CancellationTokenSource(timeout));
        CancellationToken cancellationToken = cancellationTokenSource?.Token ?? CancellationToken.None;
        return InvokeAsync<TValue>(identifier, cancellationToken, args);
    }

    public virtual async ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken = default, params object?[]? args)
    {
        var paras = new List<object?>();
        if (args != null)
        {
            paras.AddRange(args!);
        }
        return await InvokeAsync();

        [ExcludeFromCodeCoverage]
        async ValueTask<TValue> InvokeAsync()
        {
            TValue ret = default!;
            try
            {
                ret = await Module.InvokeAsync<TValue>(identifier, cancellationToken, paras.ToArray());
            }
#if NET6_0_OR_GREATER
            catch (JSDisconnectedException) { }
#endif
#if DEBUG
#else
            catch (JSException) { }
            catch (AggregateException) { }
            catch (InvalidOperationException) { }
#endif
            catch (TaskCanceledException) { }

            return ret;
        }
    }

    protected virtual async ValueTask DisposeAsyncCore(bool disposing)
    {
        if (disposing)
        {
            try
            {
                await Module.DisposeAsync();
            }
            catch { }
        }
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore(true);
        GC.SuppressFinalize(this);
    }
}
