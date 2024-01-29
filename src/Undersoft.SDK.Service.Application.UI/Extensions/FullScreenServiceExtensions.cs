namespace Undersoft.SDK.Service.Application.Components;

public static class FullScreenServiceExtensions
{
    public static Task ToggleByElement(this FullScreenService service, ElementReference element) => service.Toggle(new() { Element = element });

    public static Task ToggleById(this FullScreenService service, string id) => service.Toggle(new() { Id = id });
}
