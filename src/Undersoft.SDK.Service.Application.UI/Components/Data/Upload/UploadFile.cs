using Microsoft.AspNetCore.Components.Forms;

namespace Undersoft.SDK.Service.Application.Components;

public class UploadFile
{
    public string? FileName { get; set; }

    public string? OriginFileName { get; internal set; }

    public long Size { get; set; }

    public int Code { get; set; }

    public string? PrevUrl { get; set; }

    public string? Error { get; set; }

    public IBrowserFile? File { get; set; }

    internal Action<UploadFile>? UpdateCallback { get; set; }

    internal int ProgressPercent { get; set; }

    internal bool Uploaded { get; set; } = true;

    internal string? ValidateId { get; set; }

    internal bool? IsValid { get; set; }

    public string? GetFileName() => OriginFileName ?? FileName;

    public string? GetExtension() => Path.GetExtension(GetFileName());
}
