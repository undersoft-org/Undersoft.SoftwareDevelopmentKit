using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using System.Linq.Expressions;

namespace Undersoft.SDK.Service.Server.Controller.Open;

using Operation.Remote.Command;
using Operation.Remote.Query;
using Undersoft.SDK.Service.Data.Event;
using Undersoft.SDK.Service.Data.Store;
using Undersoft.SDK.Service.Data.Client.Attributes;


[OpenDataRemote]
public abstract class OpenDataRemoteController<TKey, TStore, TDto, TModel, TService>
    : OpenServiceRemoteController<TStore, TService, TDto>, IOpenDataRemoteController<TKey, TDto, TModel>
    where TModel : class, IOrigin, IInnerProxy
    where TDto : class, IOrigin, IInnerProxy
    where TStore : IDataServiceStore
    where TService : class

{
    protected Func<TKey, Func<TModel, object>> _keysetter = k => e => e.SetId(k);
    protected Func<TKey, Expression<Func<TDto, bool>>> _keymatcher = k => e => k.Equals(e.Id);
    protected Func<TModel, Expression<Func<TDto, bool>>> _predicate;
    protected readonly EventPublishMode _publishMode = EventPublishMode.PropagateCommand;

    protected OpenDataRemoteController() { }

    protected OpenDataRemoteController(
        IServicer servicer,
        EventPublishMode publishMode = EventPublishMode.PropagateCommand
    ) : this(servicer, null, k => e => e.SetId(k), k => e => k.Equals(e.Id), publishMode) { }

    protected OpenDataRemoteController(
       IServicer servicer,
       Func<TModel, Expression<Func<TDto, bool>>> predicate,
       EventPublishMode publishMode = EventPublishMode.PropagateCommand
   ) : this(servicer, predicate, k => e => e.SetId(k), k => e => k.Equals(e.Id), publishMode) { }

    protected OpenDataRemoteController(
        IServicer servicer,
        Func<TModel, Expression<Func<TDto, bool>>> predicate,
        Func<TKey, Func<TModel, object>> keysetter,
        Func<TKey, Expression<Func<TDto, bool>>> keymatcher,
        EventPublishMode publishMode = EventPublishMode.PropagateCommand
    ) : base(servicer)
    {
        _keymatcher = keymatcher;
        _keysetter = keysetter;
        _publishMode = publishMode;        
    }

    [EnableQuery]
    public virtual IQueryable<TModel> Get()
    {
        return _servicer.Report(new RemoteGetQuery<TStore, TDto, TModel>()).Result;
    }

    [EnableQuery]
    public virtual async Task<UniqueOne<TModel>> Get([FromRoute] TKey key)
    {
        return new UniqueOne<TModel>(await _servicer.Report(new RemoteFindQuery<TStore, TDto, TModel>(_keymatcher(key))));
    }

    public virtual async Task<IActionResult> Post([FromBody] TModel dto)
    {
        bool isValid = false;

        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _servicer.Entry(new RemoteCreateSet<TStore, TDto, TModel>
                                                (_publishMode, new[] { dto }))
                                                    .ConfigureAwait(false);

        var response = result.ForEach(c => (isValid = c.IsValid)
                                              ? c.Id as object
                                              : c.ErrorMessages).ToArray();
        return !isValid
               ? UnprocessableEntity(response)
               : Created(response);
    }

    public virtual async Task<IActionResult> Patch([FromRoute] TKey key, [FromBody] TModel dto)
    {
        bool isValid = false;

        if (!ModelState.IsValid) return BadRequest(ModelState);

        _keysetter(key).Invoke(dto);

        var result = await _servicer.Entry(new RemoteChangeSet<TStore, TDto, TModel>
                                              (_publishMode, new[] { dto }, _predicate))
                                                 .ConfigureAwait(false);

        var response = result.ForEach(c => (isValid = c.IsValid)
                                              ? c.Id as object
                                              : c.ErrorMessages).ToArray();
        return !isValid
               ? UnprocessableEntity(response)
               : Updated(response);
    }

    public virtual async Task<IActionResult> Put([FromRoute] TKey key, [FromBody] TModel dto)
    {
        bool isValid = false;

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _keysetter(key).Invoke(dto);

        var result = await _servicer.Entry(new RemoteUpdateSet<TStore, TDto, TModel>
                                                    (_publishMode, new[] { dto }, _predicate))
                                                        .ConfigureAwait(false);

        var response = result.ForEach(c => (isValid = c.IsValid)
                                              ? c.Id as object
                                              : c.ErrorMessages).ToArray();
        return !isValid
               ? UnprocessableEntity(response)
               : Updated(response);
    }

    public virtual async Task<IActionResult> Delete([FromRoute] TKey key)
    {
        bool isValid = false;

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicer.Entry(new RemoteDeleteSet<TStore, TDto, TModel>
                                                             (_publishMode, key))
                                                                    .ConfigureAwait(false);

        var response = result.ForEach(c => (isValid = c.IsValid)
                                               ? c.Id as object
                                               : c.ErrorMessages).ToArray();
        return !isValid
               ? UnprocessableEntity(response)
               : Ok(response);
    }
}
