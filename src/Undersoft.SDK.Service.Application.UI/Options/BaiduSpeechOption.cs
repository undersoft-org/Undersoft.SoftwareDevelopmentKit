namespace Undersoft.SDK.Service.Application.Components;

public class BaiduSpeechOption
{
    [NotNull]
    public string? AppId { get; set; }

    [NotNull]
    public string? ApiKey { get; set; }

    [NotNull]
    public string? Secret { get; set; }

    public int Speed { get; set; } = 5;
}
