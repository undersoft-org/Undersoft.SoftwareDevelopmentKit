namespace Undersoft.SDK.Service.Application.Components;

public abstract class MultipleUploadBase<TValue> : UploadBase<TValue>
{
    protected string? GetItemClassString(UploadFile item) => CssBuilder.Default(ItemClassString)
        .AddClass("is-valid", item.Uploaded && item.Code == 0)
        .AddClass("is-invalid", item.Code != 0)
        .AddClass("disabled", IsDisabled)
        .Build();

    protected virtual string? ItemClassString => CssBuilder.Default("upload-item")
        .Build();

    [Parameter]
    public List<UploadFile>? DefaultFileList { get; set; }

    [Parameter]
    public bool ShowProgress { get; set; }

    protected override async Task<bool> OnFileDelete(UploadFile item)
    {
        var ret = await base.OnFileDelete(item);
        if (ret)
        {
            UploadFiles.Remove(item);
            if (!string.IsNullOrEmpty(item.ValidateId))
            {
                await RemoveInvalidTooltip(item);
            }
            if (DefaultFileList != null)
            {
                DefaultFileList.Remove(item);
            }
        }
        return ret;
    }

    protected Task RemoveInvalidTooltip(UploadFile item) => InvokeExecuteAsync(Id, item.ValidateId, "disposeTooltip");

    protected bool GetShowProgress(UploadFile item) => ShowProgress && !item.Uploaded;

    public override void Reset()
    {
        DefaultFileList?.Clear();
        base.Reset();
    }
}
