using System.Collections.Generic;
using System.Linq;

namespace Undersoft.SDK.Uniques
{
    public static class UniqueOneExtensions
    {
        public static UniqueOne<T> AsUniqueOne<T>(this IQueryable<T> entity) where T : IOrigin
        {
            return new UniqueOne<T>(entity);
        }

        public static UniqueOne<T> AsUniqueOne<T>(this IEnumerable<T> entity) where T : IOrigin
        {
            return new UniqueOne<T>(entity);
        }

        public static UniqueOne<T> AsUniqueOne<T>(this T entity) where T : IOrigin
        {
            return new UniqueOne<T>(entity);
        }
    }
}
