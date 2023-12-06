using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Text.Json;

namespace Undersoft.SDK.Service.Application.Controller.Crud;

using Operation.Remote.Command;
using Operation.Remote.Query;
using Data.Query;
using Undersoft.SDK.Service.Data.Event;
using SDK.Service.Data.Store;

[RemoteResult]
[RemoteCrudDataService]
[ApiController]
[Route($"{StoreRoutes.CrudEventStore}/Events")]
public abstract class CrudEventRemoteController<TKey, TStore, TDto, TModel>
    : ControllerBase,
        ICrudEventRemoteController<TKey, TDto, TModel>
    where TDto : class, IDataObject
    where TModel : class, IDataObject
    where TStore : IDataServiceStore
{
    protected Func<TKey, Func<TModel, object>> _keysetter = k => e => e.SetId(k);
    protected Func<TKey, Expression<Func<TDto, bool>>> _keymatcher = k => e => k.Equals(e.Id);
    protected Func<TModel, Expression<Func<TDto, bool>>> _predicate;
    protected readonly IServicer _servicer;
    protected readonly EventPublishMode _publishMode;

    protected CrudEventRemoteController(IServicer servicer)
    {
        _servicer = servicer;
    }

    protected CrudEventRemoteController(
        IServicer servicer,
        Func<TKey, Expression<Func<TDto, bool>>> keymatcher
    )
    {
        _servicer = servicer;
        _keymatcher = keymatcher;
    }

    [HttpGet("count")]
    public virtual async Task<IActionResult> Count()
    {
        return Ok(await Task.Run(() => _servicer.load<TStore, TDto>().Count()));
    }

    [HttpGet]
    public virtual async Task<IActionResult> Get()
    {
        return Ok(
            await _servicer.Send(new RemoteGet<TStore, TDto, TModel>(0, 0)).ConfigureAwait(true)
        );
    }

    [HttpGet("{key}")]
    public virtual async Task<IActionResult> Get(TKey key)
    {
        Task<TModel> query =
            _keymatcher == null
                ? _servicer.Send(new RemoteFind<TStore, TDto, TModel>(key))
                : _servicer.Send(new RemoteFind<TStore, TDto, TModel>(_keymatcher(key)));

        return Ok(await query.ConfigureAwait(false));
    }

    [HttpGet("{offset}/{limit}")]
    public virtual async Task<IActionResult> Get(int offset, int limit)
    {
        return Ok(
            await _servicer
                .Send(new RemoteGet<TStore, TDto, TModel>(offset, limit))
                .ConfigureAwait(true)
        );
    }

    [HttpPost]
    public virtual async Task<IActionResult> Post([FromBody] TModel[] dtos)
    {
        bool isValid = false;

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicer
            .Send(new RemoteCreateSet<TStore, TDto, TModel>(_publishMode, dtos))
            .ConfigureAwait(false);

        object[] response = result
            .ForEach(c => (isValid = c.IsValid) ? c.Id as object : c.ErrorMessages)
            .ToArray();
        return !isValid ? UnprocessableEntity(response) : Ok(response);
    }

    [HttpPost("query/{offset}/{limit}")]
    public virtual async Task<IActionResult> Post(int offset, int limit, QuerySet query)
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
                    new RemoteFilter<TStore, TDto, TModel>(
                        offset,
                        limit,
                        new FilterExpression<TDto>(query.FilterItems).Create(),
                        new SortExpression<TDto>(query.SortItems)
                    )
                )
                .ConfigureAwait(false)
        );
    }

    [HttpPost("{key}")]
    public virtual async Task<IActionResult> Post(TKey key, [FromBody] TModel dto)
    {
        bool isValid = false;

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _keysetter(key).Invoke(dto);

        var result = await _servicer
            .Send(new RemoteCreateSet<TStore, TDto, TModel>(_publishMode, new[] { dto }))
            .ConfigureAwait(false);

        var response = result
            .ForEach(c => (isValid = c.IsValid) ? c.Id as object : c.ErrorMessages)
            .ToArray();
        return !isValid ? UnprocessableEntity(response) : Ok(response);
    }

    [HttpPatch]
    public virtual async Task<IActionResult> Patch([FromBody] TModel[] dtos)
    {
        bool isValid = false;

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicer
            .Send(
                new RemoteChangeSet<TStore, TDto, TModel>(EventPublishMode.PropagateCommand, dtos)
            )
            .ConfigureAwait(false);

        object[] response = result
            .ForEach(c => (isValid = c.IsValid) ? c.Id as object : c.ErrorMessages)
            .ToArray();
        return !isValid ? UnprocessableEntity(response) : Ok(response);
    }

    [HttpPatch("{key}")]
    public virtual async Task<IActionResult> Patch(TKey key, [FromBody] TModel dto)
    {
        bool isValid = false;

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicer
            .Send(
                new RemoteChange<TStore, TDto, TModel>(EventPublishMode.PropagateCommand, dto, key)
            )
            .ConfigureAwait(false);

        object response = result.IsValid ? result.Id as object : result.ErrorMessages;
        return !isValid ? UnprocessableEntity(response) : Ok(response);
    }

    [HttpPut]
    public virtual async Task<IActionResult> Put([FromBody] TModel[] dtos)
    {
        bool isValid = false;

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicer
            .Send(new RemoteUpdateSet<TStore, TDto, TModel>(_publishMode, dtos, _predicate))
            .ConfigureAwait(false);

        var response = result
            .ForEach(c => (isValid = c.IsValid) ? c.Id as object : c.ErrorMessages)
            .ToArray();
        return !isValid ? UnprocessableEntity(response) : Ok(response);
    }

    [HttpPut("{key}")]
    public virtual async Task<IActionResult> Put(TKey key, [FromBody] TModel dto)
    {
        bool isValid = false;

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _keysetter(key).Invoke(dto);

        var result = await _servicer
            .Send(
                new RemoteUpdateSet<TStore, TDto, TModel>(_publishMode, new[] { dto }, _predicate)
            )
            .ConfigureAwait(false);

        var response = result
            .ForEach(c => (isValid = c.IsValid) ? c.Id as object : c.ErrorMessages)
            .ToArray();
        return !isValid ? UnprocessableEntity(response) : Ok(response);
    }

    [HttpDelete]
    public virtual async Task<IActionResult> Delete([FromBody] TModel[] dtos)
    {
        bool isValid = false;

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicer
            .Send(
                new RemoteDeleteSet<TStore, TDto, TModel>(EventPublishMode.PropagateCommand, dtos)
            )
            .ConfigureAwait(false);

        object[] response = result
            .ForEach(c => (isValid = c.IsValid) ? c.Id as object : c.ErrorMessages)
            .ToArray();
        return !isValid ? UnprocessableEntity(response) : Ok(response);
    }

    [HttpDelete("{key}")]
    public virtual async Task<IActionResult> Delete(TKey key, [FromBody] TModel dto)
    {
        bool isValid = false;

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _keysetter(key).Invoke(dto);

        var result = await _servicer
            .Send(
                new RemoteDeleteSet<TStore, TDto, TModel>(
                    EventPublishMode.PropagateCommand,
                    new[] { dto }
                )
            )
            .ConfigureAwait(false);

        var response = result
            .ForEach(c => (isValid = c.IsValid) ? c.Id as object : c.ErrorMessages)
            .ToArray();
        return !isValid ? UnprocessableEntity(response) : Ok(response);
    }
}
