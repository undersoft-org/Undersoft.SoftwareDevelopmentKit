using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Undersoft.SDK.Service.Data.Object;

using AutoMapper;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;
using Undersoft.SDK.Serialization;
using Undersoft.SDK.Service.Data.Identifier;


[DataContract]
[StructLayout(LayoutKind.Sequential)]
public class DataObjectDetail<TDetail> : DetailObject, IDataObjectDetail<TDetail> where TDetail : class, IDataObject
{
    public DataObjectDetail() : base()
    {
    }

    public virtual Identifiers<TDetail> Identifiers { get; set; } = new Identifiers<TDetail>();

    public TDetail GetObject()
    {
        return base.GetObject<TDetail>();
    }

    public void SetDocument(TDetail structure)
    {
        base.SetDocument<TDetail>(structure);
    }

}


[DataContract]
[StructLayout(LayoutKind.Sequential)]
public class DetailObject : DataObject, ISerializableObject, IDataObjectDetail
{
    [NotMapped]
    [JsonIgnore]
    [IgnoreDataMember]
    [IgnoreMap]
    internal IDataSerializer _serializer;

    public DetailObject() : base() 
    {
        _serializer = new DataSerializer(this);
    }

    public virtual JsonDocument Document { get; set; }

    public virtual string TypeName { get; set; }

    public virtual T GetObject<T>()
    {
        return _serializer.GetObject<T>();
    }

    public virtual void SetDocument<T>(T structure)
    {
        _serializer.SetDocument(structure);
    }

}
