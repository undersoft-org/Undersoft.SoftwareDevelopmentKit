namespace Undersoft.SDK.Service.Application.Components;

internal class TabItemTextOptions
{
    public string? Text { get; set; }

    public string? Icon { get; set; }

    public bool IsActive { get; set; } = true;

    public bool Closable { get; set; } = true;

    public void Reset()
    {
        Text = null;
        Icon = null;
        IsActive = true;
        Closable = true;
    }

    public bool Valid() => Text != null;
}
