using Microsoft.AspNetCore.Components.Forms;

namespace Undersoft.SDK.Service.Application.Components;

public class EditDialogOption<TModel> : DialogOption
{
    public EditDialogOption()
    {
        ShowCloseButton = false;
        ShowFooter = false;
    }

    public bool IsTracking { get; set; }

    public bool ShowLabel { get; set; } = true;

    public ItemChangedType ItemChangedType { get; set; }

    public int? ItemsPerRow { get; set; }

    public RowType RowType { get; set; }

    public Alignment LabelAlign { get; set; }

    public bool ShowLoading { get; set; }

    public bool ShowUnsetGroupItemsOnTop { get; set; }

    public TModel? Model { get; set; }

    public bool? DisableAutoSubmitFormByEnter { get; set; }

    public IEnumerable<IEditorItem>? Items { get; set; }

    public RenderFragment<TModel>? DialogBodyTemplate { get; set; }

    public RenderFragment<TModel>? DialogFooterTemplate { get; set; }

    public Func<EditContext, Task<bool>>? OnEditAsync { get; set; }
}
