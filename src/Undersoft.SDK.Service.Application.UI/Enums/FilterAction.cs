using System.ComponentModel;

namespace Undersoft.SDK.Service.Application.Components;

public enum FilterAction
{
    [Description("等于")]
    Equal,

    [Description("不等于")]
    NotEqual,

    [Description("大于")]
    GreaterThan,

    [Description("大于等于")]
    GreaterThanOrEqual,

    [Description("小于")]
    LessThan,

    [Description("小于等于")]
    LessThanOrEqual,

    [Description("包含")]
    Contains,

    [Description("不包含")]
    NotContains,

    [Description("自定义条件")]
    CustomPredicate
}
