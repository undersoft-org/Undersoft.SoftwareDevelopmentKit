using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Localization.Json;

internal class StringLocalizer : IStringLocalizer
{
    private readonly IStringLocalizer _localizer;

    public StringLocalizer(IStringLocalizerFactory factory, IOptions<JsonLocalizationOptions> options)
    {
        if (options.Value.ResourceManagerStringLocalizerType == null)
        {
            throw new InvalidOperationException();
        }
        _localizer = factory.Create(options.Value.ResourceManagerStringLocalizerType);
    }

    public LocalizedString this[string name] => _localizer[name];

    public LocalizedString this[string name, params object[] arguments] => _localizer[name, arguments];

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => _localizer.GetAllStrings(includeParentCultures);
}
