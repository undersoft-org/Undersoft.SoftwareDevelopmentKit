using System.Text.Json.Serialization;

namespace Undersoft.SDK.Service.Application.Components;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RecognizerStatus
{
    Start,

    Finished,

    Close,

    Error
}
