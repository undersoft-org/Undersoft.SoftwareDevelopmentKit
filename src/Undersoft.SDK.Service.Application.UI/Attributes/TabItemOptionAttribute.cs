namespace Undersoft.SDK.Service.Application.Components;

[AttributeUsage(AttributeTargets.Class)]
public class TabItemOptionAttribute : Attribute
{
    public string? Text { get; set; }

    public bool Closable { get; set; } = true;

    public string? Icon { get; set; }
}
