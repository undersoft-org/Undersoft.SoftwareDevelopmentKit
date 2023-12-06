using Undersoft.SDK.Series;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Undersoft.SDK.Service.Data.Cache;

using Undersoft.SDK;
using Uniques;

public interface IDataCache : ITypedSeries<IOrigin>
{
    ITypedSeries<IOrigin> CacheSet<T>() where T : IOrigin;

    T Lookup<T>(T item) where T : IOrigin;

    ISeries<IOrigin> Lookup<T>(Func<ITypedSeries<IOrigin>, ISeries<IOrigin>> selector) where T : IOrigin;

    T Lookup<T>(object keys) where T : IOrigin;

    ISeries<IOrigin> Lookup<T>(Tuple<string, object> valueNamePair) where T : IOrigin;

    T Lookup<T>(T item, params string[] propertyNames) where T : IOrigin;

    T[] Lookup<T>(Func<ISeries<IOrigin>, IOrigin> key, params Func<ITypedSeries<IOrigin>, ISeries<IOrigin>>[] selectors)
        where T : IOrigin;

    T[] Lookup<T>(object key, params Tuple<string, object>[] valueNamePairs) where T : IOrigin;

    ISeries<IOrigin> Lookup<T>(object key, string propertyNames) where T : IOrigin;

    IEnumerable<T> Memorize<T>(IEnumerable<T> items) where T : IOrigin;

    T Memorize<T>(T item) where T : IOrigin;

    T Memorize<T>(T item, params string[] names) where T : IOrigin;

    Task<T> MemorizeAsync<T>(T item) where T : IOrigin;

    Task<T> MemorizeAsync<T>(T item, params string[] names) where T : IOrigin;

    ITypedSeries<IOrigin> Catalog { get; }
}