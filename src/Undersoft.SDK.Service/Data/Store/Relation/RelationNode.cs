using System.ComponentModel.DataAnnotations.Schema;


namespace Undersoft.SDK.Service.Data.Store.Relation;

using Uniques;
using Entity;

using Undersoft.SDK.Service.Data.Object;

public class RelationNode<TLeft, TRight> : DataObject, IRelationNode<TLeft, TRight> where TLeft : class, IDataObject where TRight : class, IDataObject
{
    public virtual long RightEntityId { get; set; }

    public virtual TRight RightEntity { get; set; }

    public virtual long LeftEntityId { get; set; }

    public virtual TLeft LeftEntity { get; set; }
}