using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class IconDialog
{
    [Parameter]
    [NotNull]
#if NET6_0_OR_GREATER
    [EditorRequired]
#endif
    public string? IconName { get; set; }

    [Parameter]
    public string? LabelText { get; set; }

    [Parameter]
    public string? LabelFullText { get; set; }

    [Parameter]
    public string? ButtonText { get; set; }

    [Parameter]
    public string? CopiedTooltipText { get; set; }

    private string IconFullName => $"<i class=\"{IconName}\" aria-hidden=\"true\"></i>";

    [Inject]
    [NotNull]
    private IStringLocalizer<IconDialog>? Localizer { get; set; }

    private IEnumerable<SelectedItem> Items { get; } = new List<SelectedItem>()
    {
        new("solid", "Solid"),
        new("regular", "Regular")
    };

    private string IconStyle { get; set; } = "solid";

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        LabelText ??= Localizer[nameof(LabelText)];
        LabelFullText ??= Localizer[nameof(LabelFullText)];
        ButtonText ??= Localizer[nameof(ButtonText)];
        CopiedTooltipText ??= Localizer[nameof(CopiedTooltipText)];

        IconName ??= "";
        IconName = IconName
            .Replace("fas", "fa-solid", StringComparison.OrdinalIgnoreCase)
            .Replace("far", "fa-regular", StringComparison.OrdinalIgnoreCase);
    }

    private Task OnValueChanged(string val)
    {
        IconName = val == "solid"
            ? IconName.Replace("fa-regular", "fa-solid")
            : IconName.Replace("fa-solid", "fa-regular");
        return Task.CompletedTask;
    }
}
