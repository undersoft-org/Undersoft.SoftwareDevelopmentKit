namespace Undersoft.SDK.Service.Application.Components;

public abstract class DialogBase<TModel> : ComponentModule
{
    [Parameter]
    [NotNull]
    public TModel? Model { get; set; }

    [Parameter]
    public RenderFragment<TModel>? BodyTemplate { get; set; }

    [Parameter]
    public IEnumerable<IEditorItem>? Items { get; set; }

    [Parameter]
    public bool ShowLabel { get; set; }

    [Parameter]
    public int? ItemsPerRow { get; set; }

    [Parameter]
    public RowType RowType { get; set; }

    [Parameter]
    public Alignment LabelAlign { get; set; }

    [Parameter]
    public bool ShowUnsetGroupItemsOnTop { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (Model == null)
        {
            throw new InvalidOperationException("Model value not set to null");
        }
    }
}
