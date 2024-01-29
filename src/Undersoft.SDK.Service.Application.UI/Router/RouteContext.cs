namespace Undersoft.SDK.Service.Application.Components;

[ExcludeFromCodeCoverage]
internal class RouteContext
{
    public string[]? Segments { get; set; }

    public Type? Handler { get; set; }

    public IReadOnlyDictionary<string, object> Parameters { get; set; }
}
