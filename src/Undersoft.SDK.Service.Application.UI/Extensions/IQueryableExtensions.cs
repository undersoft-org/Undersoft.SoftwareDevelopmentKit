using System.Linq.Expressions;

namespace Undersoft.SDK.Service.Application.Components;

public static class IQueryableExtensions
{
    public static IQueryable<T> Where<T>(this IQueryable<T> queryable, Expression<Func<T, bool>> predicate, bool condition) => condition ? queryable.Where(predicate) : queryable;

    public static IQueryable<T> Sort<T>(this IQueryable<T> queryable, string sortName, SortOrder sortOrder, bool condition) => condition ? queryable.Sort(sortName, sortOrder) : queryable;

    public static IQueryable<T> Page<T>(this IQueryable<T> queryable, int skipCount, int maxResultCount) => queryable.Skip(skipCount).Take(maxResultCount);

    public static IQueryable<T> Count<T>(this IQueryable<T> queryable, out int totalCount)
    {
        totalCount = queryable.Count();
        return queryable;
    }
}
