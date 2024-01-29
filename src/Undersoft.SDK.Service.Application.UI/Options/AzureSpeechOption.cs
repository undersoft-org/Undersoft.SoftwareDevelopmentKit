namespace Undersoft.SDK.Service.Application.Components;

public class AzureSpeechOption
{
    [NotNull]
    public string? SubscriptionKey { get; set; }

    [NotNull]
    public string? Region { get; set; }

    [NotNull]
    public string? AuthorizationTokenUrl { get; set; }

    public int Timeout { get; set; }
}
