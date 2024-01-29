using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Text.Json;

namespace Undersoft.SDK.Service.Server.Controller.Api;

using Operation.Command;
using Operation.Query;
using Undersoft.SDK.Service.Data.Client.Attributes;
using Undersoft.SDK.Service.Data.Event;
using Undersoft.SDK.Service.Data.Query;
using Undersoft.SDK.Service.Data.Repository;
using Undersoft.SDK.Service.Data.Store;
using Undersoft.SDK.Service.Server.Controller.Crud;

[ApiController]
[ApiData]
[Route($"{StoreRoutes.ApiDataRoute}/[controller]")]
public class ApiDataSetController<TKey, TStore, TEntity, TDto, TService>
    : ApiServiceController<TStore, TService, TDto>, IApiDataSetController<TKey, TEntity, TDto> 
    where TDto : class, IOrigin, IInnerProxy
    where TEntity : class, IOrigin, IInnerProxy
    where TStore : IDataServerStore
    where TService : class
{
    protected Func<TKey, Func<TDto, object>> _keysetter = k => e => e.SetId(k);
    protected Func<TKey, Expression<Func<TEntity, bool>>> _keymatcher;
    protected Func<TDto, Expression<Func<TEntity, bool>>> _predicate;
    protected IServicer _servicer;
    protected EventPublishMode _publishMode;

    protected ApiDataSetController() { }

    protected ApiDataSetController(
        IServicer servicer,
        EventPublishMode publishMode = EventPublishMode.PropagateCommand
    ) : this(servicer, null, k => e => e.SetId(k), null, publishMode) { }

    protected ApiDataSetController(
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

    public virtual Func<IRepository<TEntity>, IQueryable<TEntity>> Transformations { get; set; }

    [HttpGet]
    public virtual async Task<IActionResult> Get([FromHeader] int page, [FromHeader] int limit)
    {
        return Ok(
            await _servicer.Report(new Get<TStore, TEntity, TDto>((page - 1) * limit, limit)).ConfigureAwait(true)
        );
    }

    [HttpGet("count")]
    public virtual async Task<IActionResult> Count()
    {
        return Ok(await Task.Run(() => _servicer.use<TStore, TEntity>().Count()));
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
                .Entry(
                    new Filter<TStore, TEntity, TDto>(0, 0,
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

        var result = await _servicer.Entry(new CreateSet<TStore, TEntity, TDto>
                                                    (_publishMode, dtos)).ConfigureAwait(false);

        object[] response = result.ForEach(c => (isValid = c.IsValid) ? c.Id as object : c.ErrorMessages)
            .ToArray();
        return !isValid ? UnprocessableEntity(response) : Ok(response);
    }

    [HttpPatch]
    public virtual async Task<IActionResult> Patch([FromBody] TDto[] dtos)
    {
        bool isValid = false;

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicer.Entry(new ChangeSet<TStore, TEntity, TDto>
                                                                (_publishMode, dtos, _predicate))
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

        var result = await _servicer.Entry(new UpdateSet<TStore, TEntity, TDto>
                                                                    (_publishMode, dtos, _predicate))
                                                                                .ConfigureAwait(false);

        var response = result.ForEach(c => (isValid = c.IsValid) ? c.Id as object : c.ErrorMessages)
            .ToArray();
        return !isValid ? UnprocessableEntity(response) : Ok(response);
    }

    [HttpDelete]
    public virtual async Task<IActionResult> Delete([FromBody] TDto[] dtos)
    {
        bool isValid = false;

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicer.Entry(new DeleteSet<TStore, TEntity, TDto>
                                                            (_publishMode, dtos))
                                                             .ConfigureAwait(false);

        var response = result.ForEach(c => (isValid = c.IsValid)
                                                   ? c.Id as object
                                                   : c.ErrorMessages).ToArray();
        return !isValid
               ? UnprocessableEntity(response)
               : Ok(response);
    }
}
