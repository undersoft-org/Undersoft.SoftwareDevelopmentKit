using Microsoft.FluentUI.AspNetCore.Components;
using Undersoft.SDK.Instant.Proxies;

namespace Undersoft.SDK.Service.Application.UI.Generic;

public class GenericPanel<TPanel, TModel> : IGenericDialog<TModel> where TPanel : IDialogContentComponent<IGenericData<TModel>> where TModel : class, IOrigin, IInnerProxy
{
    public GenericPanel(IDialogService dialogService)
    {
        DialogService = dialogService;
    }

    public IDialogService DialogService { get; private set; }

    public IGenericData<TModel>? Content { get; set; }

    public IDialogReference? Reference { get; set; }

    public virtual async Task Show(IGenericData<TModel> data)
    {
        if (DialogService != null)
        {
            var dialog = await DialogService.ShowPanelAsync<TPanel>(data, new DialogParameters<TModel>()
            {
                Height = data.Height,
                Width = data.Width,
                Title = data.Title,
                ShowTitle = true,
                Alignment = HorizontalAlignment.Right,
                PrimaryAction = "OK",
                SecondaryAction = null,
                ShowDismiss = true
            });

            var result = await dialog.Result;
            if (!result.Cancelled && result.Data != null)
            {
                Content = (IGenericData<TModel>)result.Data;
            }
        }
    }

    public virtual async Task Show(IGenericData<TModel> data, Action<DialogParameters> setup)
    {
        if (DialogService != null)
        {
            var parameters = new DialogParameters();
            parameters.Height = data.Height;
            parameters.Width = data.Width;
            parameters.Title = data.Title;
            setup(parameters);
            Reference = await DialogService.ShowPanelAsync<TPanel>(data, parameters);

            var result = await Reference.Result;
            if (!result.Cancelled && result.Data != null)
            {
                Content = (IGenericData<TModel>)result.Data;
            }
        }
    }

    public virtual async Task Show(Action<DialogParameters<TModel>> setup)
    {
        if (DialogService != null)
        {
            var parameters = new DialogParameters<TModel>();
            setup(parameters);
            Reference = await DialogService.ShowPanelAsync<TPanel>(parameters);

            var result = await Reference.Result;
            if (!result.Cancelled && result.Data != null)
            {
                Content = (IGenericData<TModel>)result.Data;
            }
        }
    }
}
