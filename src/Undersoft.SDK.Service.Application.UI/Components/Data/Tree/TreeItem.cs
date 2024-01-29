namespace Undersoft.SDK.Service.Application.Components;

[ExcludeFromCodeCoverage]
public class TreeItem : NodeItem
{
    public bool ShowLoading { get; set; }

    public List<TreeItem> Items { get; set; } = new List<TreeItem>();

    public object? Key { get; set; }

    public object? Tag { get; set; }

    public bool Checked { get; set; }

    public bool HasChildNode { get; set; }

    public IEnumerable<TreeItem> GetAllSubItems() => Items.Concat(GetSubItems(Items));

    private static IEnumerable<TreeItem> GetSubItems(List<TreeItem> items) => items.SelectMany(i => i.Items.Any() ? i.Items.Concat(GetSubItems(i.Items)) : i.Items);

    public void CascadeSetCheck(bool isChecked)
    {
        foreach (var item in Items)
        {
            item.Checked = isChecked;
            if (item.Items.Any())
            {
                item.CascadeSetCheck(isChecked);
            }
        }
    }

}
