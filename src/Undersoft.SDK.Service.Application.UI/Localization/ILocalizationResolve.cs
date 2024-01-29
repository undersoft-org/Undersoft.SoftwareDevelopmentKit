using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Localization;

public interface ILocalizationResolve
{
    IEnumerable<LocalizedString> GetAllStringsByCulture(bool includeParentCultures);
}
