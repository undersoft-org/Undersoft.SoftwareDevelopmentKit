using System.Text.Json.Serialization;

namespace Undersoft.SDK.Service.Data.Remote;

using Entity;
using System.Runtime.Serialization;
using Undersoft.SDK.Service.Data.Object;
using Uniques;

public interface IRemoteLink<TSource, TTarget> : IDataObject
    where TSource : class, IOrigin, IInnerProxy
    where TTarget : class, IOrigin, IInnerProxy
{
    [JsonIgnore]
    [IgnoreDataMember]
    TSource LeftEntity { get; set; }
    long LeftEntityId { get; set; }

    [JsonIgnore]
    [IgnoreDataMember]
    TTarget RightEntity { get; set; }
    long RightEntityId { get; set; }
}
