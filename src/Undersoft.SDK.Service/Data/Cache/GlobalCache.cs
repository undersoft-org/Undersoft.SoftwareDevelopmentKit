using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Undersoft.SDK.Service.Data.Cache;

namespace Undersoft.SDK.Service.Data.Cache;

using Entity;
using Undersoft.SDK;
using Undersoft.SDK.Service.Data.Object;
using Uniques;

public static class GlobalCache
{
    static GlobalCache()
    {
        IConfigurationSection cacheconfig = ServiceManager
            .GetConfiguration()
            .DataCacheLifeTime();
        Catalog = new DataCache(
            TimeSpan.FromMinutes(
                cacheconfig.GetValue<uint>("Minutes") + cacheconfig.GetValue<uint>("Hours") * 60
            ),
            null,
            513
        );
    }

    public static async Task<T> Lookup<T>(object keys) where T : IOrigin
    {
        return await Task.Run((Func<T>)(() =>
        {
            if (Catalog.TryGet(keys, (long)DataObjectExtensions.GetDataTypeId(typeof(T)), out IOrigin output))
                return (T)output;
            return default;
        }));
    }

    public static T ToCache<T>(this T item) where T : IOrigin
    {
        return Catalog.Memorize(item);
    }

    public static T ToCache<T>(this T item, params string[] names) where T : IOrigin
    {
        return Catalog.Memorize(item, names);
    }

    public static async Task<T> ToCacheAsync<T>(this T item) where T : IOrigin
    {
        return await Task.Run(() => item.ToCache()).ConfigureAwait(false);
    }

    public static async Task<T> ToCacheAsync<T>(this T item, params string[] names)
        where T : IOrigin
    {
        return await Task.Run(() => item.ToCache(names)).ConfigureAwait(false);
    }

    public static IDataCache Catalog { get; }
}
