namespace Undersoft.SDK.Service.Application.Components;

public class SynthesizerService
{
    private ISynthesizerProvider Provider { get; }

    public SynthesizerService(ISynthesizerProvider provider)
    {
        Provider = provider;
    }

    public Task InvokeAsync(SynthesizerOption option) => Provider.InvokeAsync(option);
}
