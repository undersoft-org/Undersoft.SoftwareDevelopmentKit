namespace Undersoft.SDK.Service.Application.Components;

public class SearchDialogOption<TModel> : DialogOption
{
    public SearchDialogOption()
    {
        ShowCloseButton = false;
        ShowFooter = false;
    }

    public bool ShowLabel { get; set; } = true;

    public bool ShowUnsetGroupItemsOnTop { get; set; }

    public int? ItemsPerRow { get; set; }

    public RowType RowType { get; set; }

    public Alignment LabelAlign { get; set; }

    public TModel? Model { get; set; }

    public IEnumerable<IEditorItem>? Items { get; set; }

    public RenderFragment<TModel>? DialogBodyTemplate { get; set; }

    public string? ResetButtonText { get; set; }

    public string? QueryButtonText { get; set; }

    public Func<Task>? OnResetSearchClick { get; set; }

    public Func<Task>? OnSearchClick { get; set; }
}
