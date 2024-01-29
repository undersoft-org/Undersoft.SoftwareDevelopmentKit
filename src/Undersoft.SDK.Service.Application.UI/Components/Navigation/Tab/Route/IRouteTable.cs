namespace Microsoft.AspNetCore.Components.Routing;

#if NET5_0
/// <summary>
/// Provides an abstraction over <see cref="RouteTable"/>.
/// the legacy route matching logic is removed.
/// </summary>
internal interface IRouteTable
{
    void Route(RouteContext routeContext);
}
#endif
