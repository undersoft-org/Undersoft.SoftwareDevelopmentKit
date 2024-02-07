using Microsoft.FluentUI.AspNetCore.Components;
using Undersoft.SDK.Instant.Proxies;
using Undersoft.SDK.Service.Application.UI.Generic;

namespace Undersoft.SDK.Service.Application.UI.Panels;

public class EventPanel<TPanel, TModel> : GenericPanel<TPanel, TModel> where TPanel : IDialogContentComponent<IGenericData<TModel>> where TModel : class, IOrigin, IInnerProxy
{
    public EventPanel(IDialogService dialogService, IMessageService messageService) : base(dialogService)
    {
        MessageService = messageService;
    }

    public IMessageService MessageService { get; private set; }

    public async Task Show()
    {
        if (DialogService != null)
        {
            await this.Show((p) =>
            {
                p.Alignment = HorizontalAlignment.Right;
                p.Title = $"Notifications";
                p.PrimaryAction = null;
                p.SecondaryAction = null;
                p.ShowDismiss = true;
            });
        }
    }
}
