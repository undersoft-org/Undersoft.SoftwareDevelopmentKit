namespace Undersoft.SDK.Service.Application.Components;

public class ModelComparer<TItem> : IEqualityComparer<TItem>
{
    private readonly Func<TItem, TItem, bool> _comparer;
    public ModelComparer(Func<TItem, TItem, bool> comparer)
    {
        _comparer = comparer;
    }

    public bool Equals(TItem? x, TItem? y)
    {
        bool ret;
        if (x != null && y != null)
        {
            ret = _comparer(x, y);
        }
        else
        {
            ret = x == null && y == null;
        }
        return ret;
    }

    public int GetHashCode([DisallowNull] TItem obj) => obj.GetHashCode();
}
