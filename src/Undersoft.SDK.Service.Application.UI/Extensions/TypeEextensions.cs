using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using System.Reflection;

namespace Undersoft.SDK.Service.Application.Components;

[ExcludeFromCodeCoverage]
internal static class TypeEextensions
{
    public static PropertyInfo? GetPropertyByName(this Type type, string propertyName) => type.GetRuntimeProperties().FirstOrDefault(p => p.Name == propertyName);

    public static FieldInfo? GetFieldByName(this Type type, string fieldName) => type.GetRuntimeFields().FirstOrDefault(p => p.Name == fieldName);

    public static async Task<bool> IsAuthorizedAsync(this Type type, Task<AuthenticationState>? authenticateState, IAuthorizationPolicyProvider? authorizePolicy, IAuthorizationService? authorizeService, object? resource = null)
    {
        var ret = true;
        var authorizeData = AttributeAuthorizeDataCache.GetAuthorizeDataForType(type);
        if (authorizeData != null)
        {
            EnsureNoAuthenticationSchemeSpecified();

            if (authenticateState != null && authorizePolicy != null && authorizeService != null)
            {
                var currentAuthenticationState = await authenticateState;
                var user = currentAuthenticationState.User;
                var policy = await AuthorizationPolicy.CombineAsync(authorizePolicy, authorizeData);
                if (policy != null)
                {
                    var result = await authorizeService.AuthorizeAsync(user, resource, policy);
                    ret = result.Succeeded;
                }
            }
        }
        return ret;

        void EnsureNoAuthenticationSchemeSpecified()
        {
            for (var i = 0; i < authorizeData.Length; i++)
            {
                var entry = authorizeData[i];
                if (!string.IsNullOrEmpty(entry.AuthenticationSchemes))
                {
                    throw new NotSupportedException($"The authorization data specifies an authentication scheme with value '{entry.AuthenticationSchemes}'. Authentication schemes cannot be specified for components.");
                }
            }
        }
    }
}
