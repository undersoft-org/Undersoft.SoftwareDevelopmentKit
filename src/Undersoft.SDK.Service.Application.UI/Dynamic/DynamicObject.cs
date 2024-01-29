using Undersoft.SDK.Instant.Proxies;

namespace Undersoft.SDK.Service.Application.Components;

public class DynamicObject : InnerProxy, IDynamicObject
{
    [AutoGenerateColumn(Ignore = true)]
    public Guid DynamicObjectPrimaryKey { get; set; }

    public virtual object? GetValue(string propertyName) => Proxy[propertyName];

    public virtual void SetValue(string propertyName, object? value) => Proxy[propertyName] = value;
}
