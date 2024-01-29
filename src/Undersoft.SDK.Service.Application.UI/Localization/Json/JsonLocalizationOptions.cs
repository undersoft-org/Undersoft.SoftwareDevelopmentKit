using Microsoft.Extensions.Localization;
using System.Reflection;

namespace Undersoft.SDK.Service.Application.Localization.Json;

public class JsonLocalizationOptions : LocalizationOptions
{
    public Type? ResourceManagerStringLocalizerType { get; set; }

    public IEnumerable<Assembly>? AdditionalJsonAssemblies { get; set; }

    public IEnumerable<string>? AdditionalJsonFiles { get; set; }

    internal string FallbackCulture { get; set; } = "en";

    internal bool EnableFallbackCulture { get; set; } = true;

    public bool IgnoreLocalizerMissing { get; set; }

    public bool ReloadOnChange { get; set; }

    public JsonLocalizationOptions()
    {
        ResourcesPath = "Locales";
    }
}
