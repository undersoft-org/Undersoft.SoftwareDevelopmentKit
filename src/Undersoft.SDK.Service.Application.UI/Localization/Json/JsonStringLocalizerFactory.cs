using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Undersoft.SDK.Service.Application.Components;

namespace Undersoft.SDK.Service.Application.Localization.Json;

internal class JsonStringLocalizerFactory : ResourceManagerStringLocalizerFactory
{
    private ILoggerFactory LoggerFactory { get; set; }

    [NotNull]
    private string? TypeName { get; set; }

    private bool IgnoreLocalizerMissing { get; set; }

    public JsonStringLocalizerFactory(
        ICacheManager cacheManager,
        IOptionsMonitor<ApplicationOptions> options,
        IOptions<JsonLocalizationOptions> jsonLocalizationOptions,
        IOptions<LocalizationOptions> localizationOptions,
        ILoggerFactory loggerFactory) : base(localizationOptions, loggerFactory)
    {
        cacheManager.SetStartTime();

        jsonLocalizationOptions.Value.FallbackCulture = options.CurrentValue.FallbackCulture;
        jsonLocalizationOptions.Value.EnableFallbackCulture = options.CurrentValue.EnableFallbackCulture;
        if (options.CurrentValue.IgnoreLocalizerMissing.HasValue)
        {
            jsonLocalizationOptions.Value.IgnoreLocalizerMissing = options.CurrentValue.IgnoreLocalizerMissing.Value;
        }
        IgnoreLocalizerMissing = jsonLocalizationOptions.Value.IgnoreLocalizerMissing;
        LoggerFactory = loggerFactory;
        options.OnChange(OnChange);

        [ExcludeFromCodeCoverage]
        void OnChange(ApplicationOptions op)
        {
            jsonLocalizationOptions.Value.EnableFallbackCulture = op.EnableFallbackCulture;
            jsonLocalizationOptions.Value.FallbackCulture = op.FallbackCulture;
            if (op.IgnoreLocalizerMissing.HasValue)
            {
                jsonLocalizationOptions.Value.IgnoreLocalizerMissing = op.IgnoreLocalizerMissing.Value;
                IgnoreLocalizerMissing = op.IgnoreLocalizerMissing.Value;
            }
        }
    }

    protected override string GetResourcePrefix(TypeInfo typeInfo)
    {
        var typeName = typeInfo.FullName;
        if (string.IsNullOrEmpty(typeName))
        {
            throw new InvalidOperationException($"{nameof(typeInfo)} full name is null or String.Empty.");
        }

        if (typeInfo.IsGenericType)
        {
            var index = typeName.IndexOf('`');
            typeName = typeName[..index];
        }
        TypeName = typeName;

        return base.GetResourcePrefix(typeInfo);
    }

    protected override string GetResourcePrefix(string baseResourceName, string baseNamespace)
    {
        var resourcePrefix = base.GetResourcePrefix(baseResourceName, baseNamespace);
        TypeName = $"{baseNamespace}.{baseResourceName}";

        return resourcePrefix;
    }

    private IResourceNamesCache ResourceNamesCache { get; } = new ResourceNamesCache();

    protected override ResourceManagerStringLocalizer CreateResourceManagerStringLocalizer(Assembly assembly, string baseName) => new JsonStringLocalizer(
            assembly,
            TypeName,
            baseName,
            IgnoreLocalizerMissing,
            LoggerFactory.CreateLogger<JsonStringLocalizer>(),
            ResourceNamesCache);
}
