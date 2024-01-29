using System.ComponentModel;

namespace Undersoft.SDK.Service.Application.Components;

public enum Alignment
{
    None,

    [Description("start")]
    Left,

    [Description("center")]
    Center,

    [Description("end")]
    Right
}
