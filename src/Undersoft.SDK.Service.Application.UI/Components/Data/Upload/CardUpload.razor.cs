namespace Undersoft.SDK.Service.Application.Components;

public partial class CardUpload<TValue>
{
    private string? BodyClassString => CssBuilder.Default("upload-body is-card")
        .AddClass("is-single", IsSingle)
        .Build();

    private string? GetDiabledString(UploadFile item) => (!IsDisabled && item.Uploaded && item.Code == 0) ? null : "disabled";

    private bool ShowPreviewList => GetUploadFiles().Any();

    private List<string?> PreviewList => GetUploadFiles().Select(i => i.PrevUrl).ToList();

    private string? GetDeleteButtonDiabledString(UploadFile item) => (!IsDisabled && item.Uploaded) ? null : "disabled";

    private string? CardItemClass => CssBuilder.Default("upload-item")
        .AddClass("disabled", IsDisabled)
        .Build();

    private string? StatusIconString => CssBuilder.Default("valid-icon")
        .AddClass(StatusIcon)
        .Build();

    private string PreviewerId => $"prev_{Id}";

    [Parameter]
    public RenderFragment<UploadFile>? IconTemplate { get; set; }

    [Parameter]
    public string? AddIcon { get; set; }

    [Parameter]
    public string? StatusIcon { get; set; }

    [Parameter]
    public string? DeleteIcon { get; set; }

    [Parameter]
    public string? RemoveIcon { get; set; }

    [Parameter]
    public string? DownloadIcon { get; set; }

    [Parameter]
    public string? ZoomIcon { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        AddIcon ??= IconTheme.GetIconByKey(ComponentIcons.CardUploadAddIcon);
        StatusIcon ??= IconTheme.GetIconByKey(ComponentIcons.CardUploadStatusIcon);
        DeleteIcon ??= IconTheme.GetIconByKey(ComponentIcons.CardUploadDeleteIcon);
        RemoveIcon ??= IconTheme.GetIconByKey(ComponentIcons.CardUploadRemoveIcon);
        DownloadIcon ??= IconTheme.GetIconByKey(ComponentIcons.CardUploadDownloadIcon);
        ZoomIcon ??= IconTheme.GetIconByKey(ComponentIcons.CardUploadZoomIcon);
    }

    private static bool IsImage(UploadFile item)
    {
        bool ret;
        if (item.File != null)
        {
            ret = item.File.ContentType.Contains("image", StringComparison.OrdinalIgnoreCase) || CheckExtensions(item.File.Name);
        }
        else
        {
            ret = IsBase64Format() || CheckExtensions(item.FileName ?? item.PrevUrl ?? "");
        }

        bool IsBase64Format() => !string.IsNullOrEmpty(item.PrevUrl) && item.PrevUrl.StartsWith("data:image/", StringComparison.OrdinalIgnoreCase);

        bool CheckExtensions(string fileName) => Path.GetExtension(fileName).ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" or ".png" or ".bmp" or ".gif" => true,
            _ => false
        };
        return ret;
    }

    [Parameter]
    public Func<UploadFile, Task>? OnZoomAsync { get; set; }

    private async Task OnCardFileDelete(UploadFile item)
    {
        await OnFileDelete(item);
        StateHasChanged();
    }

    private async Task OnClickZoom(UploadFile item)
    {
        if (OnZoomAsync != null)
        {
            await OnZoomAsync(item);
        }
    }
}
