using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System.Linq.Expressions;

namespace Undersoft.SDK.Service.Application.Controller.Open;

using Operation.Remote.Command;
using Operation.Remote.Query;
using Undersoft.SDK.Service.Application.Documentation;
using Undersoft.SDK.Service.Data.Object;

[IgnoreApi]
[RemoteResult]
[RemoteOpenDataService]
public abstract class OpenDataRemoteController<TKey, TStore, TDto, TModel>
    : ODataController, IOpenDataRemoteController<TKey, TDto, TModel> 
    where TModel : class, IDataObject
    where TDto : class, IDataObject
    where TStore : IDataServiceStore
{
    protected Func<TKey, Func<TModel, object>> _keysetter = k => e => e.SetId(k);
    protected Func<TKey, Expression<Func<TDto, bool>>> _keymatcher = k => e => k.Equals(e.Id);
    protected Func<TModel, Expression<Func<TDto, bool>>> _predicate;
    protected readonly IServicer _servicer;
    protected readonly EventPublishMode _publishMode;

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
    )
    {
        _keymatcher = keymatcher;
        _keysetter = keysetter;
        _servicer = servicer;
        _publishMode = publishMode;
    }

    [HttpGet]
    [EnableQuery]
    public virtual IQueryable<TModel> Get()
    {
        return _servicer.Send(new RemoteGetQuery<TStore, TDto, TModel>()).Result;
    }

    [HttpGet]
    [EnableQuery]
    public virtual async Task<UniqueOne<TModel>> Get([FromODataUri] TKey key)
    {
        return new UniqueOne<TModel>(await _servicer.Send(new RemoteFindQuery<TStore, TDto, TModel>(_keymatcher(key))));
    }

    [HttpPost]
    public virtual async Task<IActionResult> Post([FromODataBody] TModel dto)
    {
        bool isValid = false;

        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _servicer.Send(new RemoteCreateSet<TStore, TDto, TModel>
                                                (_publishMode, new[] { dto }))
                                                    .ConfigureAwait(false);

        var response = result.ForEach(c => (isValid = c.IsValid)
                                              ? c.Id as object
                                              : c.ErrorMessages).ToArray();
        return !isValid
               ? UnprocessableEntity(response)
               : Created(response);
    }

    [HttpPatch]
    public virtual async Task<IActionResult> Patch([FromODataUri] TKey key, TModel dto)
    {
        bool isValid = false;

        if (!ModelState.IsValid) return BadRequest(ModelState);

        _keysetter(key).Invoke(dto);

        var result = await _servicer.Send(new RemoteChangeSet<TStore, TDto, TModel>
                                              (_publishMode, new[] { dto }, _predicate))
                                                 .ConfigureAwait(false);

        var response = result.ForEach(c => (isValid = c.IsValid)
                                              ? c.Id as object
                                              : c.ErrorMessages).ToArray();
        return !isValid
               ? UnprocessableEntity(response)
               : Updated(response);
    }

    [HttpPut]
    public virtual async Task<IActionResult> Put([FromODataUri] TKey key, TModel dto)
    {
        bool isValid = false;

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _keysetter(key).Invoke(dto);

        var result = await _servicer.Send(new RemoteUpdateSet<TStore, TDto, TModel>
                                                    (_publishMode, new[] { dto }, _predicate))
                                                        .ConfigureAwait(false);

        var response = result.ForEach(c => (isValid = c.IsValid)
                                              ? c.Id as object
                                              : c.ErrorMessages).ToArray();
        return !isValid
               ? UnprocessableEntity(response)
               : Updated(response);
    }

    [HttpDelete]
    public virtual async Task<IActionResult> Delete([FromODataUri] TKey key)
    {
        bool isValid = false;

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicer.Send(new RemoteDeleteSet<TStore, TDto, TModel>
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
