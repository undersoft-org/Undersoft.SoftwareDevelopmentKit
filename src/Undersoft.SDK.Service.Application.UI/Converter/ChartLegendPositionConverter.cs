using Undersoft.SDK.Service.Application.Components;

namespace System.Text.Json.Serialization;

class ChartLegendPositionConverter : JsonConverter<ChartLegendPosition>
{
    public override ChartLegendPosition Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, ChartLegendPosition value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToDescriptionString());
    }
}
