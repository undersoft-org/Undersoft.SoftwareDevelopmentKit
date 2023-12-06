using System.Collections.Generic;
using Undersoft.SDK.Instant;
using System.Runtime.CompilerServices;
using System.Threading;
using Undersoft.SDK.Uniques;

namespace Undersoft.SDK.Series;

using Base;
using Invoking;
using Instant.Proxies;
using Instant.Rubrics;
using Undersoft.SDK;

public class TypedCache<V> : BaseTypedRegistry<V> where V : IOrigin
{
    private readonly Catalog<Timer> timers = new Catalog<Timer>();

    private TimeSpan duration;
    private IInvoker callback;

    private void setupExpiration(TimeSpan? lifetime, IInvoker callback)
    {
        duration = (lifetime != null) ? lifetime.Value : TimeSpan.FromMinutes(15);
        if (callback != null)
            this.callback = callback;
    }

    public TypedCache(
        IEnumerable<IOrigin<V>> collection,
        TimeSpan? lifeTime = null,
        IInvoker callback = null,
        int capacity = 17
    ) : base(collection, capacity)
    {
        setupExpiration(lifeTime, callback);
    }

    public TypedCache(
        IEnumerable<V> collection,
        TimeSpan? lifeTime = null,
        IInvoker callback = null,
        int capacity = 17
    ) : base(collection, capacity)
    {
        setupExpiration(lifeTime, callback);
    }

    public TypedCache(
        IList<IOrigin<V>> collection,
        TimeSpan? lifeTime = null,
        IInvoker callback = null,
        int capacity = 17
    ) : base(collection, capacity)
    {
        setupExpiration(lifeTime, callback);
    }

    public TypedCache(
        IList<V> collection,
        TimeSpan? lifeTime = null,
        IInvoker callback = null,
        int capacity = 17
    ) : base(collection, capacity)
    {
        setupExpiration(lifeTime, callback);
    }

    public TypedCache(TimeSpan? lifeTime = null, IInvoker callback = null, int capacity = 17)
        : base(capacity)
    {
        setupExpiration(lifeTime, callback);
    }

    public override ISeriesItem<V> EmptyItem()
    {
        return new CacheSeriesItem<V>();
    }

    public override ISeriesItem<V>[] EmptyTable(int size)
    {
        return new CacheSeriesItem<V>[size];
    }

    public override ISeriesItem<V>[] EmptyVector(int size)
    {
        return new CacheSeriesItem<V>[size];
    }

    public override ISeriesItem<V> NewItem(ISeriesItem<V> item)
    {
        return new CacheSeriesItem<V>(item, duration, callback);
    }

    public override ISeriesItem<V> NewItem(object key, V value)
    {
        return new CacheSeriesItem<V>(key, value, duration, callback);
    }

    public override ISeriesItem<V> NewItem(long key, V value)
    {
        return new CacheSeriesItem<V>(key, value, duration, callback);
    }

    public override ISeriesItem<V> NewItem(V value)
    {
        return new CacheSeriesItem<V>(value, duration, callback);
    }

    protected virtual ITypedSeries<IOrigin> cache { get; set; }

    protected virtual T InnerMemorize<T>(T item) where T : IOrigin
    {
        int group = GetValidTypeKey(typeof(T));
        if (!cache.TryGet(group, out IOrigin catalog))
        {
            ProxyCreator sleeve = ProxyFactory.GetCreator(GetValidType(typeof(T)), group);
            sleeve.Create();

            IRubrics keyrubrics = sleeve.Rubrics.KeyRubrics;

            IProxy isleeve = item.ToProxy();

            catalog = new TypedRegistry<IOrigin>();

            foreach (MemberRubric keyRubric in keyrubrics)
            {
                Registry<IOrigin> subcatalog = new Registry<IOrigin>();

                subcatalog.Add(item);

                ((ITypedSeries<IOrigin>)catalog).Put(
                    isleeve[keyRubric.RubricId],
                    keyRubric.RubricName.UniqueKey32(),
                    subcatalog);
            }

            cache.Add(group, catalog);

            cache.Add(item);

            return item;
        }

        if (!cache.ContainsKey(item))
        {
            ITypedSeries<IOrigin> _catalog = (ITypedSeries<IOrigin>)catalog;

            IProxy isleeve = item.ToProxy();

            foreach (MemberRubric keyRubric in isleeve.Rubrics.KeyRubrics)
            {
                if (!_catalog.TryGet(
                    isleeve[keyRubric.RubricId],
                    keyRubric.RubricName.UniqueKey32(),
                    out IOrigin outcatalog))
                {
                    outcatalog = new Registry<IOrigin>();

                    ((ISeries<IOrigin>)outcatalog).Put(item);

                    _catalog.Put(isleeve[keyRubric.RubricId], keyRubric.RubricName.UniqueKey32(), outcatalog);
                }
                else
                {
                    ((ISeries<IOrigin>)outcatalog).Put(item);
                }
            }
            cache.Add(item);
        }

        return item;
    }

    protected virtual T InnerMemorize<T>(T item, params string[] names) where T : IOrigin
    {
        Memorize(item);

        IProxy sleeve = item.ToProxy();

        MemberRubric[] keyrubrics = sleeve.Rubrics.Where(p => names.Contains(p.RubricName)).ToArray();

        ITypedSeries<IOrigin> _catalog = (ITypedSeries<IOrigin>)cache.Get((ulong)item.TypeId);

        foreach (MemberRubric keyRubric in keyrubrics)
        {
            if (!_catalog.TryGet(sleeve[keyRubric.RubricId], keyRubric.RubricName.UniqueKey32(), out IOrigin outcatalog))
            {
                outcatalog = new Registry<IOrigin>();

                ((ISeries<IOrigin>)outcatalog).Put(item);

                _catalog.Put(sleeve[keyRubric.RubricId], keyRubric.RubricName.UniqueKey32(), outcatalog);
            }
            else
            {
                ((ISeries<IOrigin>)outcatalog).Put(item);
            }
        }

        return item;
    }      

    public virtual ITypedSeries<IOrigin> CacheSet<T>() where T : IOrigin
    {
        if (cache.TryGet(GetValidTypeKey(typeof(T)), out IOrigin catalog))
            return (ITypedSeries<IOrigin>)catalog;
        return null;
    }

    public virtual T Lookup<T>(object keys) where T : IOrigin
    {
        if (cache.TryGet(keys, GetValidTypeKey(typeof(T)), out IOrigin output))
            return (T)output;
        return default;
    }

    public virtual ISeries<IOrigin> Lookup<T>(Tuple<string, object> valueNamePair) where T : IOrigin
    { return Lookup<T>((m) => (ISeries<IOrigin>)m.Get(valueNamePair.Item2, valueNamePair.Item2.UniqueKey32())); }

    public virtual ISeries<IOrigin> Lookup<T>(Func<ITypedSeries<IOrigin>, ISeries<IOrigin>> selector) where T : IOrigin
    { return selector(CacheSet<T>()); }

    public virtual T Lookup<T>(T item) where T : IOrigin
    {
        IProxy shell = item.ToProxy();
        IRubrics mrs = shell.Rubrics.KeyRubrics;
        T[] result = new T[mrs.Count];
        int i = 0;
        if (cache.TryGet(GetValidTypeKey(typeof(T)), out IOrigin catalog))
        {
            foreach (MemberRubric mr in mrs)
            {
                if (((ITypedSeries<IOrigin>)catalog).TryGet(
                    shell[mr.RubricId],
                    mr.RubricName.UniqueKey32(),
                    out IOrigin outcatalog))
                    if (((ISeries<IOrigin>)outcatalog).TryGet(item, out IOrigin output))
                        result[i++] = (T)output;
            }
        }

        if (result.Any(r => r == null))
            return default;
        return result[0];
    }

    public virtual T[] Lookup<T>(object key, params Tuple<string, object>[] valueNamePairs) where T : IOrigin
    {
        return Lookup<T>(
            (k) => k[key],
            valueNamePairs.ForEach(
                (vnp) => new Func<ITypedSeries<IOrigin>, ISeries<IOrigin>>(
                    (m) => (ISeries<IOrigin>)m
                                                                    .Get(vnp.Item2, vnp.Item1.UniqueKey32())))
                .ToArray());
    }

    public virtual T[] Lookup<T>(
        Func<ISeries<IOrigin>, IOrigin> key,
        params Func<ITypedSeries<IOrigin>, ISeries<IOrigin>>[] selectors)
        where T : IOrigin
    {
        if (cache.TryGet(GetValidTypeKey(typeof(T)), out IOrigin catalog))
        {
            T[] result = new T[selectors.Length];
            for (int i = 0; i < selectors.Length; i++)
            {
                result[i] = (T)key(selectors[i]((ITypedSeries<IOrigin>)catalog));
            }
            return result;
        }

        return default;
    }

    public virtual ISeries<IOrigin> Lookup<T>(object key, string propertyNames) where T : IOrigin
    {
        if (CacheSet<T>().TryGet(key, propertyNames.UniqueKey32(), out IOrigin outcatalog))
            return (ISeries<IOrigin>)outcatalog;
        return default;
    }

    public virtual T Lookup<T>(T item, params string[] propertyNames) where T : IOrigin
    {
        IProxy ilValuator = item.ToProxy();
        MemberRubric[] mrs = ilValuator.Rubrics.Where(p => propertyNames.Contains(p.RubricName)).ToArray();
        T[] result = new T[mrs.Length];

        if (cache.TryGet(GetValidTypeKey(typeof(T)), out IOrigin catalog))
        {
            int i = 0;
            foreach (MemberRubric mr in mrs)
            {
                if (((ITypedSeries<IOrigin>)catalog).TryGet(
                    ilValuator[mr.RubricId],
                    mr.RubricName.UniqueKey32(),
                    out IOrigin outcatalog))
                    if (((ISeries<IOrigin>)outcatalog).TryGet(item, out IOrigin output))
                        result[i++] = (T)output;
            }
        }

        if (result.Any(r => r == null))
            return default;
        return result[0];
    }

    public virtual IEnumerable<T> Memorize<T>(IEnumerable<T> items) where T : IOrigin
    { return items.ForEach(p => Memorize(p)); }

    public virtual T Memorize<T>(T item) where T : IOrigin
    {
        return InnerMemorize(item);
    }

    public virtual T Memorize<T>(T item, params string[] names) where T : IOrigin
    {
        if (InnerMemorize(item) != null)
            return InnerMemorize(item, names);
        return default(T);
    }

    public virtual async Task<T> MemorizeAsync<T>(T item) where T : IOrigin
    { return await Task.Run(() => Memorize(item)); }
    public virtual async Task<T> MemorizeAsync<T>(T item, params string[] names) where T : IOrigin
    { return await Task.Run(() => Memorize(item, names)); }

    public virtual ITypedSeries<IOrigin> Catalog => cache;

    public virtual Type GetValidType(object obj)
    {
        return obj.GetType();
    }
    public virtual Type GetValidType(Type obj)
    {
        return obj;
    }
    public virtual int GetValidTypeKey(object obj)
    {
        return obj.GetType().UniqueKey32();
    }

    public virtual int GetValidTypeKey(Type obj)
    {
        return obj.UniqueKey32();
    }

}
