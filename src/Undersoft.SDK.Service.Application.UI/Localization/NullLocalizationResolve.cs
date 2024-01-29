using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Localization;

internal class NullLocalizationResolve : ILocalizationResolve
{
    public IEnumerable<LocalizedString> GetAllStringsByCulture(bool includeParentCultures) => Enumerable.Empty<LocalizedString>();
}
