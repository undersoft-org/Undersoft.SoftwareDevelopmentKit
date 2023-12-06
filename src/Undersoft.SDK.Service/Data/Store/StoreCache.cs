using Undersoft.SDK.Series;
using Undersoft.SDK.Service.Data.Cache;
using Undersoft.SDK.Service.Data.Mapper;
using Undersoft.SDK.Uniques;
using System;

namespace Undersoft.SDK.Service.Data.Store;

using Invoking;
using Undersoft.SDK;

public class StoreCache<TStore> : DataCache, IStoreCache<TStore>
{
    public StoreCache(IDataCache cache)
    {
        if (base.cache == null || this.cache == null)
        {
            Mapper = ServiceManager.GetObject<IDataMapper>();
            base.cache = cache;
            int seed = typeof(TStore).UniqueKey32();
            TypeId = seed;
            if (!base.Catalog.TryGet(seed, out IOrigin deck))
            {
                deck = new TypedRegistry<IOrigin>();
                base.Catalog.Add(seed, deck);
            }
            this.cache = (ITypedSeries<IOrigin>)deck;
        }
    }

    public StoreCache(TimeSpan? lifeTime = null, Invoker callback = null, int capacity = 257) : base(
        lifeTime,
        callback,
        capacity)
    {
        if (cache == null)
        {
            int seed = typeof(TStore).UniqueKey32();
            TypeId = seed;
            if (!base.Catalog.TryGet(seed, out IOrigin deck))
            {
                deck = new TypedRegistry<IOrigin>();
                base.Catalog.Add(seed, deck);
            }
            cache = (ITypedSeries<IOrigin>)deck;
        }
    }

    protected override ITypedSeries<IOrigin> cache { get; set; }

    public override ITypedSeries<IOrigin> Catalog => cache;

    public IDataMapper Mapper { get; set; }
}
