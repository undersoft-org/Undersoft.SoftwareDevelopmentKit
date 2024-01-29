namespace Undersoft.SDK.Service.Application.Components;

public interface IErrorLogger
{
    Task HandlerExceptionAsync(Exception ex);

    bool ShowToast { get; }

    string? ToastTitle { get; }

    void Register(ComponentBase component);

    void UnRegister(ComponentBase component);
}
