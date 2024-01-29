using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Text.Json;

namespace Undersoft.SDK.Service.Server.Controller.Crud;

using Operation.Remote.Command;
using Operation.Remote.Query;
using Undersoft.SDK.Service.Data.Client.Attributes;
using Undersoft.SDK.Service.Data.Event;
using Undersoft.SDK.Service.Data.Store;
using Undersoft.SDK.Service.Server.Operation.Remote;

[RemoteResult]
[ApiDataRemote]
[ApiController]
[Route($"{StoreRoutes.ApiDataRoute}/[controller]")]
public abstract class ApiDataRemoteController<TKey, TStore, TDto, TModel, TService>
    : ApiServiceRemoteController<TStore, TService, TModel>,
        IApiDataRemoteController<TKey, TDto, TModel>
    where TModel : class, IOrigin, IInnerProxy
    where TDto : class, IOrigin, IInnerProxy
    where TStore : IDataServiceStore
    where TService : class
{
    protected Func<TKey, Func<TModel, object>> _keysetter = k => e => e.SetId(k);
    protected Func<TKey, Expression<Func<TDto, bool>>> _keymatcher;
    protected Func<TModel, Expression<Func<TDto, bool>>> _predicate;
    protected readonly EventPublishMode _publishMode;

    protected ApiDataRemoteController() { }

    protected ApiDataRemoteController(
        IServicer servicer,
        EventPublishMode publishMode = EventPublishMode.PropagateCommand
    ) : this(servicer, null, k => e => e.SetId(k), null, publishMode) { }

    protected ApiDataRemoteController(
        IServicer servicer,
        Func<TModel, Expression<Func<TDto, bool>>> predicate
    ) : this(servicer, predicate, k => e => e.SetId(k), null, EventPublishMode.PropagateCommand) { }

    protected ApiDataRemoteController(
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

    [HttpGet]
    public virtual async Task<IActionResult> Get([FromHeader] int page, [FromHeader] int limit)
    {
        return Ok(
            await _servicer.Report(new RemoteGet<TStore, TDto, TModel>((page - 1) * limit, limit))
        );
    }

    [HttpGet("count")]
    public virtual async Task<IActionResult> Count()
    {
        return Ok(await Task.Run(() => _servicer.load<TStore, TDto>().Count()));
    }

    [HttpGet("{key}")]
    public virtual async Task<IActionResult> Get(TKey key)
    {
        return Ok(
           _keymatcher == null
               ? await _servicer.Report(new RemoteFind<TStore, TDto, TModel>(key)).ConfigureAwait(false)
               : await _servicer.Report(new RemoteFind<TStore, TDto, TModel>(_keymatcher(key))).ConfigureAwait(false));
    }

    [HttpPost("query")]
    public virtual async Task<IActionResult> Post([FromBody] QuerySet query)
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
                .Report(
                    new RemoteFilter<TStore, TDto, TModel>(
                        0,
                        0,
                        new FilterExpression<TDto>(query.FilterItems).Create(),
                        new SortExpression<TDto>(query.SortItems)
                    )
                )
                .ConfigureAwait(false)
        );
    }

    [HttpPost]
    public virtual async Task<IActionResult> Post([FromBody] TModel[] dtos)
    {
        bool isValid = false;

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicer
            .Entry(new RemoteCreateSet<TStore, TDto, TModel>(_publishMode, dtos))
            .ConfigureAwait(false);

        object[] response = result
            .ForEach(c => (isValid = c.IsValid) ? c.Id as object : c.ErrorMessages)
            .ToArray();
        return !isValid ? UnprocessableEntity(response) : Ok(response);
    }

    [HttpPost("{key}")]
    public virtual async Task<IActionResult> Post([FromRoute] TKey key, [FromBody] TModel dto)
    {
        bool isValid = false;

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _keysetter(key).Invoke(dto);

        var result = await _servicer
            .Entry(new RemoteCreateSet<TStore, TDto, TModel>(_publishMode, new[] { dto }))
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
            .Entry(new RemoteChangeSet<TStore, TDto, TModel>(_publishMode, dtos, _predicate))
            .ConfigureAwait(false);
        var response = result
            .ForEach(c => (isValid = c.IsValid) ? c.Id as object : c.ErrorMessages)
            .ToArray();
        return !isValid ? UnprocessableEntity(response) : Ok(response);
    }

    [HttpPatch("{key}")]
    public virtual async Task<IActionResult> Patch([FromRoute] TKey key, [FromBody] TModel dto)
    {
        bool isValid = false;

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _keysetter(key).Invoke(dto);

        var result = await _servicer
            .Entry(
                new RemoteChangeSet<TStore, TDto, TModel>(_publishMode, new[] { dto }, _predicate)
            )
            .ConfigureAwait(false);

        var response = result
            .ForEach(c => (isValid = c.IsValid) ? c.Id as object : c.ErrorMessages)
            .ToArray();
        return !isValid ? UnprocessableEntity(response) : Ok(response);
    }

    [HttpPut]
    public virtual async Task<IActionResult> Put([FromBody] TModel[] dtos)
    {
        bool isValid = false;

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicer
            .Entry(new RemoteUpdateSet<TStore, TDto, TModel>(_publishMode, dtos, _predicate))
            .ConfigureAwait(false);

        var response = result
            .ForEach(c => (isValid = c.IsValid) ? c.Id as object : c.ErrorMessages)
            .ToArray();
        return !isValid ? UnprocessableEntity(response) : Ok(response);
    }

    [HttpPut("{key}")]
    public virtual async Task<IActionResult> Put([FromRoute] TKey key, [FromBody] TModel dto)
    {
        bool isValid = false;

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _keysetter(key).Invoke(dto);

        var result = await _servicer
            .Entry(
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
            .Entry(new RemoteDeleteSet<TStore, TDto, TModel>(_publishMode, dtos))
            .ConfigureAwait(false);

        var response = result
            .ForEach(c => (isValid = c.IsValid) ? c.Id as object : c.ErrorMessages)
            .ToArray();
        return !isValid ? UnprocessableEntity(response) : Ok(response);
    }

    [HttpDelete("{key}")]
    public virtual async Task<IActionResult> Delete([FromRoute] TKey key, [FromBody] TModel dto)
    {
        bool isValid = false;

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _keysetter(key).Invoke(dto);

        var result = await _servicer
            .Entry(new RemoteDeleteSet<TStore, TDto, TModel>(_publishMode, new[] { dto }))
            .ConfigureAwait(false);

        var response = result
            .ForEach(c => (isValid = c.IsValid) ? c.Id as object : c.ErrorMessages)
            .ToArray();
        return !isValid ? UnprocessableEntity(response) : Ok(response);
    }
}
