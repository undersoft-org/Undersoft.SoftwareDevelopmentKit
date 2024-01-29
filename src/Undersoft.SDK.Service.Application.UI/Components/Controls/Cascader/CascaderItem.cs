namespace Undersoft.SDK.Service.Application.Components;

public class CascaderItem
{
    public CascaderItem() { }

    public CascaderItem(string value, string text) => (Value, Text) = (value, text);

    public CascaderItem? Parent { get; private set; }

    public IEnumerable<CascaderItem> Items => _items;

    private readonly List<CascaderItem> _items = new(20);

    public string Text { get; set; } = "";

    public string Value { get; set; } = "";

    public bool HasChildren => _items.Count > 0;

    public void AddItem(CascaderItem item)
    {
        item.Parent = this;
        _items.Add(item);
    }
}
