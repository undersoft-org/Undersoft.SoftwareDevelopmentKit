using Undersoft.SDK.Service.Data.Object;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace Undersoft.SDK.Service.Application.Operation.Command;

public class Delete<TStore, TEntity, TDto> : Command<TDto>
    where TEntity : class, IDataObject
    where TDto : class, IDataObject
    where TStore : IDatabaseStore
{
    [JsonIgnore]
    public Func<TEntity, Expression<Func<TEntity, bool>>> Predicate { get; }

    public Delete(EventPublishMode publishPattern, TDto input)
        : base(CommandMode.Delete, publishPattern, input) { }

    public Delete(
        EventPublishMode publishPattern,
        TDto input,
        Func<TEntity, Expression<Func<TEntity, bool>>> predicate
    ) : base(CommandMode.Delete, publishPattern, input)
    {
        Predicate = predicate;
    }

    public Delete(
        EventPublishMode publishPattern,
        Func<TEntity, Expression<Func<TEntity, bool>>> predicate
    ) : base(CommandMode.Delete, publishPattern)
    {
        Predicate = predicate;
    }

    public Delete(EventPublishMode publishPattern, params object[] keys)
        : base(CommandMode.Delete, publishPattern, keys) { }
}
