namespace Undersoft.SDK.Instant.Proxies;

using Rubrics;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

public interface IInnerProxy
{
    [JsonIgnore]
    [IgnoreDataMember]
    [NotMapped]
    object this[string propertyName] { get; set; }

    [JsonIgnore]
    [IgnoreDataMember]
    [NotMapped]
    object this[int fieldId] { get; set; }

    [JsonIgnore]
    [IgnoreDataMember]
    [NotMapped]
    IProxy Proxy { get; }
}