using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Undersoft.SDK.Uniques;

namespace Undersoft.SDK.Instant.Proxies;

using Undersoft.SDK;

[DataContract]
[StructLayout(LayoutKind.Sequential)]
public class InnerProxy : Origin, IInnerProxy
{
    public InnerProxy(bool autoId) : base(autoId) { }
    public InnerProxy() : base(true) { }

    [NotMapped]
    [JsonIgnore]
    [IgnoreDataMember]
    internal IProxy proxy;

    [NotMapped]
    [JsonIgnore]
    [IgnoreDataMember]
    public IProxy Proxy => proxy ??= CreateProxy();

    object IInnerProxy.this[string propertyName]
    {
        get => Proxy[propertyName];
        set => Proxy[propertyName] = value;
    }
    object IInnerProxy.this[int id]
    {
        get => Proxy[id];
        set => Proxy[id] = value;
    }

    protected virtual void CreateProxy(Func<InnerProxy, IProxy> compileAction)
    {
        proxy = compileAction.Invoke(this);
    }

    protected virtual IProxy CreateProxy()
    {
        Type type = this.GetType();

        if (TypeId == 0)
            TypeId = type.UniqueKey32();

        if (type.IsAssignableTo(typeof(IProxy)))
            return (IProxy)this;

        return ProxyFactory.GetCreator(type, TypeId).Create(this);
    }
}
