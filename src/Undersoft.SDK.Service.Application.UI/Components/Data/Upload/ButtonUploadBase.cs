using Microsoft.AspNetCore.Components.Forms;

namespace Undersoft.SDK.Service.Application.Components;

public abstract class ButtonUploadBase<TValue> : SingleUploadBase<TValue>
{
    [Parameter]
    public bool IsDirectory { get; set; }

    [Parameter]
    public bool IsMultiple { get; set; }

    [Parameter]
    public Func<string?, string>? OnGetFileFormat { get; set; }

    [Parameter]
    public bool ShowDownloadButton { get; set; }

    [Parameter]
    public Func<UploadFile, Task>? OnDownload { get; set; }

    [Parameter]
    public string? FileIconExcel { get; set; }

    [Parameter]
    public string? FileIconDocx { get; set; }

    [Parameter]
    public string? FileIconPPT { get; set; }

    [Parameter]
    public string? FileIconAudio { get; set; }

    [Parameter]
    public string? FileIconVideo { get; set; }

    [Parameter]
    public string? FileIconCode { get; set; }

    [Parameter]
    public string? FileIconPdf { get; set; }

    [Parameter]
    public string? FileIconZip { get; set; }

    [Parameter]
    public string? FileIconArchive { get; set; }

    [Parameter]
    public string? FileIconImage { get; set; }

    [Parameter]
    public string? FileIconFile { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (IsDirectory)
        {
            IsMultiple = true;
        }
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        FileIconExcel ??= IconTheme.GetIconByKey(ComponentIcons.FileIconExcel);
        FileIconDocx ??= IconTheme.GetIconByKey(ComponentIcons.FileIconDocx);
        FileIconPPT ??= IconTheme.GetIconByKey(ComponentIcons.FileIconPPT);
        FileIconAudio ??= IconTheme.GetIconByKey(ComponentIcons.FileIconAudio);
        FileIconVideo ??= IconTheme.GetIconByKey(ComponentIcons.FileIconVideo);
        FileIconCode ??= IconTheme.GetIconByKey(ComponentIcons.FileIconCode);
        FileIconPdf ??= IconTheme.GetIconByKey(ComponentIcons.FileIconPdf);
        FileIconZip ??= IconTheme.GetIconByKey(ComponentIcons.FileIconZip);
        FileIconArchive ??= IconTheme.GetIconByKey(ComponentIcons.FileIconArchive);
        FileIconImage ??= IconTheme.GetIconByKey(ComponentIcons.FileIconImage);
        FileIconFile ??= IconTheme.GetIconByKey(ComponentIcons.FileIconFile);
    }

    protected override async Task OnFileChange(InputFileChangeEventArgs args)
    {
        if (IsMultiple)
        {
            var items = args.GetMultipleFiles(args.FileCount).Select(f => new UploadFile()
            {
                OriginFileName = f.Name,
                Size = f.Size,
                File = f,
                Uploaded = OnChange == null,
                UpdateCallback = Update
            }).ToList();
            UploadFiles.AddRange(items);
            if (OnChange != null)
            {
                foreach (var item in items)
                {
                    await OnChange(item);
                    item.Uploaded = true;
                    StateHasChanged();
                }
            }
        }
        else
        {
            var file = new UploadFile()
            {
                OriginFileName = args.File.Name,
                Size = args.File.Size,
                File = args.File,
                Uploaded = false,
                UpdateCallback = Update
            };
            UploadFiles.Add(file);
            if (OnChange != null)
            {
                await OnChange(file);
            }
            file.Uploaded = true;
        }
    }

    private void Update(UploadFile file)
    {
        if (GetShowProgress(file))
        {
            StateHasChanged();
        }
    }

    protected string? GetFileFormatClassString(UploadFile item)
    {
        var builder = CssBuilder.Default("file-icon");
        var fileExtension = Path.GetExtension(item.OriginFileName ?? item.FileName);
        if (!string.IsNullOrEmpty(fileExtension))
        {
            fileExtension = fileExtension.ToLowerInvariant();
        }
        var icon = OnGetFileFormat?.Invoke(fileExtension) ?? fileExtension switch
        {
            ".csv" or ".xls" or ".xlsx" => FileIconExcel,
            ".doc" or ".docx" or ".dot" or ".dotx" => FileIconDocx,
            ".ppt" or ".pptx" => FileIconPPT,
            ".wav" or ".mp3" => FileIconAudio,
            ".mp4" or ".mov" or ".mkv" => FileIconVideo,
            ".cs" or ".html" or ".vb" => FileIconCode,
            ".pdf" => FileIconPdf,
            ".zip" or ".rar" or ".iso" => FileIconZip,
            ".txt" or ".log" => FileIconArchive,
            ".jpg" or ".jpeg" or ".png" or ".bmp" or ".gif" => FileIconImage,
            _ => FileIconFile
        };
        builder.AddClass(icon);
        return builder.Build();
    }

    protected override IDictionary<string, object> GetUploadAdditionalAttributes()
    {
        var ret = base.GetUploadAdditionalAttributes();

        if (IsMultiple)
        {
            ret.Add("multiple", "multiple");
        }

        if (IsDirectory)
        {
            ret.Add("directory", "dicrectory");
            ret.Add("webkitdirectory", "webkitdirectory");
        }
        return ret;
    }

    protected async Task OnClickDownload(UploadFile item)
    {
        if (OnDownload != null)
        {
            await OnDownload(item);
        }
    }
}
