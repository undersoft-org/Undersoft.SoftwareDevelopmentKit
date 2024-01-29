namespace Undersoft.SDK.Service.Application.Components;

public class RecognizerOption
{
    public string? MethodName { get; set; }

    public Func<RecognizerStatus, string?, Task>? Callback { get; set; }

    public string SpeechRecognitionLanguage { get; set; } = "zh-CN";

    public string TargetLanguage { get; set; } = "zh-CN";

    public int AutoRecoginzerElapsedMilliseconds { get; set; } = 5000;
}
