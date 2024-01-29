using System.ComponentModel;

namespace Undersoft.SDK.Service.Application.Components;

public enum ChartAction
{
    [Description("update")]
    Update,

    [Description("addDataset")]
    AddDataset,

    [Description("removeDataset")]
    RemoveDataset,

    [Description("addData")]
    AddData,

    [Description("removeData")]
    RemoveData,

    [Description("setAngle")]
    SetAngle,

    [Description("reload")]
    Reload
}
