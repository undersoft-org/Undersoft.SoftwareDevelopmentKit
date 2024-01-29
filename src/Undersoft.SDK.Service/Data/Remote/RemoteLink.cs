namespace Undersoft.SDK.Service.Data.Remote;

using Undersoft.SDK.Service.Data.Object;

public class RemoteLink<TSource, TTarget> : DataObject, IRemoteLink<TSource, TTarget> where TSource : class, IOrigin, IInnerProxy where TTarget : class, IOrigin, IInnerProxy
{
    public virtual long LeftEntityId { get; set; }

    public virtual TSource  LeftEntity { get; set; }

    public virtual long RightEntityId { get; set; }

    public virtual TTarget RightEntity { get; set; }
}