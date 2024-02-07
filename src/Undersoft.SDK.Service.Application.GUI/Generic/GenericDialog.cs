using Microsoft.FluentUI.AspNetCore.Components;
using Undersoft.SDK.Instant.Proxies;

namespace Undersoft.SDK.Service.Application.UI.Generic;

public class GenericDialog<TDialog, TModel> : IGenericDialog<TModel> where TDialog : IDialogContentComponent<IGenericData<TModel>> where TModel : class, IOrigin, IInnerProxy
{
    public GenericDialog(IDialogService dialogService)
    {
        Service = dialogService;
    }

    public IDialogService Service { get; private set; }

    public IGenericData<TModel>? Content { get; set; }

    public IDialogReference? Reference { get; set; }

    public virtual async Task Show(IGenericData<TModel> data)
    {
        if (Service != null)
        {
            Reference = await Service.ShowDialogAsync<TDialog>(data, new DialogParameters()
            {
                Height = data.Height,
                Width = data.Width,
                Title = data.Title,
            });

            var result = await Reference.Result;
            if (!result.Cancelled && result.Data != null)
            {
                Content = (IGenericData<TModel>)result.Data;
            }
        }
    }

    public virtual async Task Show(IGenericData<TModel> data, Action<DialogParameters> setup)
    {
        if (Service != null)
        {
            var parameters = new DialogParameters();
            parameters.Height = data.Height;
            parameters.Width = data.Width;
            parameters.Title = data.Title;
            setup(parameters);
            Reference = await Service.ShowPanelAsync<TDialog>(data, parameters);

            var result = await Reference.Result;
            if (!result.Cancelled && result.Data != null)
            {
                Content = (IGenericData<TModel>)result.Data;
            }
        }
    }

    public virtual async Task Show(Action<DialogParameters<TModel>> setup)
    {
        if (Service != null)
        {
            var parameters = new DialogParameters<TModel>();
            setup(parameters);
            Reference = await Service.ShowPanelAsync<TDialog>(parameters);

            var result = await Reference.Result;
            if (!result.Cancelled && result.Data != null)
            {
                Content = (IGenericData<TModel>)result.Data;
            }
        }
    }
}
