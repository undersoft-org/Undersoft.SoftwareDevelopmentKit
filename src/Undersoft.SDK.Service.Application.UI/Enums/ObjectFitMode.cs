using System.ComponentModel;

namespace Undersoft.SDK.Service.Application.Components;

public enum ObjectFitMode
{
    [Description("fill")]
    Fill,

    [Description("contain")]
    Contain,

    [Description("cover")]
    Cover,

    [Description("none")]
    None,

    [Description("scale-down")]
    ScaleDown
}
