using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class SearchDialog<TModel>
{
    [Parameter]
    [NotNull]
    public Func<Task>? OnResetSearchClick { get; set; }

    [Parameter]
    [NotNull]
    public Func<Task>? OnSearchClick { get; set; }

    [Parameter]
    [NotNull]
    public string? ResetButtonText { get; set; }

    [Parameter]
    [NotNull]
    public string? QueryButtonText { get; set; }

    [Parameter]
    public string? ClearIcon { get; set; }

    [Parameter]
    public string? SearchIcon { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<SearchDialog<TModel>>? Localizer { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        ResetButtonText ??= Localizer[nameof(ResetButtonText)];
        QueryButtonText ??= Localizer[nameof(QueryButtonText)];

        ClearIcon ??= IconTheme.GetIconByKey(ComponentIcons.SearchDialogClearIcon);
        SearchIcon ??= IconTheme.GetIconByKey(ComponentIcons.SearchDialogSearchIcon);
    }
}
