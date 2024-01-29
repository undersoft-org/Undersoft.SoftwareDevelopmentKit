namespace Undersoft.SDK.Service.Application.Components;

public interface IResultDialog
{
    Task<bool> OnClosing(DialogResult result) => Task.FromResult(true);

    Task OnClose(DialogResult result);
}
