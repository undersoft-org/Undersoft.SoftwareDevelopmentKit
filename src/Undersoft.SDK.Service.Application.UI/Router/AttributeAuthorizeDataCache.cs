using Microsoft.AspNetCore.Authorization;
using System.Collections.Concurrent;

namespace Undersoft.SDK.Service.Application.Components;

[ExcludeFromCodeCoverage]
internal static class AttributeAuthorizeDataCache
{
    private static readonly ConcurrentDictionary<Type, IAuthorizeData[]?> _cache = new();

    public static IAuthorizeData[]? GetAuthorizeDataForType(Type type)
    {
        if (!_cache.TryGetValue(type, out var result))
        {
            result = ComputeAuthorizeDataForType(type);
            _cache[type] = result;         
        }

        return result;
    }

    private static IAuthorizeData[]? ComputeAuthorizeDataForType(Type type)
    {
        var allAttributes = type.GetCustomAttributes(inherit: true);
        if (allAttributes.OfType<IAllowAnonymous>().Any())
        {
            return null;
        }

        var authorizeDataAttributes = allAttributes.OfType<IAuthorizeData>().ToArray();
        return authorizeDataAttributes.Length > 0 ? authorizeDataAttributes : null;
    }
}
