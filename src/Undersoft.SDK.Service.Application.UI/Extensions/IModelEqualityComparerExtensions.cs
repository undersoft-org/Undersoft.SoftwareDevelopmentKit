namespace Undersoft.SDK.Service.Application.Components;

public static class IModelEqualityComparerExtensions
{
    public static bool Equals<TItem>(this IModelEqualityComparer<TItem> comparer, TItem? x, TItem? y)
    {
        bool ret;
        if (x == null && y == null)
        {
            ret = true;
        }
        else if (x == null || y == null)
        {
            ret = false;
        }
        else
        {
            ret = comparer.ModelEqualityComparer?.Invoke(x, y)
                ?? Utility.GetKeyValue<TItem, object>(x, comparer.CustomKeyAttribute)?.Equals(Utility.GetKeyValue<TItem, object>(y, comparer.CustomKeyAttribute))
                ?? EqualityComparer();
        }
        return ret;

        bool EqualityComparer()
        {
            bool ret;
            if (x is IEqualityComparer<TItem> comparer)
            {
                ret = comparer.Equals(x, y);
            }
            else
            {
                ret = x.Equals(y);
            }
            return ret;
        }
    }
}
