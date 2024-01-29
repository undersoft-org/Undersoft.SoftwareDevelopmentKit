using System.Text.Json;
using System.Text.Json.Serialization;

namespace Undersoft.SDK.Service.Application.Components;

public class CommodityEntity
{
    public string? Row { get; set; }

    [JsonPropertyName("word")]
    public string? CommodityName { get; set; }
}
