using System.Linq.Expressions;


namespace Undersoft.SDK.Service.Server.Operation.Remote.Query;

public interface IRemoteQuery<TDto> : IOperation where TDto : class, IOrigin, IInnerProxy
{
    Expression<Func<TDto, object>>[] Expanders { get; }
    Expression<Func<TDto, bool>> Predicate { get; }
    SortExpression<TDto> Sort { get; }
}