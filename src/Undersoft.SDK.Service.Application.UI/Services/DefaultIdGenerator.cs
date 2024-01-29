namespace Undersoft.SDK.Service.Application.Components;

internal class DefaultIdGenerator : IComponentIdGenerator
{
    public string Generate(object component) => $"bb_{component.GetHashCode()}";
}
