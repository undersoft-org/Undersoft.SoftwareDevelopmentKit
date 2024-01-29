namespace Undersoft.SDK.Service.Application.Components;

public partial class BootstrapLabel
{
    [Parameter]
    [NotNull]
    public string? Value { get; set; }

    [Parameter]
    [NotNull]
    public bool? ShowLabelTooltip { get; set; }

    private bool _showTooltip;

    private string? ClassString => CssBuilder.Default("form-label")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (ShowLabelTooltip.HasValue)
        {
            _showTooltip = ShowLabelTooltip.Value;
        }
        Value ??= "";
    }
}
