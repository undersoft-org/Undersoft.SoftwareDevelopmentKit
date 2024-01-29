namespace Undersoft.SDK.Service.Application.Components;

public static class ToastServiceExtensions
{
    public static Task Success(this ToastService service, string? title = null, string? content = null, bool autoHide = true) => Success(service, title, content, autoHide, true);

    public static Task Success(this ToastService service, string? title, string? content, bool autoHide, bool showClose) => service.Show(new ToastOption()
    {
        Category = ToastCategory.Success,
        IsAutoHide = autoHide,
        Title = title ?? "",
        Content = content ?? "",
        ShowClose = showClose
    });

    public static Task Error(this ToastService service, string? title = null, string? content = null, bool autoHide = true) => Error(service, title, content, autoHide, true);

    public static Task Error(this ToastService service, string? title, string? content, bool autoHide, bool showClose) => service.Show(new ToastOption()
    {
        Category = ToastCategory.Error,
        IsAutoHide = autoHide,
        Title = title ?? "",
        Content = content ?? "",
        ShowClose = showClose
    });

    public static Task Information(this ToastService service, string? title = null, string? content = null, bool autoHide = true) => Information(service, title, content, autoHide, true);

    public static Task Information(this ToastService service, string? title, string? content, bool autoHide, bool showClose) => service.Show(new ToastOption()
    {
        Category = ToastCategory.Information,
        IsAutoHide = autoHide,
        Title = title ?? "",
        Content = content ?? "",
        ShowClose = showClose
    });

    public static Task Warning(this ToastService service, string? title = null, string? content = null, bool autoHide = true) => Warning(service, title, content, autoHide, true);

    public static Task Warning(this ToastService service, string? title, string? content, bool autoHide, bool showClose) => service.Show(new ToastOption()
    {
        Category = ToastCategory.Warning,
        IsAutoHide = autoHide,
        Title = title ?? "",
        Content = content ?? "",
        ShowClose = showClose
    });
}
