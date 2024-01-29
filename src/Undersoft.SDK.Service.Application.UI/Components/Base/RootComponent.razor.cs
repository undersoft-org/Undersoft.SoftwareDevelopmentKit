using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Undersoft.SDK.Service.Application.Components;

public partial class RootComponent
{
    [Inject]
    [NotNull]
    private ICacheManager? Cache { get; set; }

    [Inject]
    [NotNull]
    private IOptionsMonitor<ApplicationOptions>? Options { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [NotNull]
    public Message? MessageContainer { get; private set; }

    [NotNull]
    public ToastContainer? ToastContainer { get; private set; }

    [Parameter]
    public Func<ILogger, Exception, Task>? OnErrorHandleAsync { get; set; }

    [Parameter]
    public bool ShowToast { get; set; } = true;

    [Parameter]
    public string? ToastTitle { get; set; }

    [Parameter]
    public bool? EnableErrorLogger { get; set; }

    private bool EnableErrorLoggerValue => EnableErrorLogger ?? Options.CurrentValue.EnableErrorLogger;

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        Cache.SetStartTime();

        await base.SetParametersAsync(parameters);
    }

    private RenderFragment RenderBody() => builder =>
    {
        if (EnableErrorLoggerValue)
        {
            builder.OpenComponent<ErrorLogger>(0);
            builder.AddAttribute(1, nameof(ErrorLogger.ShowToast), ShowToast);
            builder.AddAttribute(2, nameof(ErrorLogger.ToastTitle), ToastTitle);
            if (OnErrorHandleAsync != null)
            {
                builder.AddAttribute(3, nameof(ErrorLogger.OnErrorHandleAsync), OnErrorHandleAsync);
            }
            builder.AddAttribute(4, nameof(ErrorLogger.ChildContent), RenderContent);
            builder.CloseComponent();
        }
        else
        {
            builder.AddContent(0, RenderContent);
        }
    };

    private static RenderFragment RenderComponents() => builder =>
    {
        builder.OpenComponent<Dialog>(0);
        builder.CloseComponent();

        builder.OpenComponent<Ajax>(1);
        builder.CloseComponent();

        builder.OpenComponent<SweetAlert>(2);
        builder.CloseComponent();

        builder.OpenComponent<Print>(3);
        builder.CloseComponent();

        builder.OpenComponent<Download>(4);
        builder.CloseComponent();
    };

    private RenderFragment RenderContent => builder =>
    {
        Render();

        [ExcludeFromCodeCoverage]
        void Render()
        {
            if (OperatingSystem.IsBrowser())
            {
                builder.AddContent(0, RenderChildContent);
                builder.AddContent(1, RenderComponents());
            }
            else
            {
                builder.OpenElement(0, "app");
                builder.AddContent(1, RenderChildContent);
                builder.CloseElement();
                builder.AddContent(2, RenderComponents());
            }
        }
    };
}
