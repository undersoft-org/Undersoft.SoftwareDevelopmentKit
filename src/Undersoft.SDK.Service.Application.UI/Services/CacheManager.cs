using Undersoft.SDK.Service.Application.Localization;
using Undersoft.SDK.Service.Application.Localization.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace Undersoft.SDK.Service.Application.Components;

internal class CacheManager : ICacheManager
{
    private IMemoryCache Cache { get; set; }

    private IServiceProvider Provider { get; set; }

    [NotNull]
    private static CacheManager? Instance { get; set; }

    public CacheManager(
        IServiceProvider provider,
        IMemoryCache memoryCache)
    {
        Provider = provider;
        Cache = memoryCache;
        Instance = this;
    }

    public TItem GetOrCreate<TItem>(object key, Func<ICacheEntry, TItem> factory) => Cache.GetOrCreate(key, entry =>
    {
#if DEBUG
        entry.SlidingExpiration = TimeSpan.FromSeconds(500000);
#endif

        if (key is not string)
        {
            entry.SetSlidingExpiration(TimeSpan.FromMinutes(5));
        }
        return factory(entry);
    })!;

    public Task<TItem> GetOrCreateAsync<TItem>(object key, Func<ICacheEntry, Task<TItem>> factory) => Cache.GetOrCreateAsync(key, async entry =>
    {
#if DEBUG
        entry.SlidingExpiration = TimeSpan.FromSeconds(5);
#endif

        if (key is not string)
        {
            entry.SetSlidingExpiration(TimeSpan.FromMinutes(5));
        }
        return await factory(entry);
    })!;

    public void Clear(string? key)
    {
        if (!string.IsNullOrEmpty(key))
        {
            Cache.Remove(key);
        }
        else if (Cache is MemoryCache c)
        {
            c.Compact(100);
        }
    }

    public void SetStartTime()
    {
        GetOrCreate("ServiceApplication_StartTime", entry => DateTimeOffset.Now);
    }

    public DateTimeOffset GetStartTime()
    {
        var ret = DateTimeOffset.MinValue;
        if (Cache.TryGetValue("ServiceApplication_StartTime", out var v) && v is DateTimeOffset d)
        {
            ret = d;
        }
        return ret;
    }

    #region Count
    public static int ElementCount(object? value)
    {
        var ret = 0;
        if (value != null)
        {
            var type = value.GetType();
            var cacheKey = $"Lambda-Count-{type.FullName}";
            var invoker = Instance.GetOrCreate(cacheKey, entry =>
            {
                entry.SetDynamicAssemblyPolicy(type);
                return LambdaExtensions.CountLambda(type).Compile();
            });
            if (invoker != null)
            {
                ret = invoker(value);
            }
        }
        return ret;
    }
    #endregion

    #region Localizer
    public static IStringLocalizer? CreateLocalizerByType(Type resourceSource) => resourceSource.Assembly.IsDynamic
        ? null
        : Instance.Provider.GetRequiredService<IStringLocalizerFactory>().Create(resourceSource);

    public static JsonLocalizationOptions GetJsonLocalizationOption()
    {
        var localizationOptions = Instance.Provider.GetRequiredService<IOptions<JsonLocalizationOptions>>();
        return localizationOptions.Value;
    }

    public static IStringLocalizer? GetStringLocalizerFromService(Assembly assembly, string typeName) => assembly.IsDynamic
        ? null
        : Instance.GetOrCreate($"{nameof(GetStringLocalizerFromService)}-{CultureInfo.CurrentUICulture.Name}-{assembly.GetName().Name}-{typeName}", entry =>
    {
        IStringLocalizer? ret = null;
        var factories = Instance.Provider.GetServices<IStringLocalizerFactory>();
        if (factories != null)
        {
            var factory = factories.LastOrDefault(a => a is not JsonStringLocalizerFactory);
            if (factory != null)
            {
                var type = assembly.GetType(typeName);
                if (type != null)
                {
                    ret = factory.Create(type);
                }
            }
        }
        return ret;
    });

    public static IEnumerable<LocalizedString>? GetAllStringsByTypeName(Assembly assembly, string typeName) => GetJsonStringByTypeName(GetJsonLocalizationOption(), assembly, typeName, CultureInfo.CurrentUICulture.Name);

    public static IEnumerable<LocalizedString>? GetJsonStringByTypeName(JsonLocalizationOptions option, Assembly assembly, string typeName, string? cultureName = null, bool forceLoad = false)
    {
        return assembly.IsDynamic ? null : GetJsonStringByTypeName();

        IEnumerable<LocalizedString>? GetJsonStringByTypeName()
        {
            cultureName ??= CultureInfo.CurrentUICulture.Name;
            var key = $"{nameof(GetJsonStringByTypeName)}-{assembly.GetName().Name}-{cultureName}";
            var typeKey = $"{key}-{typeName}";
            if (forceLoad)
            {
                Instance.Cache.Remove(key);
                Instance.Cache.Remove(typeKey);
            }
            return Instance.GetOrCreate(typeKey, entry =>
            {
                var sections = Instance.GetOrCreate(key, entry => option.GetJsonStringFromAssembly(assembly, cultureName));
                return sections.FirstOrDefault(kv => typeName.Equals(kv.Key, StringComparison.OrdinalIgnoreCase))?
                    .GetChildren()
                    .SelectMany(kv => new[] { new LocalizedString(kv.Key, kv.Value!, false, typeName) });
            });
        }
    }

    public static IEnumerable<LocalizedString> GetAllStringsFromResolve(bool includeParentCultures = true) => Instance.GetOrCreate($"{nameof(GetAllStringsFromResolve)}-{CultureInfo.CurrentUICulture.Name}", entry => Instance.Provider.GetRequiredService<ILocalizationResolve>().GetAllStringsByCulture(includeParentCultures));
    #endregion

    #region DisplayName
    public static string GetDisplayName(Type modelType, string fieldName)
    {
        var cacheKey = $"{nameof(GetDisplayName)}-{CultureInfo.CurrentUICulture.Name}-{modelType.FullName}-{fieldName}";
        var displayName = Instance.GetOrCreate(cacheKey, entry =>
        {
            string? dn = null;
            var localizer = modelType.Assembly.IsDynamic ? null : CreateLocalizerByType(modelType);
            var stringLocalizer = localizer?[fieldName];
            if (stringLocalizer is { ResourceNotFound: false })
            {
                dn = stringLocalizer.Value;
            }
            else if (modelType.IsEnum)
            {
                var info = modelType.GetFieldByName(fieldName);
                if (info != null)
                {
                    dn = FindDisplayAttribute(info);
                }
            }
            else if (TryGetProperty(modelType, fieldName, out var propertyInfo))
            {
                dn = FindDisplayAttribute(propertyInfo);
            }

            entry.SetDynamicAssemblyPolicy(modelType);

            return dn;
        });

        return displayName ?? fieldName;

        string? FindDisplayAttribute(MemberInfo memberInfo)
        {
            var dn = memberInfo.GetCustomAttribute<DisplayAttribute>(true)?.Name
                ?? memberInfo.GetCustomAttribute<DisplayNameAttribute>(true)?.DisplayName
                ?? memberInfo.GetCustomAttribute<DescriptionAttribute>(true)?.Description;

            if (!modelType.Assembly.IsDynamic && !string.IsNullOrEmpty(dn))
            {
                dn = GetLocalizerValueFromResourceManager(dn);
            }
            return dn;
        }
    }

    public static List<SelectedItem> GetNullableBoolItems(Type modelType, string fieldName)
    {
        var cacheKey = $"{nameof(GetNullableBoolItems)}-{CultureInfo.CurrentUICulture.Name}-{modelType.FullName}-{fieldName}";
        return Instance.GetOrCreate(cacheKey, entry =>
        {
            var items = new List<SelectedItem>();
            var localizer = modelType.Assembly.IsDynamic ? null : CreateLocalizerByType(modelType);
            IStringLocalizer? localizerAttr = null;
            items.Add(new SelectedItem("", FindDisplayText(nameof(NullableBoolItemsAttribute.NullValueDisplayText), attr => attr.NullValueDisplayText)));
            items.Add(new SelectedItem("True", FindDisplayText(nameof(NullableBoolItemsAttribute.TrueValueDisplayText), attr => attr.TrueValueDisplayText)));
            items.Add(new SelectedItem("False", FindDisplayText(nameof(NullableBoolItemsAttribute.FalseValueDisplayText), attr => attr.FalseValueDisplayText)));
            return items;

            string FindDisplayText(string itemName, Func<NullableBoolItemsAttribute, string?> callback)
            {
                string? dn = null;

                var stringLocalizer = localizer?[$"{fieldName}-{itemName}"];
                if (stringLocalizer is { ResourceNotFound: false })
                {
                    dn = stringLocalizer.Value;
                }
                else if (TryGetProperty(modelType, fieldName, out var propertyInfo))
                {
                    var attr = propertyInfo.GetCustomAttribute<NullableBoolItemsAttribute>(true);
                    if (attr != null && !modelType.Assembly.IsDynamic)
                    {
                        dn = callback(attr);
                    }
                }

                return dn ?? FindDisplayTextByItemName(itemName);
            }

            string FindDisplayTextByItemName(string itemName)
            {
                localizerAttr ??= CreateLocalizerByType(typeof(NullableBoolItemsAttribute));
                var stringLocalizer = localizerAttr![itemName];
                return stringLocalizer.Value;
            }
        });
    }

    private static string? GetLocalizerValueFromResourceManager(string key)
    {
        string? dn = null;
        var options = GetJsonLocalizationOption();
        if (options.ResourceManagerStringLocalizerType != null)
        {
            var localizer = CreateLocalizerByType(options.ResourceManagerStringLocalizerType);
            dn = GetValueByKey(localizer);
        }
        return dn ?? key;

        [ExcludeFromCodeCoverage]
        string? GetValueByKey(IStringLocalizer? l)
        {
            string? val = null;
            var stringLocalizer = l?[key];
            if (stringLocalizer is { ResourceNotFound: false })
            {
                val = stringLocalizer.Value;
            }
            return val;
        }
    }
    #endregion

    #region Placeholder
    public static string? GetPlaceholder(Type modelType, string fieldName)
    {
        var cacheKey = $"{nameof(GetPlaceholder)}-{CultureInfo.CurrentUICulture.Name}-{modelType.FullName}-{fieldName}";
        return Instance.GetOrCreate(cacheKey, entry =>
        {
            string? ret = null;
            var localizer = CreateLocalizerByType(modelType);
            if (localizer != null)
            {
                var stringLocalizer = localizer[$"{fieldName}.PlaceHolder"];
                if (!stringLocalizer.ResourceNotFound)
                {
                    ret = stringLocalizer.Value;
                }
                else if (TryGetProperty(modelType, fieldName, out var propertyInfo))
                {
                    var placeHolderAttribute = propertyInfo.GetCustomAttribute<PlaceHolderAttribute>(true);
                    if (placeHolderAttribute != null)
                    {
                        ret = placeHolderAttribute.Text;
                    }
                }

                entry.SetDynamicAssemblyPolicy(modelType);
            }
            return ret;
        });
    }
    #endregion

    #region Lambda Property
    public static bool TryGetProperty(Type modelType, string fieldName, [NotNullWhen(true)] out PropertyInfo? propertyInfo)
    {
        var cacheKey = $"{nameof(TryGetProperty)}-{modelType.FullName}-{fieldName}";
        propertyInfo = Instance.GetOrCreate(cacheKey, entry =>
        {
            var props = modelType.GetRuntimeProperties().AsEnumerable();

            var metadataType = modelType.GetCustomAttribute<MetadataTypeAttribute>(false);
            if (metadataType != null)
            {
                props = props.Concat(metadataType.MetadataClassType.GetRuntimeProperties());
            }

            var pi = props.FirstOrDefault(p => p.Name == fieldName);

            entry.SetDynamicAssemblyPolicy(modelType);

            return pi;
        });
        return propertyInfo != null;
    }

    public static TResult GetPropertyValue<TModel, TResult>(TModel model, string fieldName)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        return (model is IDynamicColumnsObject d)
            ? (TResult)d.GetValue(fieldName)!
            : GetValue();

        TResult GetValue()
        {
            var type = model.GetType();
            var cacheKey = ($"Lambda-Get-{type.FullName}", typeof(TModel), fieldName, typeof(TResult));
            var invoker = Instance.GetOrCreate(cacheKey, entry =>
            {
                entry.SetDynamicAssemblyPolicy(type);
                return LambdaExtensions.GetPropertyValueLambda<TModel, TResult>(model, fieldName).Compile();
            })!;
            return invoker(model);
        }
    }

    public static void SetPropertyValue<TModel, TValue>(TModel model, string fieldName, TValue value)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        if (model is IDynamicColumnsObject d)
        {
            d.SetValue(fieldName, value);
        }
        else
        {
            SetValue();
        }

        void SetValue()
        {
            var type = model.GetType();
            var cacheKey = ($"Lambda-EntitySet-{type.FullName}", typeof(TModel), fieldName, typeof(TValue));
            var invoker = Instance.GetOrCreate(cacheKey, entry =>
            {
                entry.SetDynamicAssemblyPolicy(type);
                return LambdaExtensions.SetPropertyValueLambda<TModel, TValue>(model, fieldName).Compile();
            })!;
            invoker(model, value);
        }
    }

    public static TValue? GetKeyValue<TModel, TValue>(TModel model, Type? customAttribute = null)
    {
        var ret = default(TValue);
        if (model != null)
        {
            var type = model.GetType();
            var cacheKey = ($"Lambda-GetKeyValue-{type.FullName}-{customAttribute?.FullName}", typeof(TModel));
            var invoker = Instance.GetOrCreate(cacheKey, entry =>
            {
                entry.SetDynamicAssemblyPolicy(type);

                return LambdaExtensions.GetKeyValue<TModel, TValue>(customAttribute).Compile();
            })!;
            ret = invoker(model);
        }
        return ret;
    }
    #endregion

    #region Lambda Sort
    public static Func<IEnumerable<T>, string, SortOrder, IEnumerable<T>> GetSortFunc<T>()
    {
        var cacheKey = $"Lambda-{nameof(LambdaExtensions.GetSortLambda)}-{typeof(T).FullName}";
        return Instance.GetOrCreate(cacheKey, entry =>
        {
            entry.SetDynamicAssemblyPolicy(typeof(T));
            return LambdaExtensions.GetSortLambda<T>().Compile();
        })!;
    }

    public static Func<IEnumerable<T>, List<string>, IEnumerable<T>> GetSortListFunc<T>()
    {
        var cacheKey = $"Lambda-{nameof(LambdaExtensions.GetSortListLambda)}-{typeof(T).FullName}";
        return Instance.GetOrCreate(cacheKey, entry =>
        {
            entry.SetDynamicAssemblyPolicy(typeof(T));
            return LambdaExtensions.GetSortListLambda<T>().Compile();
        })!;
    }
    #endregion

    #region Lambda ConvertTo
    public static Func<object, IEnumerable<string?>> CreateConverterInvoker(Type type)
    {
        var cacheKey = $"Lambda-{nameof(CreateConverterInvoker)}-{type.FullName}";
        return Instance.GetOrCreate(cacheKey, entry =>
        {
            var method = typeof(CacheManager)
                .GetMethod(nameof(ConvertToString), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(type);

            var para_exp = Expression.Parameter(typeof(object));
            var convert = Expression.Convert(para_exp, typeof(List<>).MakeGenericType(type));
            var body = Expression.Call(method, convert);

            entry.SetDynamicAssemblyPolicy(type);
            return Expression.Lambda<Func<object, IEnumerable<string?>>>(body, para_exp).Compile();
        })!;
    }

    private static IEnumerable<string?> ConvertToString<TSource>(List<TSource> source) => source is List<SelectedItem> list
        ? list.Select(i => i.Value)
        : source.Select(i => i?.ToString());
    #endregion

    #region OnValueChanged Lambda
    public static Func<TModel, ITableColumn, Func<TModel, ITableColumn, object?, Task>, object> GetOnValueChangedInvoke<TModel>(Type fieldType)
    {
        var cacheKey = $"Lambda-{nameof(GetOnValueChangedInvoke)}-{typeof(TModel).FullName}-{fieldType.FullName}";
        return Instance.GetOrCreate(cacheKey, entry =>
        {
            entry.SetDynamicAssemblyPolicy(fieldType);
            return Utility.CreateOnValueChanged<TModel>(fieldType).Compile();
        })!;
    }
    #endregion

    #region Format
    public static Func<object, string, IFormatProvider?, string> GetFormatInvoker(Type type)
    {
        var cacheKey = $"{nameof(GetFormatInvoker)}-{nameof(GetFormatLambda)}-{type.FullName}";
        return Instance.GetOrCreate(cacheKey, entry =>
        {
            entry.SetDynamicAssemblyPolicy(type);
            return GetFormatLambda(type).Compile();
        })!;

        static Expression<Func<object, string, IFormatProvider?, string>> GetFormatLambda(Type type)
        {
            var exp_p1 = Expression.Parameter(typeof(object));
            var exp_p2 = Expression.Parameter(typeof(string));
            var exp_p3 = Expression.Parameter(typeof(IFormatProvider));
            Expression? body = null;
            if (type.IsAssignableTo(typeof(IFormattable)))
            {
                var mi = type.GetMethod("ToString", new Type[] { typeof(string), typeof(IFormatProvider) });
                if (mi != null)
                {
                    body = Expression.Call(Expression.Convert(exp_p1, type), mi, exp_p2, exp_p3);
                }
            }
            else
            {
                var mi = type.GetMethod("ToString", Array.Empty<Type>());
                if (mi != null)
                {
                    body = Expression.Call(Expression.Convert(exp_p1, type), mi);
                }
            }
            return BuildExpression();

            [ExcludeFromCodeCoverage]
            Expression<Func<object, string, IFormatProvider?, string>> BuildExpression() => body == null
                ? (s, f, provider) => s.ToString() ?? ""
                : Expression.Lambda<Func<object, string, IFormatProvider?, string>>(body, exp_p1, exp_p2, exp_p3);
        }
    }

    public static Func<object, IFormatProvider?, string> GetFormatProviderInvoker(Type type)
    {
        var cacheKey = $"{nameof(GetFormatProviderInvoker)}-{nameof(GetFormatProviderLambda)}-{type.FullName}";
        return Instance.GetOrCreate(cacheKey, entry =>
        {
            entry.SetDynamicAssemblyPolicy(type);
            return GetFormatProviderLambda(type).Compile();
        })!;

        static Expression<Func<object, IFormatProvider?, string>> GetFormatProviderLambda(Type type)
        {
            var exp_p1 = Expression.Parameter(typeof(object));
            var exp_p2 = Expression.Parameter(typeof(IFormatProvider));
            Expression? body;

            var mi = type.GetMethod("ToString", new Type[] { typeof(IFormatProvider) });
            if (mi != null)
            {
                body = Expression.Call(Expression.Convert(exp_p1, type), mi, exp_p2);
            }
            else
            {
                mi = type.GetMethod("ToString");
                body = Expression.Call(Expression.Convert(exp_p1, type), mi!);
            }
            return Expression.Lambda<Func<object, IFormatProvider?, string>>(body, exp_p1, exp_p2);
        }
    }
    #endregion
}
