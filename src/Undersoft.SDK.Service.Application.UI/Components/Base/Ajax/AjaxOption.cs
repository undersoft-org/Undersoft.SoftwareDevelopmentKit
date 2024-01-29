namespace Undersoft.SDK.Service.Application.Components;

public class AjaxOption
{
    [NotNull]
    public object? Data { get; set; }

    public string Method { get; set; } = "POST";

    [NotNull]
    public string? Url { get; set; }
}
