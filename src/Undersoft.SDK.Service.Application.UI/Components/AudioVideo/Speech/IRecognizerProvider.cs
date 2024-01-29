namespace Undersoft.SDK.Service.Application.Components;

public interface IRecognizerProvider
{
    Task InvokeAsync(RecognizerOption option);
}
