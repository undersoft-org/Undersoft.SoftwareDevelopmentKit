namespace Undersoft.SDK.Service.Application.Components;

[AttributeUsage(AttributeTargets.Property)]
public class PlaceHolderAttribute : Attribute
{
    public string Text { get; }

    public PlaceHolderAttribute(string placeholder)
    {
        Text = placeholder;
    }
}
