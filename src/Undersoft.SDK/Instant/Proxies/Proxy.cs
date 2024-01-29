using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Undersoft.SDK.Uniques;

namespace Undersoft.SDK.Instant.Proxies;

public class Proxy<T> : Proxy
{
    protected new T target { get; set; }

    public Proxy(T target)
    {
        this.target = target;
    }

    public Proxy(T target, Action<IInnerProxy, T> compilationAction)
    {
        this.target = target;
        CreateProxy(compilationAction);
    }

    protected virtual void CreateProxy(Action<IInnerProxy, T> compilationAction)
    {
        compilationAction.Invoke(this, target);
    }

    protected override IProxy CreateProxy()
    {
        Type type = typeof(T);

        if (type.IsAssignableTo(typeof(IProxy)))
            return (IProxy)target;

        return proxy = ProxyFactory.GetCreator<T>().Create(target);
    }
}

public class Proxy : InnerProxy
{
    protected virtual object target { get; set; }

    public Proxy() { }

    public Proxy(object target) 
    {
        this.target = target;
        CreateProxy(); 
    }

    public Proxy(object target, Func<InnerProxy, IProxy> compilationAction)
    {
        this.target = target;
        CreateProxy(compilationAction);
    }

    protected override void CreateProxy(Func<InnerProxy, IProxy> compilationAction)
    {
        proxy = compilationAction.Invoke(this);
    }

    protected override IProxy CreateProxy()
    {
        Type type = target.GetType();

        if (type.IsAssignableTo(typeof(IProxy)))
            return (IProxy)target;

        return proxy ??= ProxyFactory.GetCreator(type).Create(target);
    }
}
