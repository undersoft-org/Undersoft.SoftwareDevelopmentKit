using Microsoft.Extensions.Caching.Memory;

namespace Undersoft.SDK.Service.Application.Components;

[ExcludeFromCodeCoverage]
internal static class ICacheEntryExtensions
{
    public static ICacheEntry SetSlidingExpirationForDynamicAssembly(this ICacheEntry entry, TimeSpan? offset = null)
    {
        entry.SlidingExpiration = offset ?? TimeSpan.FromSeconds(10);
        return entry;
    }

    public static void SetDynamicAssemblyPolicy(this ICacheEntry entry, Type? type)
    {
        if (type?.Assembly.IsDynamic ?? false)
        {
            entry.SetSlidingExpiration(TimeSpan.FromSeconds(10));
        }
    }
}
