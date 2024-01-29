namespace Undersoft.SDK.Service.Application.Components;

public static class PrintServiceExtensions
{
    public static async Task PrintAsync<TComponent>(this PrintService service, Func<DialogOption, IDictionary<string, object?>> parametersFactory) where TComponent : ComponentBase
    {
        var option = new DialogOption();
        var parameters = parametersFactory(option);
        option.Component = BootstrapDynamicComponent.CreateComponent<TComponent>(parameters);
        await service.PrintAsync(option);
    }
}
