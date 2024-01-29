namespace Undersoft.SDK.Service.Application.Components;

public class JSModule2<TCom> : JSModule where TCom : class
{
    protected DotNetObjectReference<TCom> DotNetReference { get; }

    public JSModule2(IJSObjectReference? jSObjectReference, TCom value) : base(jSObjectReference)
    {
        DotNetReference = DotNetObjectReference.Create(value);
    }

    public override async ValueTask InvokeVoidAsync(string identifier, CancellationToken cancellationToken = default, params object?[]? args)
    {
        var paras = new List<object?>();
        if (args != null)
        {
            if (args.Length > 0)
            {
                paras.Add(args[0]);
            }
            paras.Add(DotNetReference);
            if (args.Length > 1)
            {
                paras.AddRange(args.Skip(1).Take(args.Length - 1));
            }
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

    public override async ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken = default, params object?[]? args)
    {
        var paras = new List<object?>();
        if (args != null)
        {
            if (args.Length > 0)
            {
                paras.Add(args[0]);
            }
            paras.Add(DotNetReference);
            if (args.Length > 1)
            {
                paras.AddRange(args.Skip(1).Take(args.Length - 1));
            }
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

    protected override ValueTask DisposeAsyncCore(bool disposing)
    {
        if (disposing)
        {
            DotNetReference.Dispose();
        }
        return base.DisposeAsyncCore(disposing);
    }
}
