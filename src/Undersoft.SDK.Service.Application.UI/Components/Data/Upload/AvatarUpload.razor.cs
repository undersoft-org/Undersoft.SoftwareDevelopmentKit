using Microsoft.AspNetCore.Components.Forms;

namespace Undersoft.SDK.Service.Application.Components;

public partial class AvatarUpload<TValue>
{
    protected new string? GetItemClassString(UploadFile item) => CssBuilder.Default(ItemClassString)
        .AddClass("is-valid", !IsDisabled && item.IsValid.HasValue && item.IsValid.Value)
        .AddClass("is-invalid", !IsDisabled && item.IsValid.HasValue && !item.IsValid.Value)
        .AddClass("is-valid", !IsDisabled && !item.IsValid.HasValue && item.Uploaded && item.Code == 0)
        .AddClass("is-invalid", !IsDisabled && !item.IsValid.HasValue && item.Code != 0)
        .AddClass("disabled", IsDisabled)
        .Build();

    protected override string? ItemClassString => CssBuilder.Default(base.ItemClassString)
        .AddClass("is-circle", IsCircle)
        .AddClass("is-single", IsSingle)
        .AddClass("disabled", IsDisabled)
        .Build();

    private string? PrevStyleString => CssBuilder.Default()
        .AddClass($"width: {Width}px;", Width > 0)
        .AddClass($"height: {Height}px;", Height > 0 && !IsCircle)
        .AddClass($"height: {Width}px;", IsCircle)
        .Build();

    private string? ValidStatusIconString => CssBuilder.Default("valid-icon")
        .AddClass(ValidStatusIcon)
        .Build();

    private string? InvalidStatusIconString => CssBuilder.Default("valid-icon")
        .AddClass(InvalidStatusIcon)
        .Build();

    [Parameter]
    public int Width { get; set; } = 100;

    [Parameter]
    public int Height { get; set; } = 100;

    [Parameter]
    public bool IsCircle { get; set; }

    [Parameter]
    public string? DeleteIcon { get; set; }

    [Parameter]
    public string? LoadingIcon { get; set; }

    [Parameter]
    public string? AddIcon { get; set; }

    [Parameter]
    public string? ValidStatusIcon { get; set; }

    [Parameter]
    public string? InvalidStatusIcon { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        DeleteIcon ??= IconTheme.GetIconByKey(ComponentIcons.AvatarUploadDelteIcon);
        LoadingIcon ??= IconTheme.GetIconByKey(ComponentIcons.AvatarUploadLoadingIcon);
        AddIcon ??= IconTheme.GetIconByKey(ComponentIcons.AvatarUploadAddIcon);
        ValidStatusIcon ??= IconTheme.GetIconByKey(ComponentIcons.AvatarUploadValidStatusIcon);
        InvalidStatusIcon ??= IconTheme.GetIconByKey(ComponentIcons.AvatarUploadInvalidStatusIcon);
    }

    protected override async Task OnFileChange(InputFileChangeEventArgs args)
    {
        CurrentFile = new UploadFile()
        {
            OriginFileName = args.File.Name,
            Size = args.File.Size,
            File = args.File,
            Uploaded = false
        };
        CurrentFile.ValidateId = $"{Id}_{CurrentFile.GetHashCode()}";

        if (IsSingle)
        {
            DefaultFileList?.Clear();
            UploadFiles.Clear();
        }

        UploadFiles.Add(CurrentFile);

        await base.OnFileChange(args);

        CurrentFile.IsValid = IsValid;

        if (OnChange != null)
        {
            await OnChange(CurrentFile);
        }
        else
        {
            await CurrentFile.RequestBase64ImageFileAsync(CurrentFile.File.ContentType, 320, 240);
        }
    }

    protected override string? RetrieveId() => CurrentFile?.ValidateId;
}
