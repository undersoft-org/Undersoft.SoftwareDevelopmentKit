using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Undersoft.SDK.Uniques;

namespace Undersoft.SDK.Series;

public class UniqueList<TOrigin> : KeyedCollection<long, TOrigin>, IFindableSeries, IListing<TOrigin>, IIdentifiable where TOrigin : IIdentifiable
{
    private readonly Uscn _code;

    public UniqueList() : base()
    {
        _code = new Uscn();
        Id = Unique.NewId;
        TypeId = this.GetType().UniqueKey32();
    }
    public UniqueList(IEnumerable<TOrigin> list) : this()
    {
        foreach (var item in list)
            Add(item);
    }

    protected override long GetKeyForItem(TOrigin item)
    {
        return item.Id == 0 ? (long)Unique.NewId : item.Id;
    }

    public byte[] GetBytes()
    {
        return ((IUnique)_code).GetBytes();
    }

    public byte[] GetKeyBytes()
    {
        return ((IUnique)_code).GetIdBytes();
    }

    public bool Equals(IUnique other)
    {
        return ((IEquatable<IUnique>)_code).Equals(other);
    }

    public int CompareTo(IUnique other)
    {
        return ((IComparable<IUnique>)_code).CompareTo(other);
    }

    public TOrigin Single
    {
        get => this.FirstOrDefault();
    }

    public long Id { get => ((IIdentifiable)_code).Id; set => ((IIdentifiable)_code).Id = value; }
    public long TypeId { get => ((IIdentifiable)_code).TypeId; set => ((IIdentifiable)_code).TypeId = value; }
    

    public object this[object key]
    {
        get
        {
            TryGetValue((long)key.UniqueKey64(), out TOrigin result);
            return result;
        }
        set
        {
            Dictionary[(long)key.UniqueKey64()] = (TOrigin)value;
        }
    }
}