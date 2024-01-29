namespace Undersoft.SDK.Service.Application.Components;

public class TableSettings
{
    public int CheckboxColumnWidth { get; set; } = 36;

    public int DetailColumnWidth { get; set; } = 24;

    public int ShowCheckboxTextColumnWidth { get; set; } = 80;

    public int LineNoColumnWidth { get; set; } = 60;

    public TableRenderMode? TableRenderMode { get; set; }
}
