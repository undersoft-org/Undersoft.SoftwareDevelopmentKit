namespace Undersoft.SDK.Service.Application.Components;

public static class RecognizerServiceExtensions
{
    public static async Task RecognizeOnceAsync(this RecognizerService service, Func<RecognizerStatus, string?, Task> callback, int? autoRecoginzerElapsedMilliseconds = null)
    {
        var option = new RecognizerOption()
        {
            MethodName = "bb_baidu_speech_recognizeOnce",
            Callback = callback
        };

        if (autoRecoginzerElapsedMilliseconds.HasValue)
        {
            option.AutoRecoginzerElapsedMilliseconds = autoRecoginzerElapsedMilliseconds.Value;
        }
        await service.InvokeAsync(option);
    }

    public static async Task RecognizeOnceAsync(this IRecognizerProvider provider, Func<RecognizerStatus, string?, Task> callback, int? autoRecoginzerElapsedMilliseconds = null)
    {
        var option = new RecognizerOption()
        {
            MethodName = "bb_baidu_speech_recognizeOnce",
            Callback = callback
        };

        if (autoRecoginzerElapsedMilliseconds.HasValue)
        {
            option.AutoRecoginzerElapsedMilliseconds = autoRecoginzerElapsedMilliseconds.Value;
        }
        await provider.InvokeAsync(option);
    }

    public static async Task CloseAsync(this RecognizerService service, Func<RecognizerStatus, string?, Task>? callback = null)
    {
        var option = new RecognizerOption()
        {
            MethodName = "bb_baidu_speech_close",
            Callback = callback
        };
        await service.InvokeAsync(option);
    }

    public static async Task CloseAsync(this IRecognizerProvider provider, Func<RecognizerStatus, string?, Task>? callback = null)
    {
        var option = new RecognizerOption()
        {
            MethodName = "bb_baidu_speech_close",
            Callback = callback
        };
        await provider.InvokeAsync(option);
    }
}
