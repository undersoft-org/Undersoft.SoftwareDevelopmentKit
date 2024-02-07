using Microsoft.FluentUI.AspNetCore.Components;
using Undersoft.SDK.Instant.Proxies;
using Undersoft.SDK.Service.Application.UI.Generic;

namespace Undersoft.SDK.Service.Application.UI.Panels;

public class SetupPanel<TPanel, TModel> : GenericPanel<TPanel, TModel> where TPanel : IDialogContentComponent<IGenericData<TModel>> where TModel : class, IOrigin, IInnerProxy
{
    public SetupPanel(IDialogService dialogService) : base(dialogService)
    {
    }

    public override async Task Show(IGenericData<TModel> data)
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
}
