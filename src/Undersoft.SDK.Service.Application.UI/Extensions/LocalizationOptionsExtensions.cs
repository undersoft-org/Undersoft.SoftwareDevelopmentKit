using Undersoft.SDK.Service.Application.Localization.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System.Reflection;

namespace Undersoft.SDK.Service.Application.Components;

internal static class LocalizationOptionsExtensions
{
    public static IEnumerable<IConfigurationSection> GetJsonStringFromAssembly(this JsonLocalizationOptions option, Assembly assembly, string cultureName)
    {
        var langHandlers = option.GetJsonHanlders(assembly, cultureName).ToList();

        var builder = new ConfigurationBuilder();

        foreach (var h in langHandlers)
        {
            builder.AddJsonStream(h);
        }

        if (option.AdditionalJsonFiles != null)
        {
            var files = option.AdditionalJsonFiles.Where(f =>
            {
                var fileName = Path.GetFileNameWithoutExtension(f);
                return fileName.Equals(cultureName, StringComparison.OrdinalIgnoreCase);
            });
            foreach (var file in files)
            {
                builder.AddJsonFile(file, true, option.ReloadOnChange);
            }
        }

        var config = builder.Build();

        foreach (var h in langHandlers)
        {
            h.Dispose();
        }
        return config.GetChildren();
    }

    private static IEnumerable<Stream> GetJsonHanlders(this JsonLocalizationOptions option, Assembly assembly, string cultureName)
    {
        var assemblies = new List<Assembly>()
        {
            assembly
        };
        if (option.AdditionalJsonAssemblies != null)
        {
            assemblies.AddRange(option.AdditionalJsonAssemblies);
        }
        return assemblies.SelectMany(i => option.GetResourceStream(i, cultureName));
    }

    private static List<Stream> GetResourceStream(this JsonLocalizationOptions option, Assembly assembly, string cultureName)
    {
        var ret = new List<Stream>();

        if (option.EnableFallbackCulture)
        {
            AddStream(option.FallbackCulture);
        }

        var parentName = GetParentCultureName(cultureName).Value;
        if (!string.IsNullOrEmpty(parentName) && !EqualFallbackCulture(parentName))
        {
            AddStream(parentName);
        }

        if (!EqualFallbackCulture(cultureName))
        {
            AddStream(cultureName);
        }

        return ret;

        void AddStream(string name)
        {
            var json = $"{assembly.GetName().Name}.{option.ResourcesPath}.{name}.json";
            var stream = assembly.GetManifestResourceStream(json);
            if (stream != null)
            {
                ret.Add(stream);
            }
        }

        bool EqualFallbackCulture(string name) => option.EnableFallbackCulture && option.FallbackCulture == name;

        StringSegment GetParentCultureName(StringSegment cultureInfoName)
        {
            var ret = new StringSegment();
            var index = cultureInfoName.IndexOf('-');
            if (index > 0)
            {
                ret = cultureInfoName.Subsegment(0, index);
            }
            return ret;
        }
    }
}
