using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Undersoft.SDK.Service.Application.Components;

public class ErrorLogger
#if NET6_0_OR_GREATER
    : ErrorBoundaryBase, IErrorLogger
#else
    : ComponentBase, IErrorLogger
#endif
{
    [Inject]
    [NotNull]
    private ILogger<ErrorLogger>? Logger { get; set; }

    [Inject]
    [NotNull]
    private IConfiguration? Configuration { get; set; }

    [Inject]
    [NotNull]
    private ToastService? ToastService { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<ErrorLogger>? Localizer { get; set; }

    [Parameter]
    public bool ShowToast { get; set; } = true;

    [Parameter]
    public string? ToastTitle { get; set; }

    [Parameter]
    public Func<ILogger, Exception, Task>? OnErrorHandleAsync { get; set; }

#if NET6_0_OR_GREATER
    [Inject]
    [NotNull]
    private IErrorBoundaryLogger? ErrorBoundaryLogger { get; set; }
#else
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    [NotNull]
    public RenderFragment<Exception>? ErrorContent { get; set; }
#endif

    protected Exception? Exception { get; set; }

    private bool ShowErrorDetails { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        ToastTitle ??= Localizer[nameof(ToastTitle)];

        ShowErrorDetails = Configuration.GetValue<bool>("DetailedErrors", false);

        if (ShowErrorDetails)
        {
            ErrorContent ??= RenderException();
        }
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        Exception = null;

#if NET6_0_OR_GREATER
        Recover();
#endif
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<CascadingValue<IErrorLogger>>(0);
        builder.AddAttribute(1, nameof(CascadingValue<IErrorLogger>.Value), this);
        builder.AddAttribute(2, nameof(CascadingValue<IErrorLogger>.IsFixed), true);

        var content = ChildContent;
#if NET6_0_OR_GREATER
        var ex = Exception ?? CurrentException;
#else
        var ex = Exception;
#endif
        if (ex != null && ErrorContent != null)
        {
            if (Cache.Any())
            {
                var component = Cache.Last();
                if (component is IHandlerException handler)
                {
                    handler.HandlerException(ex, ErrorContent);
                }
            }
            else
            {
                content = ErrorContent.Invoke(ex);
            }
        }
        builder.AddAttribute(3, nameof(CascadingValue<IErrorLogger>.ChildContent), content);
        builder.CloseComponent();
    }

    private RenderFragment<Exception> RenderException() => ex => builder =>
    {
        var index = 0;
        builder.OpenElement(index++, "div");
        builder.AddAttribute(index++, "class", "error-stack");
        builder.AddContent(index++, ex.FormatMarkupString(Configuration.GetEnvironmentInformation()));
        builder.CloseElement();
    };

    public async Task HandlerExceptionAsync(Exception exception)
    {
        await OnErrorAsync(exception);

        if (OnErrorHandleAsync is null && ShowErrorDetails)
        {
            Exception = exception;
            StateHasChanged();
        }
    }

#if NET6_0_OR_GREATER
    protected override async Task OnErrorAsync(Exception exception)
#else
    protected async Task OnErrorAsync(Exception exception)
#endif
    {
        if (OnErrorHandleAsync != null)
        {
            await OnErrorHandleAsync(Logger, exception);
        }
        else
        {
            if (ShowToast)
            {
                await ToastService.Error(ToastTitle, exception.Message);
            }

#if NET6_0_OR_GREATER
            await ErrorBoundaryLogger.LogErrorAsync(exception);
#else
            Logger.LogError(exception, "");
#endif
        }
    }

    private List<ComponentBase> Cache { get; } = new();

    public void Register(ComponentBase component)
    {
        Cache.Add(component);
    }

    public void UnRegister(ComponentBase component)
    {
        Cache.Remove(component);
    }
}
