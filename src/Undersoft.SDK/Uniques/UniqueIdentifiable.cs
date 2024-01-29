using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Undersoft.SDK.Uniques;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;

namespace Undersoft.SDK.Uniques;

using Uniques;
using Instant.Proxies;
using Instant.Attributes;
using Instant.Rubrics;
using Instant.Rubrics.Attributes;
using Undersoft.SDK.Extracting;
using Undersoft.SDK.Serialization;
using Undersoft.SDK;

[DataContract]
[StructLayout(LayoutKind.Sequential, Pack = 2, CharSet = CharSet.Ansi)]
public abstract class UniqueIdentifiable : InnerProxy, IUniqueIdentifiable
{
    private int[] keyOrdinals;

    public UniqueIdentifiable() : base(true) 
    {      
    }

    public UniqueIdentifiable(bool autoId) : base(autoId)
    {
    }

    public UniqueIdentifiable(object id) : this(id.UniqueKey64()) { }

    public UniqueIdentifiable(long id) : this()
    {
        code.SetId(id);
    }

 

    [NotMapped]
    [JsonIgnore]
    [IgnoreDataMember]
    public virtual byte Flags
    {
        get => (byte)code.GetFlags();
        set => code.SetFlagBits(new BitVector32(value));
    }

    [NotMapped]
    [JsonIgnore]
    [IgnoreDataMember]
    public virtual bool Inactive
    {
        get => GetFlag(1);
        set => SetFlag(value, 1);
    }

    [NotMapped]
    [JsonIgnore]
    [IgnoreDataMember]
    public virtual bool Locked
    {
        get => GetFlag(0);
        set => SetFlag(value, 0);
    }

    [NotMapped]
    [JsonIgnore]
    [IgnoreDataMember]
    public override int OriginId
    {
        get { return (int)code.OriginId; }
        set { code.SetOriginId(value); }
    }

    [NotMapped]
    [JsonIgnore]
    [IgnoreDataMember]
    public virtual bool Obsolete
    {
        get => GetFlag(2);
        set => SetFlag(value, 2);
    }

    [NotMapped]
    [JsonIgnore]
    [IgnoreDataMember]
    public virtual byte Priority
    {
        get => GetPriority();
        set => SetPriority(value);
    }
   
    public override long AutoId()
    {
        long key = code.Id;
        if (key != 0)
            return (long)key;

        long id = Unique.NewId;
        code.SetId(id);
        return (long)id;
    }

    public void ClearFlag(ushort position)
    {
        code.ClearFlagBit(position);
    }

    public virtual long CompactKey()
    {
        return UniqueValues().UniqueKey64();
    }

    public int CompareTo(IUniqueIdentifiable other)
    {
        return code.CompareTo(other);
    }

    public override int CompareTo(IUnique other)
    {
        return code.CompareTo(other);
    }

    public bool Equals(BitVector32 other)
    {
        return ((IEquatable<BitVector32>)code).Equals(other);
    }

    public bool Equals(DateTime other)
    {
        return ((IEquatable<DateTime>)code).Equals(other);
    }

    public bool Equals(IUniqueIdentifiable other)
    {
        return code.Equals(other);
    }

    public bool Equals(IUniqueStructure other)
    {
        return ((IEquatable<IUniqueStructure>)code).Equals(other);
    }

    public override bool Equals(IUnique other)
    {
        return code.Equals(other);
    }

    public bool GetFlag(ushort position)
    {
        return code.GetFlagBit(position);
    }

    public void SetFlag(ushort position)
    {
        code.SetFlagBit(position);
    }

    public void SetFlag(bool flag, ushort position)
    {
        code.SetFlag(flag, position);
    }

    public virtual int[] UniqueOrdinals()
    {
        if (keyOrdinals == null)
        {
            IRubrics pks = ((IInnerProxy)this).Proxy.Rubrics.KeyRubrics;
            if (pks.Any())
            {
                keyOrdinals = pks.Select(p => p.RubricId).ToArray();
            }
        }
        return keyOrdinals;
    }

    public virtual object[] UniqueValues()
    {
        int[] ids = keyOrdinals;
        if (ids == null)
            ids = UniqueOrdinals();
        return ids.Select(k => this.Proxy[k]).ToArray();
    }

    public byte SetPriority(byte priority)
    {
        return code.SetPriority(priority);
    }

    public IOrigin Sign()
    {
        return Sign(this);
    }

    public TEntity Sign<TEntity>() where TEntity : class, IOrigin
    {
        return (TEntity)Sign((TEntity)(object)this);
    }

    public IOrigin Sign(object id)
    {
        return Sign(this, id);
    }

    public TEntity Sign<TEntity>(object id) where TEntity : class, IOrigin
    {
        return (TEntity)Sign((TEntity)(object)this);
    }

    public TEntity Sign<TEntity>(TEntity entity, object id) where TEntity : class, IOrigin
    {
        entity.SetId(id);
        Stamp(entity);
        Created = Time;
        return entity;
    }

    public IOrigin Stamp()
    {
        return Stamp(this);
    }

    public TEntity Stamp<TEntity>() where TEntity : class, IOrigin
    {
        return Stamp((TEntity)(object)this);
    }

    public byte ComparePriority(IOrigin entity)
    {
        return code.ComparePriority(entity.GetPriority());
    }

}
