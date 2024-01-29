namespace Undersoft.SDK.Service.Application.Components;

public interface IDynamicColumnsObject : IDynamicObject
{
    public Dictionary<string, object?> Columns { get; set; }
}
