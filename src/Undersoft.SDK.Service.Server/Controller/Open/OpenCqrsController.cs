using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using System.Linq.Expressions;

namespace Undersoft.SDK.Service.Server.Controller.Open;
using Operation.Command;
using Operation.Query;
using Undersoft.SDK.Service.Data.Event;
using Undersoft.SDK.Service.Data.Object;
using Undersoft.SDK.Service.Data.Store;
using Undersoft.SDK.Uniques;
using Undersoft.SDK.Service.Data.Client.Attributes;


[OpenData]
public abstract class OpenCqrsController<TKey, TEntry, TReport, TEntity, TDto, TService>
    : OpenDataController<TKey, TEntry, TEntity, TDto, TService>
    where TDto : class, IOrigin, IInnerProxy, new()
    where TEntity : class, IOrigin, IInnerProxy
    where TEntry : IDataServerStore
    where TReport : IDataServerStore
    where TService : class
{
    protected OpenCqrsController() { }

    public OpenCqrsController(IServicer servicer) : base(servicer) { }

    protected OpenCqrsController(
        IServicer servicer,
        EventPublishMode publishMode = EventPublishMode.PropagateCommand
    ) : this(servicer, null, k => e => e.SetId(k), k => e => k.Equals(e.Id), publishMode) { }

    protected OpenCqrsController(
        IServicer servicer,
        Func<TDto, Expression<Func<TEntity, bool>>> predicate,
        Func<TKey, Func<TDto, object>> keysetter,
        Func<TKey, Expression<Func<TEntity, bool>>> keymatcher,
        EventPublishMode publishMode = EventPublishMode.PropagateCommand
    ) : base(servicer)
    {
        _keymatcher = keymatcher;
        _keysetter = keysetter;
        _publishMode = publishMode;
    }

    [EnableQuery]
    public override async Task<IQueryable<TDto>> Get()
    {
        return await _servicer.Report(new GetQuery<TReport, TEntity, TDto>());
    }

    [EnableQuery]
    public override async Task<UniqueOne<TDto>> Get([FromRoute] TKey key)
    {
        return new UniqueOne<TDto>(
            await _servicer.Report(new FindQuery<TReport, TEntity, TDto>(_keymatcher(key)))
        );
    }

    public override async Task<IActionResult> Post([FromBody] TDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicer.Entry(new Create<TEntry, TEntity, TDto>(_publishMode, dto));

        return !result.IsValid
            ? UnprocessableEntity(result.ErrorMessages.ToArray())
            : Created(result.Id as object);
    }

    public override async Task<IActionResult> Patch([FromRoute] TKey key, [FromBody] TDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _keysetter(key).Invoke(dto);

        var result = await _servicer.Entry(
            new Change<TEntry, TEntity, TDto>(_publishMode, dto, _predicate)
        );

        return !result.IsValid
            ? UnprocessableEntity(result.ErrorMessages.ToArray())
            : Updated(result.Id as object);
    }

    public override async Task<IActionResult> Put([FromRoute] TKey key, [FromBody] TDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _keysetter(key).Invoke(dto);

        var result = await _servicer.Entry(
            new Update<TEntry, TEntity, TDto>(_publishMode, dto, _predicate)
        );

        return !result.IsValid
            ? UnprocessableEntity(result.ErrorMessages.ToArray())
            : Updated(result.Id as object);
    }

    public override async Task<IActionResult> Delete([FromRoute] TKey key)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicer.Entry(new Delete<TEntry, TEntity, TDto>(_publishMode, key));

        return !result.IsValid
            ? UnprocessableEntity(result.ErrorMessages.ToArray())
            : Ok(result.Id as object);
    }
}
