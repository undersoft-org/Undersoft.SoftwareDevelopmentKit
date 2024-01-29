using System.ComponentModel;

namespace Undersoft.SDK.Service.Application.Components;

public enum Color
{
    [Description("none")]
    None,

    [Description("active")]
    Active,

    [Description("primary")]
    Primary,

    [Description("secondary")]
    Secondary,

    [Description("success")]
    Success,

    [Description("danger")]
    Danger,

    [Description("warning")]
    Warning,

    [Description("info")]
    Info,

    [Description("light")]
    Light,

    [Description("dark")]
    Dark,

    [Description("link")]
    Link
}
