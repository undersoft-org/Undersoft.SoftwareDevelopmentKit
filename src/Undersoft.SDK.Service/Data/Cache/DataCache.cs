using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Undersoft.SDK.Instant;
using System.Linq;
using Undersoft.SDK.Series;
using System.Threading.Tasks;
using Undersoft.SDK.Uniques;
using Undersoft.SDK.Service.Data.Store;

namespace Undersoft.SDK.Service.Data.Cache;

using Invoking;
using Instant.Proxies;
using Instant.Rubrics;
using Entity;
using Undersoft.SDK.Service.Data.Object;
using Undersoft.SDK;

public class DataCache : TypedCache<IOrigin>, IDataCache
{
    public DataCache() : this(TimeSpan.FromMinutes(15), null, 259)
    {
    }

    public DataCache(TimeSpan? lifeTime = null, IInvoker callback = null, int capacity = 259) : base(
        lifeTime,
        callback,
        capacity)
    {
        if (cache == null)
        {
            cache = this;
        }
    }

    protected override T InnerMemorize<T>(T item)
    {
        int group = typeof(T).GetDataTypeId();
        if (!cache.TryGet(group, out IOrigin deck))
        {
            deck = new TypedRegistry<IOrigin>();

            cache.Add(group, deck);

            ((ITypedSeries<IOrigin>)deck).Put(item);

            cache.Add(item);

            return item;
        }

        if (!cache.ContainsKey(item))
        {
            ((ITypedSeries<IOrigin>)deck).Put(item);

            cache.Add(item);
        }

        return item;
    }

    protected override T InnerMemorize<T>(T item, params string[] names)
    {
        Memorize(item);        

        return item;
    }

    public static IRubrics AssignKeyRubrics(ProxyCreator sleeve, IOrigin item)
    {
        if (!sleeve.Rubrics.KeyRubrics.Any())
        {
            IEnumerable<bool[]>[] rk = item.GetIndentityProperties()
                .AsItems()
                .Select(
                    p => p.Id != (int)DbIdentityType.NONE
                        ? p.Value
                            .Select(
                                e => new[]
                                        {
                                            sleeve.Rubrics[e.Name].IsKey = true,
                                            sleeve.Rubrics[e.Name].IsIdentity = true
                                        })
                        : p.Value.Select(h => new[] { sleeve.Rubrics[h.Name].IsIdentity = true }))
                .ToArray();

            sleeve.Rubrics.KeyRubrics.Put(sleeve.Rubrics.Where(p => p.IsIdentity == true).ToArray());
            sleeve.Rubrics.Update();
        }

        return sleeve.Rubrics.KeyRubrics;
    }

    public override T Memorize<T>(T item)
    {
        if (!item.IsEventType())
            return InnerMemorize(item);
        return default;
    }

    public override int GetValidTypeKey(Type obj)
    {
        return obj.GetDataTypeId();
    }
    public override Type GetValidType(Type obj)
    {
        return obj.GetDataType();
    }
    public override int GetValidTypeKey(object obj)
    {
        return obj.GetDataTypeId();
    }
    public override Type GetValidType(object obj)
    {
        return obj.GetDataType();
    }
}
