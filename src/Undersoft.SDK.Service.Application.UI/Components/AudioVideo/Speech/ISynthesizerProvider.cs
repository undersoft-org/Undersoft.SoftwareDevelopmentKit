namespace Undersoft.SDK.Service.Application.Components;

public interface ISynthesizerProvider
{
    Task InvokeAsync(SynthesizerOption option);
}
