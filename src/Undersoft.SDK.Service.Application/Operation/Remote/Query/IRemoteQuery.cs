using System.Linq.Expressions;


namespace Undersoft.SDK.Service.Application.Operation.Remote.Query;

public interface IRemoteQuery<TDto> : IOperation where TDto : class, IDataObject
{
    Expression<Func<TDto, object>>[] Expanders { get; }
    Expression<Func<TDto, bool>> Predicate { get; }
    SortExpression<TDto> Sort { get; }
}