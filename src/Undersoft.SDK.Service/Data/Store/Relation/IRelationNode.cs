using System.Text.Json.Serialization;

namespace Undersoft.SDK.Service.Data.Store.Relation;

using Entity;

using Undersoft.SDK.Service.Data.Object;
using Uniques;

public interface IRelationNode<TLeft, TRight> : IDataObject where TLeft : class, IDataObject where TRight : class, IDataObject
{
    [JsonIgnore]
    TLeft LeftEntity { get; set; }
    long LeftEntityId { get; set; }
    [JsonIgnore]
    TRight RightEntity { get; set; }
    long RightEntityId { get; set; }
}