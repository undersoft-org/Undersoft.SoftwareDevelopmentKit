using Undersoft.SDK.Instant;
using Undersoft.SDK.Uniques;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;

namespace Undersoft.SDK.Service.Data.Service.Remote;

using Instant.Rubrics;

public enum Towards
{
    None,
    ToSet,
    ToSingle,
    SetToSet
}

public abstract class ServiceRemoteBase : IServiceRemote
{
    protected Uscn serialcode;

    protected ServiceRemoteBase() { }

    public virtual Towards Towards { get; set; }

    public virtual IUnique Empty => Uscn.Empty;

    public virtual string CodeNo
    {
        get => serialcode.CodeNo;
        set => serialcode.CodeNo = value;
    }

    public virtual long Id
    {
        get => serialcode.Id;
        set => serialcode.Id = value;
    }
    public virtual long TypeId
    {
        get => serialcode.TypeId;
        set => serialcode.TypeId = value;
    }

    public virtual int CompareTo(IUnique other)
    {
        return serialcode.CompareTo(other);
    }

    public virtual bool Equals(IUnique other)
    {
        return serialcode.Equals(other);
    }

    public virtual byte[] GetBytes()
    {
        return serialcode.GetBytes();
    }

    public virtual byte[] GetIdBytes()
    {
        return serialcode.GetIdBytes();
    }

    public virtual MemberRubric RemoteMember { get; }
}
