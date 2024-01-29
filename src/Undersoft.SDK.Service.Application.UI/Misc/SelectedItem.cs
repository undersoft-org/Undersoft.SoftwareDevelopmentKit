namespace Undersoft.SDK.Service.Application.Components;

public class SelectedItem
{
    public SelectedItem() { }

    public SelectedItem(string value, string text) => (Value, Text) = (value, text);

    public virtual string Text { get; set; } = "";

    public string Value { get; set; } = "";

    public bool Active { get; set; }

    public bool IsDisabled { get; set; }

    public string GroupName { get; set; } = "";
}
