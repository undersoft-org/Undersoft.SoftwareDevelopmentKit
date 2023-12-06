using System.Linq.Expressions;
using System.Text.Json.Serialization;
using Undersoft.SDK.Service.Application.Operation.Command;



namespace Undersoft.SDK.Service.Application.Operation.Remote.Command;

public class RemoteCreateSet<TStore, TDto, TModel> : RemoteCommandSet<TModel>
    where TModel : class, IDataObject
    where TDto : class, IDataObject
    where TStore : IDataServiceStore
{
    [JsonIgnore]
    public Func<TDto, Expression<Func<TDto, bool>>> Predicate { get; }

    public RemoteCreateSet(EventPublishMode publishPattern, TModel input, object key)
        : base(
            CommandMode.Create,
            publishPattern,
            new[] { new RemoteCreate<TStore, TDto, TModel>(publishPattern, input, key) }
        )
    { }

    public RemoteCreateSet(EventPublishMode publishPattern, TModel[] inputs)
        : base(
            CommandMode.Create,
            publishPattern,
            inputs
                .Select(input => new RemoteCreate<TStore, TDto, TModel>(publishPattern, input))
                .ToArray()
        )
    { }

    public RemoteCreateSet(
        EventPublishMode publishPattern,
        TModel[] inputs,
        Func<TDto, Expression<Func<TDto, bool>>> predicate
    )
        : base(
            CommandMode.Create,
            publishPattern,
            inputs
                .Select(
                    input => new RemoteCreate<TStore, TDto, TModel>(publishPattern, input, predicate)
                )
                .ToArray()
        )
    {
        Predicate = predicate;
    }
}
