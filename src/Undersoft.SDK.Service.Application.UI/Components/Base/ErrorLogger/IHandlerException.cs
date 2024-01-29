namespace Undersoft.SDK.Service.Application.Components;

public interface IHandlerException
{
    Task HandlerException(Exception ex, RenderFragment<Exception> errorContent);
}
