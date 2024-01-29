using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class Table<TItem>
{
    [Parameter]
    [NotNull]
    public string? AddButtonText { get; set; }

    [Parameter]
    public string? AddButtonIcon { get; set; }

    [Parameter]
    [NotNull]
    public string? EditButtonText { get; set; }

    [Parameter]
    public string? EditButtonIcon { get; set; }

    [Parameter]
    [NotNull]
    public string? UpdateButtonText { get; set; }

    [Parameter]
    [NotNull]
    public string? CancelButtonText { get; set; }

    [Parameter]
    public string? CancelButtonIcon { get; set; }

    [Parameter]
    [NotNull]
    public string? DeleteButtonText { get; set; }

    [Parameter]
    public string? DeleteButtonIcon { get; set; }

    [Parameter]
    [NotNull]
    public string? CancelDeleteButtonText { get; set; }

    [Parameter]
    [NotNull]
    public string? SaveButtonText { get; set; }

    [Parameter]
    public string? SaveButtonIcon { get; set; }

    [Parameter]
    [NotNull]
    public string? CloseButtonText { get; set; }

    [Parameter]
    public string? CloseButtonIcon { get; set; }

    [Parameter]
    [NotNull]
    public string? ConfirmDeleteButtonText { get; set; }

    [Parameter]
    [NotNull]
    public string? ConfirmDeleteContentText { get; set; }

    [Parameter]
    [NotNull]
    public string? RefreshButtonText { get; set; }

    [Parameter]
    public string? RefreshButtonIcon { get; set; }

    [Parameter]
    [NotNull]
    public string? CardViewButtonText { get; set; }

    [Parameter]
    public string? CardViewButtonIcon { get; set; }

    [Parameter]
    [NotNull]
    public string? ColumnButtonTitleText { get; set; }

    [Parameter]
    [NotNull]
    public string? ColumnButtonText { get; set; }

    [Parameter]
    public string? CopyColumnButtonIcon { get; set; }

    [Parameter]
    [NotNull]
    public string? ExportButtonText { get; set; }

    [Parameter]
    [NotNull]
    public string? SearchPlaceholderText { get; set; }

    [Parameter]
    [NotNull]
    public string? SearchButtonText { get; set; }

    [Parameter]
    public string? SearchButtonIcon { get; set; }

    [Parameter]
    [NotNull]
    public string? SearchModalTitle { get; set; }

    [Parameter]
    [NotNull]
    public string? SearchTooltip { get; set; }

    [Parameter]
    [NotNull]
    public string? ResetSearchButtonText { get; set; }

    [Parameter]
    public string? ResetSearchButtonIcon { get; set; }

    [Parameter]
    [NotNull]
    public string? AdvanceButtonText { get; set; }

    [Parameter]
    public string? AdvanceButtonIcon { get; set; }

    [Parameter]
    [NotNull]
    public string? AddButtonToastTitle { get; set; }

    [Parameter]
    [NotNull]
    public string? AddButtonToastContent { get; set; }

    [Parameter]
    [NotNull]
    public string? EditButtonToastTitle { get; set; }

    [Parameter]
    [NotNull]
    public string? EditButtonToastNotSelectContent { get; set; }

    [Parameter]
    [NotNull]
    public string? EditButtonToastReadonlyContent { get; set; }

    [Parameter]
    [NotNull]
    public string? EditButtonToastMoreSelectContent { get; set; }

    [Parameter]
    [NotNull]
    public string? EditButtonToastNoSaveMethodContent { get; set; }

    [Parameter]
    [NotNull]
    public string? SaveButtonToastTitle { get; set; }

    [Parameter]
    [NotNull]
    public string? SaveButtonToastContent { get; set; }

    [Parameter]
    [NotNull]
    public string? SaveButtonToastResultContent { get; set; }

    [Parameter]
    [NotNull]
    public string? SuccessText { get; set; }

    [Parameter]
    [NotNull]
    public string? FailText { get; set; }

    [Parameter]
    [NotNull]
    public string? DeleteButtonToastTitle { get; set; }

    [Parameter]
    [NotNull]
    public string? DeleteButtonToastCanNotDeleteContent { get; set; }

    [Parameter]
    [NotNull]
    public string? DeleteButtonToastContent { get; set; }

    [Parameter]
    [NotNull]
    public string? DeleteButtonToastResultContent { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<Table<TItem>>? Localizer { get; set; }

    private void OnInitLocalization()
    {
        AddButtonText ??= Localizer[nameof(AddButtonText)];
        EditButtonText ??= Localizer[nameof(EditButtonText)];
        UpdateButtonText ??= Localizer[nameof(UpdateButtonText)];
        DeleteButtonText ??= Localizer[nameof(DeleteButtonText)];
        CancelButtonText ??= Localizer[nameof(CancelButtonText)];
        SaveButtonText ??= Localizer[nameof(SaveButtonText)];
        CloseButtonText ??= Localizer[nameof(CloseButtonText)];
        CancelDeleteButtonText ??= Localizer[nameof(CancelDeleteButtonText)];
        ConfirmDeleteButtonText ??= Localizer[nameof(ConfirmDeleteButtonText)];
        ConfirmDeleteContentText ??= Localizer[nameof(ConfirmDeleteContentText)];
        RefreshButtonText ??= Localizer[nameof(RefreshButtonText)];
        CardViewButtonText ??= Localizer[nameof(CardViewButtonText)];
        ColumnButtonTitleText ??= Localizer[nameof(ColumnButtonTitleText)];
        ColumnButtonText ??= Localizer[nameof(ColumnButtonText)];
        ExportButtonText ??= Localizer[nameof(ExportButtonText)];
        SearchPlaceholderText ??= Localizer[nameof(SearchPlaceholderText)];
        SearchButtonText ??= Localizer[nameof(SearchButtonText)];
        ResetSearchButtonText ??= Localizer[nameof(ResetSearchButtonText)];
        AdvanceButtonText ??= Localizer[nameof(AdvanceButtonText)];
        CheckboxDisplayText ??= Localizer[nameof(CheckboxDisplayText)];
        EditModalTitle ??= Localizer[nameof(EditModalTitle)];
        AddModalTitle ??= Localizer[nameof(AddModalTitle)];
        ColumnButtonTemplateHeaderText ??= Localizer[nameof(ColumnButtonTemplateHeaderText)];
        SearchTooltip ??= Localizer[nameof(SearchTooltip)];
        SearchModalTitle ??= Localizer[nameof(SearchModalTitle)];
        AddButtonToastTitle ??= Localizer[nameof(AddButtonToastTitle)];
        AddButtonToastContent ??= Localizer[nameof(AddButtonToastContent)];
        EditButtonToastTitle ??= Localizer[nameof(EditButtonToastTitle)];
        EditButtonToastNotSelectContent ??= Localizer[nameof(EditButtonToastNotSelectContent)];
        EditButtonToastMoreSelectContent ??= Localizer[nameof(EditButtonToastMoreSelectContent)];
        EditButtonToastNoSaveMethodContent ??= Localizer[nameof(EditButtonToastNoSaveMethodContent)];
        EditButtonToastReadonlyContent ??= Localizer[nameof(EditButtonToastReadonlyContent)];
        SaveButtonToastTitle ??= Localizer[nameof(SaveButtonToastTitle)];
        SaveButtonToastContent ??= Localizer[nameof(SaveButtonToastContent)];
        SaveButtonToastResultContent ??= Localizer[nameof(SaveButtonToastResultContent)];
        SuccessText ??= Localizer[nameof(SuccessText)];
        FailText ??= Localizer[nameof(FailText)];
        DeleteButtonToastTitle ??= Localizer[nameof(DeleteButtonToastTitle)];
        DeleteButtonToastContent ??= Localizer[nameof(DeleteButtonToastContent)];
        DeleteButtonToastResultContent ??= Localizer[nameof(DeleteButtonToastResultContent)];
        DeleteButtonToastCanNotDeleteContent ??= Localizer[nameof(DeleteButtonToastCanNotDeleteContent)];
        DataServiceInvalidOperationText ??= Localizer[nameof(DataServiceInvalidOperationText), typeof(TItem).FullName!];
        NotSetOnTreeExpandErrorMessage = Localizer[nameof(NotSetOnTreeExpandErrorMessage)];
        UnsetText ??= Localizer[nameof(UnsetText)];
        SortAscText ??= Localizer[nameof(SortAscText)];
        SortDescText ??= Localizer[nameof(SortDescText)];
        EmptyText ??= Localizer[nameof(EmptyText)];
        LineNoText ??= Localizer[nameof(LineNoText)];
        ExportToastTitle ??= Localizer[nameof(ExportToastTitle)];
        ExportToastContent ??= Localizer[nameof(ExportToastContent)];
        ExportToastInProgressContent ??= Localizer[nameof(ExportToastInProgressContent)];
        ExportExcelDropdownItemText ??= Localizer[nameof(ExportExcelDropdownItemText)];
        CopyColumnTooltipText ??= Localizer[nameof(CopyColumnTooltipText)];
        CopyColumnCopiedTooltipText ??= Localizer[nameof(CopyColumnCopiedTooltipText)];
    }
}
