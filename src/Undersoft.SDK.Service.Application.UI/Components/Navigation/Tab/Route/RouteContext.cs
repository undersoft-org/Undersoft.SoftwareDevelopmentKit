namespace Microsoft.AspNetCore.Components.Routing;

[ExcludeFromCodeCoverage]
internal class RouteContext
{
    private static readonly char[] Separator = new[] { '/' };

    public RouteContext(string path)
    {
        Segments = path.Trim('/').Split(Separator, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < Segments.Length; i++)
        {
            Segments[i] = Uri.UnescapeDataString(Segments[i]);
        }
    }

    public string[] Segments { get; }

#if NET6_0_OR_GREATER
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
    public Type? Handler { get; set; }

    public IReadOnlyDictionary<string, object>? Parameters { get; set; }
}
