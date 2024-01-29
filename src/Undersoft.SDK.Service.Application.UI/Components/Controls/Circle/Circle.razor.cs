namespace Undersoft.SDK.Service.Application.Components;

public sealed partial class Circle
{
    [Parameter]
    public int Value { get; set; }

    private string? ValueString => $"{Math.Round(((1 - Value * 1.0 / 100) * CircleLength), 2)}";

    private string ValueTitleString => $"{Value}%";
}
