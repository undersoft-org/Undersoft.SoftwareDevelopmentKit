using System.ComponentModel;

namespace Undersoft.SDK.Service.Application.Components;

public enum Placement
{
    [Description("auto")]
    Auto,

    [Description("top")]
    Top,

    [Description("top-start")]
    TopStart,

    [Description("top-center")]
    TopCenter,

    [Description("top-end")]
    TopEnd,

    [Description("middle")]
    Middle,

    [Description("middel-start")]
    MiddleStart,

    [Description("middel-center")]
    MiddleCenter,

    [Description("middle-end")]
    MiddleEnd,

    [Description("bottom")]
    Bottom,

    [Description("bottom-start")]
    BottomStart,

    [Description("bottom-center")]
    BottomCenter,

    [Description("bottom-end")]
    BottomEnd,

    [Description("left")]
    Left,

    [Description("left-start")]
    LeftStart,

    [Description("left-end")]
    LeftEnd,

    [Description("right")]
    Right,

    [Description("right-start")]
    RightStart,

    [Description("right-end")]
    RightEnd,
}
