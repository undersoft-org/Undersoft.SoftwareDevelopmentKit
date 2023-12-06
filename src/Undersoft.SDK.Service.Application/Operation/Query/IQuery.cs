using System.Linq.Expressions;

namespace Undersoft.SDK.Service.Application.Operation.Query;


using Undersoft.SDK.Service.Data.Object;
using Undersoft.SDK.Service.Data.Query;

public interface IQuery<TEntity> : IOperation where TEntity : class, IDataObject
{
    Expression<Func<TEntity, object>>[] Expanders { get; }
    Expression<Func<TEntity, bool>> Predicate { get; }
    SortExpression<TEntity> Sort { get; }
}