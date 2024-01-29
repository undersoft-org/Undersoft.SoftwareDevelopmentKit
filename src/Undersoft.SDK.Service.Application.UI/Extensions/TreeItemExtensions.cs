namespace Undersoft.SDK.Service.Application.Components;

public static class TreeItemExtensions
{
    public static TreeViewItem<TItem>? FirstOrDefaultActiveItem<TItem>(this IEnumerable<TreeViewItem<TItem>> source)
    {
        var ret = source.FirstOrDefault(item => item.IsActive);
        if (ret == null)
        {
            var items = source.SelectMany(e => e.Items);
            if (items.Any())
            {
                ret = FirstOrDefaultActiveItem(items);
            }
        }
        return ret;
    }

    public static IEnumerable<TreeViewItem<TItem>> GetAllItems<TItem>(this IEnumerable<TreeViewItem<TItem>> source) => GetAllSubItems(source).Union(source);

    public static IEnumerable<TreeViewItem<TItem>> GetAllSubItems<TItem>(this IEnumerable<TreeViewItem<TItem>> source) => source.SelectMany(i => i.Items.Any() ? i.Items.Concat(GetAllSubItems(i.Items)) : i.Items);
}
