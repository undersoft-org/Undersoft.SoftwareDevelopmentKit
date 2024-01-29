using System.Text.Json.Serialization;

namespace Undersoft.SDK.Service.Application.Components;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SynthesizerStatus
{
    Synthesizer,

    Finished,

    Cancel,

    Error
}
