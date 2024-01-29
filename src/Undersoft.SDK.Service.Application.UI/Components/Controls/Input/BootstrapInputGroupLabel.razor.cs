namespace Undersoft.SDK.Service.Application.Components;

public partial class BootstrapInputGroupLabel
{
    private string? ClassString => CssBuilder.Default()
        .AddClass("Input-group-text", IsInnerLabel)
        .AddClass("form-label", !IsInnerLabel)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private bool IsInnerLabel { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        IsInnerLabel = InputGroup != null;
    }
}
