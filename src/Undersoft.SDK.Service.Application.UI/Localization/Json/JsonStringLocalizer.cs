using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Undersoft.SDK.Service.Application.Localization.Json;

internal class JsonStringLocalizer : ResourceManagerStringLocalizer
{
    private Assembly Assembly { get; set; }

    private string TypeName { get; set; }

    private bool IgnoreLocalizerMissing { get; set; }

    private ILogger Logger { get; set; }

    private ConcurrentDictionary<string, object?> MissingLocalizerCache { get; } = new();

    public JsonStringLocalizer(
        Assembly assembly,
        string typeName,
        string baseName,
        bool ignoreLocalizerMissing,
        ILogger logger,
        IResourceNamesCache resourceNamesCache) : base(new ResourceManager(baseName, assembly), assembly, baseName, resourceNamesCache, logger)
    {
        Assembly = assembly;
        TypeName = typeName;
        IgnoreLocalizerMissing = ignoreLocalizerMissing;
        Logger = logger;
    }

    public override LocalizedString this[string name]
    {
        get
        {
            var value = GetStringSafely(name);
            return new LocalizedString(name, value ?? name, resourceNotFound: value == null, searchedLocation: TypeName);
        }
    }

    public override LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var value = SafeFormat();
            return new LocalizedString(name, value ?? name, resourceNotFound: value == null, searchedLocation: TypeName);

            string? SafeFormat()
            {
                string? ret = null;
                try
                {
                    var format = GetStringSafely(name);
                    ret = string.Format(CultureInfo.CurrentCulture, format ?? name, arguments);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "{JsonStringLocalizerName} searched for '{ServiceName}' in '{TypeName}' with culture '{CultureName}' throw exception.", nameof(JsonStringLocalizer), name, TypeName, CultureInfo.CurrentUICulture.Name);
                }
                return ret;
            }
        }
    }

    private string? GetStringSafely(string name)
    {
        return GetStringFromService(name)
            ?? GetStringSafely(name, null)
            ?? GetStringSafelyFromJson(name);

        string? GetStringFromService(string name)
        {
            string? ret = null;
            var localizer = Utility.GetStringLocalizerFromService(Assembly, TypeName);
            if (localizer != null)
            {
                ret = GetLocalizerValueFromCache(localizer, name);
            }
            return ret;
        }

        string? GetStringSafelyFromJson(string name)
        {
            var localizerStrings = CacheManager.GetAllStringsByTypeName(Assembly, TypeName);
            return GetValueFromCache(localizerStrings, name);
        }
    }

    private string? GetValueFromCache(IEnumerable<LocalizedString>? localizerStrings, string name)
    {
        string? ret = null;
        var cultureName = CultureInfo.CurrentUICulture.Name;
        var cacheKey = $"{nameof(GetValueFromCache)}&name={name}&{Assembly.GetName().Name}&type={TypeName}&culture={cultureName}";
        if (!MissingLocalizerCache.ContainsKey(cacheKey))
        {
            var l = GetLocalizedString();
            if (l is { ResourceNotFound: false })
            {
                ret = l.Value;
            }
            else
            {
                LogSearchedLocation(name);
                MissingLocalizerCache.TryAdd(cacheKey, null);
            }
        }
        return ret;

        LocalizedString? GetLocalizedString()
        {
            LocalizedString? localizer = null;
            if (localizerStrings != null)
            {
                localizer = localizerStrings.FirstOrDefault(i => i.Name == name);
            }
            return localizer ?? CacheManager.GetAllStringsFromResolve().FirstOrDefault(i => i.Name == name);
        }
    }

    private string? GetLocalizerValueFromCache(IStringLocalizer localizer, string name)
    {
        string? ret = null;
        var cultureName = CultureInfo.CurrentUICulture.Name;
        var cacheKey = $"{nameof(GetLocalizerValueFromCache)}&name={name}&{Assembly.GetName().Name}&type={TypeName}&culture={cultureName}";
        if (!MissingLocalizerCache.ContainsKey(cacheKey))
        {
            var l = localizer[name];
            if (l.ResourceNotFound)
            {
                LogSearchedLocation(name);
                MissingLocalizerCache.TryAdd(cacheKey, null);
            }
            else
            {
                ret = l.Value;
            }
        }
        return ret;
    }

    private void LogSearchedLocation(string name)
    {
        if (!IgnoreLocalizerMissing)
        {
            Logger.LogInformation("{JsonStringLocalizerName} searched for '{ServiceName}' in '{TypeName}' with culture '{CultureName}' not found.", nameof(JsonStringLocalizer), name, TypeName, CultureInfo.CurrentUICulture.Name);
        }
    }

    public override IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        var ret = GetAllStringsFromService(includeParentCultures)
            ?? GetAllStringsFromBase(includeParentCultures)
            ?? GetAllStringsFromJson(includeParentCultures);

        return ret;

        IEnumerable<LocalizedString>? GetAllStringsFromService(bool includeParentCultures)
        {
            IEnumerable<LocalizedString>? ret = null;
            var localizer = Utility.GetStringLocalizerFromService(Assembly, TypeName);
            if (localizer != null)
            {
                ret = localizer.GetAllStrings(includeParentCultures);
            }
            return ret;
        }

        IEnumerable<LocalizedString>? GetAllStringsFromBase(bool includeParentCultures)
        {
            IEnumerable<LocalizedString>? ret = base.GetAllStrings(includeParentCultures);
            try
            {
                CheckMissing();
            }
            catch (MissingManifestResourceException)
            {
                ret = null;
            }
            return ret;

            [ExcludeFromCodeCoverage]
            void CheckMissing() => _ = ret.Any();
        }

        IEnumerable<LocalizedString> GetAllStringsFromJson(bool includeParentCultures) => CacheManager.GetAllStringsByTypeName(Assembly, TypeName)
            ?? CacheManager.GetAllStringsFromResolve(includeParentCultures);
    }
}
