namespace Undersoft.SDK.Service.Application.Components;

public class BootstrapDynamicComponent
{
    private IDictionary<string, object?>? Parameters { get; set; }

    private Type ComponentType { get; }

    public BootstrapDynamicComponent(Type componentType, IDictionary<string, object?>? parameters = null)
    {
        ComponentType = componentType;
        Parameters = parameters;
    }

    public static BootstrapDynamicComponent CreateComponent<TCom>(IDictionary<string, object?>? parameters = null) where TCom : IComponent => new(typeof(TCom), parameters);

    public static BootstrapDynamicComponent CreateComponent<TCom>() where TCom : IComponent => CreateComponent<TCom>(new Dictionary<string, object?>());

    public RenderFragment Render() => builder =>
    {
        var index = 0;
        builder.OpenComponent(index++, ComponentType);
        if (Parameters != null)
        {
            foreach (var p in Parameters)
            {
                builder.AddAttribute(index++, p.Key, p.Value);
            }
        }
        builder.CloseComponent();
    };
}
