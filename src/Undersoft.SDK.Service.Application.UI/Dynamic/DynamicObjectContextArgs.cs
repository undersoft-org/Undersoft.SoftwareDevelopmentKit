namespace Undersoft.SDK.Service.Application.Components;

public class DynamicObjectContextArgs
{
    public DynamicObjectContextArgs(IEnumerable<IDynamicObject> items, DynamicItemChangedType changedType = DynamicItemChangedType.Add)
    {
        Items = items;
        ChangedType = changedType;
    }

    public IEnumerable<IDynamicObject> Items { get; }

    public DynamicItemChangedType ChangedType { get; }
}
