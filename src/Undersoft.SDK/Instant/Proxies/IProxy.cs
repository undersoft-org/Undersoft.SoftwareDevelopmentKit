namespace Undersoft.SDK.Instant.Proxies;

using Rubrics;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

public interface IProxy : IInstant
{
    [JsonIgnore]
    [IgnoreDataMember]    
    IRubrics Rubrics { get; set; }

    [JsonIgnore]
    [IgnoreDataMember]  
    object Target { get; set; }
}
