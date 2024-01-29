namespace Undersoft.SDK.Service.Application.Components;

public class CaptchaOption
{
    public int Width { get; set; }

    public int Height { get; set; }

    public int SideLength { get; set; } = 42;

    public int Diameter { get; set; } = 9;

    public int OffsetX { get; set; }

    public int OffsetY { get; set; }

    public int BarWidth { get; set; }

    public string? ImageUrl { get; set; }
}
