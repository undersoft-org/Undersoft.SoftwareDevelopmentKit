using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Undersoft.SDK.Uniques;

namespace Undersoft.SDK.Series;

public class Listing<TUnique> : KeyedCollection<long, TUnique>, IFindableSeries, IListing<TUnique> where TUnique : IIdentifiable
{
    public Listing() : base()
    {

    }
    public Listing(IEnumerable<TUnique> list)
    {
        foreach (var item in list)
            Add(item);
    }

    protected override long GetKeyForItem(TUnique item)
    {
        if (item.Id == 0)
            return (long)(item.Id = (long)Unique.NewId);
        return (long)item.Id;
    }

    public TUnique Single
    {
        get => this.FirstOrDefault();
    }

    public object this[object key]
    {
        get
        {
            TryGetValue((long)key.UniqueKey64(), out TUnique result);
            return result;
        }
        set
        {
            Dictionary[(long)key.UniqueKey64()] = (TUnique)value;
        }
    }
}