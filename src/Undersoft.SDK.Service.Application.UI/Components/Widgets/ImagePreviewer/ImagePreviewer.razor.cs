namespace Undersoft.SDK.Service.Application.Components;

public partial class ImagePreviewer
{
    private string? MinusIconString => CssBuilder.Default("minus-icon")
        .AddClass(MinusIcon)
        .Build();

    private string? PlusIconString => CssBuilder.Default("plus-icon")
        .AddClass(PlusIcon)
        .Build();

    private string? RotateLeftIconString => CssBuilder.Default("rotate-left")
        .AddClass(RotateLeftIcon)
        .Build();

    private string? RotateRightIconString => CssBuilder.Default("rotate-right")
        .AddClass(RotateRightIcon)
        .Build();

    [Parameter]
    public int ZIndex { get; set; } = 2050;

    [Parameter]
    [NotNull]
#if NET6_0_OR_GREATER
    [EditorRequired]
#endif
    public List<string>? PreviewList { get; set; }

    [Parameter]
    public string? PreviousIcon { get; set; }

    [Parameter]
    public string? NextIcon { get; set; }

    [Parameter]
    public string? MinusIcon { get; set; }

    [Parameter]
    public string? PlusIcon { get; set; }

    [Parameter]
    public string? RotateLeftIcon { get; set; }

    [Parameter]
    public string? RotateRightIcon { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    private string? GetFirstImageUrl() => PreviewList.First();

    private bool ShowButtons => PreviewList.Count > 1;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        PreviousIcon ??= IconTheme.GetIconByKey(ComponentIcons.ImagePreviewPreviousIcon);
        NextIcon ??= IconTheme.GetIconByKey(ComponentIcons.ImagePreviewNextIcon);
        MinusIcon ??= IconTheme.GetIconByKey(ComponentIcons.ImagePreviewMinusIcon);
        PlusIcon ??= IconTheme.GetIconByKey(ComponentIcons.ImagePreviewPlusIcon);
        RotateLeftIcon ??= IconTheme.GetIconByKey(ComponentIcons.ImagePreviewRotateLeftIcon);
        RotateRightIcon ??= IconTheme.GetIconByKey(ComponentIcons.ImagePreviewRotateRightIcon);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender)
        {
            await InvokeVoidAsync("update", Id, PreviewList);
        }
    }

    protected override Task InvokeInitAsync() => InvokeVoidAsync("init", Id, PreviewList);
}
