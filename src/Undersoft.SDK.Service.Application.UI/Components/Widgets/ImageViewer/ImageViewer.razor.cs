namespace Undersoft.SDK.Service.Application.Components;

public partial class ImageViewer
{
    private string? ClassString => CssBuilder.Default("bb-img")
        .AddClass("is-preview", ShowPreviewList)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? ImageClassString => CssBuilder.Default()
        .AddClass($"obj-fit-{FitMode.ToDescriptionString()}")
        .AddClass("d-none", ShouldHandleError && !IsLoaded)
        .Build();

    [Parameter]
    [NotNull]
#if NET6_0_OR_GREATER
    [EditorRequired]
#endif
    public string? Url { get; set; }

    [Parameter]
    public bool IsAsync { get; set; }

    [Parameter]
    public string? Alt { get; set; }

    [Parameter]
    public bool ShowPlaceHolder { get; set; }

    [Parameter]
    public bool HandleError { get; set; }

    [Parameter]
    public RenderFragment? PlaceHolderTemplate { get; set; }

    [Parameter]
    public RenderFragment? ErrorTemplate { get; set; }

    [Parameter]
    public ObjectFitMode FitMode { get; set; }

    [Parameter]
    public int ZIndex { get; set; } = 2050;

    [Parameter]
    public List<string>? PreviewList { get; set; }

    [Parameter]
    public Func<string, Task>? OnErrorAsync { get; set; }

    [Parameter]
    public Func<string, Task>? OnLoadAsync { get; set; }

    [Parameter]
    public string? FileIcon { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    private bool ShowImage => !string.IsNullOrEmpty(Url);

    private bool IsLoaded { get; set; }

    private bool IsError { get; set; }

    private string? IsAsyncString => IsAsync ? "true" : null;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        FileIcon ??= IconTheme.GetIconByKey(ComponentIcons.ImageViewerFileIcon);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender)
        {
            await InvokeVoidAsync("update", Id, PreviewList);
        }
    }

    protected override Task InvokeInitAsync() => InvokeVoidAsync("init", Id, Url, PreviewList);

    private RenderFragment RenderChildContent() => builder =>
    {
        if (!IsError)
        {
            builder.OpenElement(0, "img");
            builder.AddAttribute(1, "class", ImageClassString);
            if (!IsAsync)
            {
                builder.AddAttribute(2, "src", Url);
            }
            if (!string.IsNullOrEmpty(Alt))
            {
                builder.AddAttribute(3, "alt", Alt);
            }
            if (ShowPlaceHolder || ShouldHandleError)
            {
                builder.AddAttribute(4, "onload", EventCallback.Factory.Create(this, async () =>
                {
                    IsLoaded = true;
                    if (OnLoadAsync != null)
                    {
                        await OnLoadAsync(Url);
                    }
                }));
            }
            if (ShouldHandleError)
            {
                builder.AddAttribute(4, "onerror", EventCallback.Factory.Create(this, async () =>
                {
                    IsError = true;
                    if (OnErrorAsync != null)
                    {
                        await OnErrorAsync(Url);
                    }
                }));
            }
            builder.CloseElement();

            if (ShouldRenderPlaceHolder)
            {
                builder.AddContent(6, PlaceHolderTemplate ?? RenderPlaceHolder());
            }
        }
        else
        {
            builder.AddContent(7, ErrorTemplate ?? RenderErrorTemplate());
        }
    };

    private bool ShouldRenderPlaceHolder => (ShowPlaceHolder || PlaceHolderTemplate != null) && !IsLoaded;

    private bool ShouldHandleError => HandleError || ErrorTemplate != null;

    private bool ShowPreviewList => PreviewList?.Any() ?? false;

    private string PreviewerId => $"prev_{Id}";
}
