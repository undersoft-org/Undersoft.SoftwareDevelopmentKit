namespace Undersoft.SDK.Service.Application.Components;

public abstract class SingleUploadBase<TValue> : MultipleUploadBase<TValue>
{
    [Parameter]
    public bool IsSingle { get; set; }

    protected bool CanUpload => !(IsSingle && GetUploadFiles().Count > 0);

    protected virtual List<UploadFile> GetUploadFiles()
    {
        var ret = new List<UploadFile>();
        if (IsSingle)
        {
            if (DefaultFileList?.Any() ?? false)
            {
                ret.Add(DefaultFileList.First());
            }
            if (ret.Count == 0 && UploadFiles.Any())
            {
                ret.Add(UploadFiles.First());
            }
        }
        else
        {
            if (DefaultFileList != null)
            {
                ret.AddRange(DefaultFileList);
            }
            ret.AddRange(UploadFiles);
        }
        return ret;
    }

    protected override async Task<bool> OnFileDelete(UploadFile item)
    {
        var ret = await base.OnFileDelete(item);
        if (ret)
        {
            if (IsSingle)
            {
                UploadFiles.Clear();
            }
            else
            {
                UploadFiles.Remove(item);
            }
            if (!string.IsNullOrEmpty(item.ValidateId))
            {
                await RemoveInvalidTooltip(item);
            }
            RemoveItem();
        }

        void RemoveItem()
        {
            if (DefaultFileList != null)
            {
                if (IsSingle)
                {
                    DefaultFileList.Clear();
                }
                else
                {
                    DefaultFileList.Remove(item);
                }
            }
        }
        return ret;
    }
}
