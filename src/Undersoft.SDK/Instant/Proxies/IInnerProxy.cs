namespace Undersoft.SDK.Instant.Proxies;

using Rubrics;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

public interface IInnerProxy
{
    object this[string propertyName] { get; set; }

    object this[int fieldId] { get; set; }

    [JsonIgnore]
    [IgnoreDataMember]
    [NotMapped]
    IProxy Proxy { get; }
}