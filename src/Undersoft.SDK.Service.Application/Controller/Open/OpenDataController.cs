using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using System.Linq.Expressions;

namespace Undersoft.SDK.Service.Application.Controller.Open;

using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Operation.Command;
using Operation.Query;
using Undersoft.SDK.Service.Data.Event;

using Undersoft.SDK.Service.Data.Object;
using Undersoft.SDK.Uniques;
using SDK.Service.Data.Store;

[OpenDataService]
public abstract class OpenDataController<TKey, TEntry, TReport, TEntity, TDto>
    : ODataController, IOpenDataController<TKey, TEntity, TDto>
    where TDto : class, IDataObject
    where TEntity : class, IDataObject
    where TEntry : IDatabaseStore
    where TReport : IDatabaseStore
{
    protected Func<TKey, Func<TDto, object>> _keysetter = k => e => e.SetId(k);
    protected Func<TKey, Expression<Func<TEntity, bool>>> _keymatcher = k => e => k.Equals(e.Id);
    protected Func<TDto, Expression<Func<TEntity, bool>>> _predicate;
    protected readonly IServicer _servicer;
    protected readonly EventPublishMode _publishMode;

    protected OpenDataController() { }

    protected OpenDataController(
        IServicer servicer,
        EventPublishMode publishMode = EventPublishMode.PropagateCommand
    ) : this(servicer, null, k => e => e.SetId(k), k => e => k.Equals(e.Id), publishMode) { }

    protected OpenDataController(
        IServicer servicer,
        Func<TDto, Expression<Func<TEntity, bool>>> predicate,
        Func<TKey, Func<TDto, object>> keysetter,
        Func<TKey, Expression<Func<TEntity, bool>>> keymatcher,
        EventPublishMode publishMode = EventPublishMode.PropagateCommand
    )
    {
        _keymatcher = keymatcher;
        _keysetter = keysetter;
        _servicer = servicer;
        _publishMode = publishMode;
    }

    [EnableQuery]
    public async Task<IQueryable<TDto>> Get()
    {
        return await _servicer.Send(new GetQuery<TReport, TEntity, TDto>());
    }

    [EnableQuery]
    public async Task<UniqueOne<TDto>> Get([FromODataUri] TKey key)
    {
        return new UniqueOne<TDto>(await _servicer.Send(new FindQuery<TReport, TEntity, TDto>(_keymatcher(key))));
    }

    public async Task<IActionResult> Post(TDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicer 
            .Send(new Create<TEntry, TEntity, TDto>(_publishMode, dto));

        return !result.IsValid ? UnprocessableEntity(result.ErrorMessages.ToArray()) : Ok(result.Id as object);
    }

    public async Task<IActionResult> Patch([FromODataUri] TKey key, TDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _keysetter(key).Invoke(dto);

        var result = await _servicer
            .Send(new Change<TEntry, TEntity, TDto>(_publishMode, dto, _predicate));

        return !result.IsValid ? UnprocessableEntity(result.ErrorMessages.ToArray()) : Ok(result.Id as object);
    }

    public async Task<IActionResult> Put([FromODataUri] TKey key, TDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _keysetter(key).Invoke(dto);

        var result = await _servicer
            .Send(new Update<TEntry, TEntity, TDto>(_publishMode, dto, _predicate));

        return !result.IsValid ? UnprocessableEntity(result.ErrorMessages.ToArray()) : Ok(result.Id as object);
    }

    public async Task<IActionResult> Delete([FromODataUri] TKey key)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicer
            .Send(new Delete<TEntry, TEntity, TDto>(_publishMode, key));

        return !result.IsValid ? UnprocessableEntity(result.ErrorMessages.ToArray()) : Ok(result.Id as object);
    }
}
