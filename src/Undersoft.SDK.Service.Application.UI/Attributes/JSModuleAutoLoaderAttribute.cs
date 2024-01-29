namespace Undersoft.SDK.Service.Application.Components;

[AttributeUsage(AttributeTargets.Class)]
public class JSModuleAutoLoaderAttribute : Attribute
{
    public string? Path { get; }

    public string? ModuleName { get; set; }

    public bool JSObjectReference { get; set; }

    public bool Relative { get; set; } = true;

    public bool AutoInvokeInit { get; set; } = true;

    public bool AutoInvokeDispose { get; set; } = true;

    public JSModuleAutoLoaderAttribute(string? path = null)
    {
        Path = path;
    }
}
