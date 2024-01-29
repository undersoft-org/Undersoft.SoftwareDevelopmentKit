namespace Undersoft.SDK.Service.Application.Components;

[AttributeUsage(AttributeTargets.Property)]
public class NullableBoolItemsAttribute : Attribute
{
    public string? NullValueDisplayText { get; set; }

    public string? TrueValueDisplayText { get; set; }

    public string? FalseValueDisplayText { get; set; }
}
