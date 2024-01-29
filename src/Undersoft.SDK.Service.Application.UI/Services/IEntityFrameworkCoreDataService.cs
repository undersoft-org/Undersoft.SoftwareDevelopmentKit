namespace Undersoft.SDK.Service.Application.Components;

public interface IEntityFrameworkCoreDataService
{
    Task CancelAsync();

    Task EditAsync(object model);
}
