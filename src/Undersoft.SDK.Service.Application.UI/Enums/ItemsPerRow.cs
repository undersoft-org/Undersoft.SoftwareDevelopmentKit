using System.ComponentModel;

namespace Undersoft.SDK.Service.Application.Components;

public enum ItemsPerRow
{
    [Description("12")]
    One,

    [Description("6")]
    Two,

    [Description("4")]
    Three,

    [Description("3")]
    Four,

    [Description("2")]
    Six,

    [Description("1")]
    Twelve
}
