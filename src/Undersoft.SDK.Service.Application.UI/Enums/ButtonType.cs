using System.ComponentModel;

namespace Undersoft.SDK.Service.Application.Components;

public enum ButtonType
{
    [Description("button")]
    Button,

    [Description("submit")]
    Submit,

    [Description("reset")]
    Reset
}
