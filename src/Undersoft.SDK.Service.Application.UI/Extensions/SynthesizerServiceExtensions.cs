namespace Undersoft.SDK.Service.Application.Components;

public static class SynthesizerServiceExtensions
{
    public static async Task SynthesizerOnceAsync(this SynthesizerService service, string? text, Func<SynthesizerStatus, Task>? callback = null)
    {
        var option = new SynthesizerOption()
        {
            Text = text,
            MethodName = "bb_baidu_speech_synthesizerOnce",
            Callback = callback
        };
        await service.InvokeAsync(option);
    }

    public static async Task SynthesizerOnceAsync(this ISynthesizerProvider provider, string? text, Func<SynthesizerStatus, Task>? callback = null)
    {
        var option = new SynthesizerOption()
        {
            Text = text,
            MethodName = "bb_baidu_speech_synthesizerOnce",
            Callback = callback
        };
        await provider.InvokeAsync(option);
    }

    public static async Task CloseAsync(this SynthesizerService service, Func<SynthesizerStatus, Task>? callback = null)
    {
        var option = new SynthesizerOption()
        {
            MethodName = "bb_baidu_close_synthesizer",
            Callback = callback
        };
        await service.InvokeAsync(option);
    }

    public static async Task CloseAsync(this ISynthesizerProvider provider, Func<SynthesizerStatus, Task>? callback = null)
    {
        var option = new SynthesizerOption()
        {
            MethodName = "bb_baidu_close_synthesizer",
            Callback = callback
        };
        await provider.InvokeAsync(option);
    }
}
