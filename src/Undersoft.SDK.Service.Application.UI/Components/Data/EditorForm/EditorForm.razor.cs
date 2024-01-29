using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

#if NET6_0_OR_GREATER
[CascadingTypeParameter(nameof(TModel))]
#endif
public partial class EditorForm<TModel> : IShowLabel
{
    private string? ClassString => CssBuilder.Default("form-body")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? GetCssString(IEditorItem item) => CssBuilder.Default("col-12")
        .AddClass($"col-sm-6 col-md-{Math.Floor(12d / (ItemsPerRow ?? 1))}", item.Items == null && ItemsPerRow != null && item.Rows == 0)
        .Build();

    private string? FormClassString => CssBuilder.Default("row g-3")
        .AddClass("form-inline", RowType == RowType.Inline)
        .AddClass("form-inline-end", RowType == RowType.Inline && LabelAlign == Alignment.Right)
        .AddClass("form-inline-center", RowType == RowType.Inline && LabelAlign == Alignment.Center)
        .Build();

    [Parameter]
    public int? ItemsPerRow { get; set; }

    [Parameter]
    public ItemChangedType ItemChangedType { get; set; }

    [Parameter]
    public RowType RowType { get; set; }

    [Parameter]
    public Alignment LabelAlign { get; set; }

    [Parameter]
    public RenderFragment<TModel>? FieldItems { get; set; }

    [Parameter]
    public RenderFragment? Buttons { get; set; }

    [Parameter]
    [NotNull]
    public TModel? Model { get; set; }

    [Parameter]
    public bool? ShowLabel { get; set; }

    [Parameter]
    public bool? ShowLabelTooltip { get; set; }

    [Parameter]
    public bool IsDisplay { get; set; }

    [CascadingParameter(Name = "IsSearch")]
    [NotNull]
    private bool? IsSearch { get; set; }

    [Parameter]
    public bool AutoGenerateAllItem { get; set; } = true;

    [Parameter]
    public IEnumerable<IEditorItem>? Items { get; set; }

    [Parameter]
    public bool ShowUnsetGroupItemsOnTop { get; set; }

    [CascadingParameter]
    private EditContext? CascadedEditContext { get; set; }

    [CascadingParameter]
    private ValidateForm? ValidateForm { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<EditorForm<TModel>>? Localizer { get; set; }

    [Inject]
    [NotNull]
    private ILookupService? LookupService { get; set; }

    private List<IEditorItem> EditorItems { get; } = new();

    private List<IEditorItem> FormItems { get; } = new();

    private IEnumerable<IEditorItem> UnsetGroupItems => FormItems.Where(i => string.IsNullOrEmpty(i.GroupName)).OrderBy(i => i.Order);

    private IEnumerable<KeyValuePair<string, IOrderedEnumerable<IEditorItem>>> GroupItems => FormItems
        .Where(i => !string.IsNullOrEmpty(i.GroupName))
        .GroupBy(i => i.GroupOrder).OrderBy(i => i.Key)
        .Select(i => new KeyValuePair<string, IOrderedEnumerable<IEditorItem>>(i.First().GroupName!, i.OrderBy(x => x.Order)));

    [NotNull]
    private string? PlaceHolderText { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (CascadedEditContext != null)
        {
            var message = Localizer["ModelInvalidOperationExceptionMessage", nameof(EditorForm<TModel>)];
            if (!CascadedEditContext.Model.GetType().IsAssignableTo(typeof(TModel)))
            {
                throw new InvalidOperationException(message);
            }

            Model = (TModel)CascadedEditContext.Model;
        }

        if (Model == null)
        {
            throw new ArgumentNullException(nameof(Model));
        }

        PlaceHolderText ??= Localizer[nameof(PlaceHolderText)];

        IsSearch ??= false;
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        ShowLabel ??= ValidateForm?.ShowLabel;
    }

    private bool FirstRender { get; set; } = true;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            FirstRender = false;

            if (Items != null)
            {
                FormItems.AddRange(Items);
            }
            else
            {
                if (AutoGenerateAllItem)
                {
                    var items = Utility.GetTableColumns<TModel>().ToList();

                    foreach (var el in EditorItems)
                    {
                        var item = items.FirstOrDefault(i => i.GetFieldName() == el.GetFieldName());
                        if (item != null)
                        {
                            if (!el.Editable)
                            {
                                items.Remove(item);
                            }
                            else
                            {
                                item.Editable = true;
                                item.CopyValue(el);
                            }
                        }
                    }
                    FormItems.AddRange(items.Where(i => i.Editable));
                }
                else
                {
                    FormItems.AddRange(EditorItems.Where(i => i.Editable));
                }
            }
            StateHasChanged();
        }
    }

    private RenderFragment AutoGenerateTemplate(IEditorItem item) => builder =>
    {
        if (IsDisplay || !item.CanWrite(typeof(TModel), ItemChangedType, IsSearch.Value))
        {
            builder.CreateDisplayByFieldType(item, Model);
        }
        else
        {
            item.PlaceHolder ??= PlaceHolderText;
            builder.CreateComponentByFieldType(this, item, Model, ItemChangedType, IsSearch.Value, LookupService);
        }
    };

    private RenderFragment<object>? GetRenderTemplate(IEditorItem item) => IsSearch.Value && item is ITableColumn col
        ? col.SearchTemplate
        : item.EditTemplate;
}
