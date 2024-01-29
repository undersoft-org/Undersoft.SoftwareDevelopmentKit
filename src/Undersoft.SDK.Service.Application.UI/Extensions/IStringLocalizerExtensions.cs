using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

internal static class IStringLocalizerExtensions
{
    public static bool TryGetLocalizerString(this IStringLocalizer localizer, string key, [MaybeNullWhen(false)] out string? text)
    {
        var ret = false;
        text = null;
        var l = localizer[key];
        if (l != null)
        {
            ret = !l.ResourceNotFound;
            if (ret)
            {
                text = l.Value;
            }
        }
        return ret;
    }
}
