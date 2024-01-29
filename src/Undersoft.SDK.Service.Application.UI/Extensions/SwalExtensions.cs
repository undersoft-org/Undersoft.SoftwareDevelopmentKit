namespace Undersoft.SDK.Service.Application.Components;

public static class SwalExtensions
{
    public static async Task<bool> ShowModal(this SwalService service, SwalOption option, SweetAlert? swal = null)
    {
        option.IsConfirm = true;
        await service.Show(option, swal);
        return await option.ReturnTask.Task;
    }

    public static IDictionary<string, object?> Parse(this SwalOption option) => new Dictionary<string, object?>()
    {
        [nameof(SweetAlertBody.Category)] = option.Category,
        [nameof(SweetAlertBody.ShowClose)] = option.ShowClose,
        [nameof(SweetAlertBody.IsConfirm)] = option.IsConfirm,
        [nameof(SweetAlertBody.ShowFooter)] = option.ShowFooter,
        [nameof(SweetAlertBody.OnCloseAsync)] = async () =>
        {
            if (option.IsConfirm)
            {
                option.ReturnTask.TrySetResult(false);
            }
            if (option.OnCloseAsync != null)
            {
                await option.OnCloseAsync();
            }
        },
        [nameof(SweetAlertBody.OnConfirmAsync)] = async () =>
        {
            if (option.IsConfirm)
            {
                option.ReturnTask.TrySetResult(true);
            }
            if (option.OnConfirmAsync != null)
            {
                await option.OnConfirmAsync();
            }
        },
        [nameof(SweetAlertBody.Title)] = option.Title,
        [nameof(SweetAlertBody.Content)] = option.Content,
        [nameof(SweetAlertBody.BodyTemplate)] = option.BodyTemplate,
        [nameof(SweetAlertBody.FooterTemplate)] = option.FooterTemplate,
        [nameof(SweetAlertBody.ButtonTemplate)] = option.ButtonTemplate,
        [nameof(SweetAlertBody.CloseButtonIcon)] = option.CloseButtonIcon,
        [nameof(SweetAlertBody.ConfirmButtonIcon)] = option.ConfirmButtonIcon,
        [nameof(SweetAlertBody.CloseButtonText)] = option.CloseButtonText,
        [nameof(SweetAlertBody.CancelButtonText)] = option.CancelButtonText,
        [nameof(SweetAlertBody.ConfirmButtonText)] = option.ConfirmButtonText
    };
}
