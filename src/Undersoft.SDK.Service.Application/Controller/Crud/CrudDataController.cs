using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Text.Json;

namespace Undersoft.SDK.Service.Application.Controller.Crud;
using Data.Service;
using Operation.Command;
using Operation.Query;
using Undersoft.SDK.Service.Data.Event;
using Undersoft.SDK.Service.Data.Object;
using Undersoft.SDK.Service.Data.Query;

[ApiController]
[CrudDataService]
[Route($"{StoreRoutes.CrudDataStore}/[controller]")]
public class CrudDataController<TKey, TEntry, TReport, TEntity, TDto>
    : ControllerBase, ICrudDataController<TKey, TEntity, TDto>
    where TDto : class, IDataObject
    where TEntity : class, IDataObject
    where TEntry : IDatabaseStore
    where TReport : IDatabaseStore
{
    protected Func<TKey, Func<TDto, object>> _keysetter = k => e => e.SetId(k);
    protected Func<TKey, Expression<Func<TEntity, bool>>> _keymatcher;
    protected Func<TDto, Expression<Func<TEntity, bool>>> _predicate;
    protected readonly IServicer _servicer;
    protected readonly EventPublishMode _publishMode;

    protected CrudDataController() { }

    protected CrudDataController(
        IServicer servicer,
        EventPublishMode publishMode = EventPublishMode.PropagateCommand
    ) : this(servicer, null, k => e => e.SetId(k), null, publishMode) { }

    protected CrudDataController(
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

    [HttpGet]
    public virtual async Task<IActionResult> Get()
    {
        return Ok(
            await _servicer.Send(new Get<TReport, TEntity, TDto>(0, 0)).ConfigureAwait(true)
        );
    }

    [HttpGet("count")]
    public virtual async Task<IActionResult> Count()
    {
        return Ok(await Task.Run(() => _servicer.use<TReport, TEntity>().Count()));
    }

    [HttpGet("{key}")]
    public virtual async Task<IActionResult> Get([FromRoute] TKey key)
    {
        Task<TDto> query =
            _keymatcher == null
                ? _servicer.Send(new Find<TReport, TEntity, TDto>(key))
                : _servicer.Send(new Find<TReport, TEntity, TDto>(_keymatcher(key)));

        return Ok(await query.ConfigureAwait(false));
    }

    [HttpGet("{offset}/{limit}")]
    public virtual async Task<IActionResult> Get([FromRoute] int offset, [FromRoute] int limit)
    {
        return Ok(
            await _servicer
                .Send(new Get<TReport, TEntity, TDto>(offset, limit))
                .ConfigureAwait(true)
        );
    }

    [HttpPost("query/{offset}/{limit}")]
    public virtual async Task<IActionResult> Post([FromRoute] int offset, [FromRoute] int limit, [FromBody] QuerySet query)
    {
        query.FilterItems.ForEach(
            (fi) =>
                fi.Value = JsonSerializer.Deserialize(
                    ((JsonElement)fi.Value).GetRawText(),
                    Type.GetType($"System.{fi.Type}", null, null, false, true)
                )
        );

        return Ok(
            await _servicer
                .Send(
                    new Filter<TReport, TEntity, TDto>(offset, limit,
                        new FilterExpression<TEntity>(query.FilterItems).Create(),
                        new SortExpression<TEntity>(query.SortItems)
                    )
                )
                .ConfigureAwait(false)
        );
    }

    [HttpPost]
    public virtual async Task<IActionResult> Post([FromBody] TDto[] dtos)
    {
        bool isValid = false;

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicer.Send(new CreateSet<TEntry, TEntity, TDto>
                                                    (_publishMode, dtos)).ConfigureAwait(false);

        object[] response = result.ForEach(c => (isValid = c.IsValid) ? c.Id as object : c.ErrorMessages)
            .ToArray();
        return !isValid ? UnprocessableEntity(response) : Ok(response);
    }

    [HttpPost("{key}")]
    public virtual async Task<IActionResult> Post([FromRoute] TKey key, [FromBody] TDto dto)
    {
        bool isValid = false;

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _keysetter(key).Invoke(dto);

        var result = await _servicer.Send(new CreateSet<TEntry, TEntity, TDto>
                                                (_publishMode, new[] { dto }))
                                                    .ConfigureAwait(false);

        var response = result.ForEach(c => (isValid = c.IsValid)
                                              ? c.Id as object
                                              : c.ErrorMessages).ToArray();
        return !isValid
               ? UnprocessableEntity(response)
               : Ok(response);
    }

    [HttpPatch]
    public virtual async Task<IActionResult> Patch([FromBody] TDto[] dtos)
    {
        bool isValid = false;

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicer.Send(new ChangeSet<TEntry, TEntity, TDto>
                                                                (_publishMode, dtos, _predicate))
                                                                    .ConfigureAwait(false);
        var response = result.ForEach(c => (isValid = c.IsValid)
                                              ? c.Id as object
                                              : c.ErrorMessages).ToArray();
        return !isValid
               ? UnprocessableEntity(response)
               : Ok(response);
    }

    [HttpPatch("{key}")]
    public virtual async Task<IActionResult> Patch([FromRoute] TKey key, [FromBody] TDto dto)
    {
        bool isValid = false;

        if (!ModelState.IsValid) return BadRequest(ModelState);

        _keysetter(key).Invoke(dto);

        var result = await _servicer.Send(new ChangeSet<TEntry, TEntity, TDto>
                                              (_publishMode, new[] { dto }, _predicate))
                                                 .ConfigureAwait(false);

        var response = result.ForEach(c => (isValid = c.IsValid)
                                              ? c.Id as object
                                              : c.ErrorMessages).ToArray();
        return !isValid
               ? UnprocessableEntity(response)
               : Ok(response);
    }

    [HttpPut]
    public virtual async Task<IActionResult> Put([FromBody] TDto[] dtos)
    {
        bool isValid = false;

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicer.Send(new UpdateSet<TEntry, TEntity, TDto>
                                                                    (_publishMode, dtos, _predicate))
                                                                                .ConfigureAwait(false);

        var response = result.ForEach(c => (isValid = c.IsValid) ? c.Id as object : c.ErrorMessages)
            .ToArray();
        return !isValid ? UnprocessableEntity(response) : Ok(response);
    }

    [HttpPut("{key}")]
    public virtual async Task<IActionResult> Put([FromRoute] TKey key, [FromBody] TDto dto)
    {
        bool isValid = false;

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _keysetter(key).Invoke(dto);

        var result = await _servicer.Send(new UpdateSet<TEntry, TEntity, TDto>
                                                    (_publishMode, new[] { dto }, _predicate))
                                                        .ConfigureAwait(false);

        var response = result.ForEach(c => (isValid = c.IsValid)
                                              ? c.Id as object
                                              : c.ErrorMessages).ToArray();
        return !isValid
               ? UnprocessableEntity(response)
               : Ok(response);
    }

    [HttpDelete]
    public virtual async Task<IActionResult> Delete([FromBody] TDto[] dtos)
    {
        bool isValid = false;

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicer.Send(new DeleteSet<TEntry, TEntity, TDto>
                                                            (_publishMode, dtos))
                                                             .ConfigureAwait(false);

        var response = result.ForEach(c => (isValid = c.IsValid)
                                                   ? c.Id as object
                                                   : c.ErrorMessages).ToArray();
        return !isValid
               ? UnprocessableEntity(response)
               : Ok(response);
    }

    [HttpDelete("{key}")]
    public virtual async Task<IActionResult> Delete([FromRoute] TKey key, [FromBody] TDto dto)
    {
        bool isValid = false;

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _keysetter(key).Invoke(dto);

        var result = await _servicer.Send(new DeleteSet<TEntry, TEntity, TDto>
                                                             (_publishMode, new[] { dto }))
                                                                    .ConfigureAwait(false);

        var response = result.ForEach(c => (isValid = c.IsValid)
                                               ? c.Id as object
                                               : c.ErrorMessages).ToArray();
        return !isValid
               ? UnprocessableEntity(response)
               : Ok(response);
    }
}
