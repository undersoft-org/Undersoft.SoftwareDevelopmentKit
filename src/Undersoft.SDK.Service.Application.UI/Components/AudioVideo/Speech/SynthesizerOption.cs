namespace Undersoft.SDK.Service.Application.Components;

public class SynthesizerOption
{
    public string? MethodName { get; set; }

    public string? Text { get; set; }

    public Func<SynthesizerStatus, Task>? Callback { get; set; }

    public string SpeechSynthesisLanguage { get; set; } = "zh-CN";

    public string SpeechSynthesisVoiceName { get; set; } = "zh-CN-XiaoxiaoNeural";
}
