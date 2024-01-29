using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Text.Json;

namespace Undersoft.SDK.Service.Server.Controller.Api;
using Operation.Command;
using Operation.Query;
using Undersoft.SDK.Service.Data.Client.Attributes;
using Undersoft.SDK.Service.Data.Event;
using Undersoft.SDK.Service.Data.Object;
using Undersoft.SDK.Service.Data.Query;
using Undersoft.SDK.Service.Data.Repository;
using Undersoft.SDK.Service.Data.Store;

[ApiController]
[ApiData]
[Route($"{StoreRoutes.ApiDataRoute}/[controller]")]
public class ApiCqrsSetController<TKey, TEntry, TReport, TEntity, TDto, TService>
    : ApiDataSetController<TKey, TEntry, TEntity, TDto, TService>
    where TDto : class, IOrigin, IInnerProxy
    where TEntity : class, IOrigin, IInnerProxy
    where TEntry : IDataServerStore
    where TReport : IDataServerStore
    where TService : class
{
    protected ApiCqrsSetController() { }

    protected ApiCqrsSetController(
        IServicer servicer,
        EventPublishMode publishMode = EventPublishMode.PropagateCommand
    ) : this(servicer, null, k => e => e.SetId(k), null, publishMode) { }

    protected ApiCqrsSetController(
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

    public override Func<IRepository<TEntity>, IQueryable<TEntity>> Transformations { get; set; }

    [HttpGet]
    public override async Task<IActionResult> Get([FromHeader] int page, [FromHeader] int limit)
    {
        return Ok(
            await _servicer
                .Report(new Get<TReport, TEntity, TDto>((page - 1) * limit, limit))
                .ConfigureAwait(true)
        );
    }

    [HttpGet("count")]
    public override async Task<IActionResult> Count()
    {
        return Ok(await Task.Run(() => _servicer.use<TReport, TEntity>().Count()));
    }

    [HttpPost("query")]
    public override async Task<IActionResult> Post([FromBody] QuerySet query)
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
                    new Filter<TReport, TEntity, TDto>(
                        0,
                        0,
                        new FilterExpression<TEntity>(query.FilterItems).Create(),
                        new SortExpression<TEntity>(query.SortItems)
                    )
                )
                .ConfigureAwait(false)
        );
    }

    [HttpPost]
    public override async Task<IActionResult> Post([FromBody] TDto[] dtos)
    {
        bool isValid = false;

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicer
            .Entry(
                new CreateSet<TEntry, TEntity, TDto>(_publishMode, dtos)
                {
                    Processings = Transformations
                }
            )
            .ConfigureAwait(false);

        object[] response = result
            .ForEach(c => (isValid = c.IsValid) ? c.Id as object : c.ErrorMessages)
            .ToArray();
        return !isValid ? UnprocessableEntity(response) : Ok(response);
    }

    [HttpPatch]
    public override async Task<IActionResult> Patch([FromBody] TDto[] dtos)
    {
        bool isValid = false;

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicer
            .Entry(
                new ChangeSet<TEntry, TEntity, TDto>(_publishMode, dtos, _predicate)
                {
                    Processings = Transformations
                }
            )
            .ConfigureAwait(false);
        var response = result
            .ForEach(c => (isValid = c.IsValid) ? c.Id as object : c.ErrorMessages)
            .ToArray();
        return !isValid ? UnprocessableEntity(response) : Ok(response);
    }

    [HttpPut]
    public override async Task<IActionResult> Put([FromBody] TDto[] dtos)
    {
        bool isValid = false;

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicer
            .Entry(
                new UpdateSet<TEntry, TEntity, TDto>(_publishMode, dtos, _predicate)
                {
                    Processings = Transformations
                }
            )
            .ConfigureAwait(false);

        var response = result
            .ForEach(c => (isValid = c.IsValid) ? c.Id as object : c.ErrorMessages)
            .ToArray();
        return !isValid ? UnprocessableEntity(response) : Ok(response);
    }

    [HttpDelete]
    public override async Task<IActionResult> Delete([FromBody] TDto[] dtos)
    {
        bool isValid = false;

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicer
            .Entry(
                new DeleteSet<TEntry, TEntity, TDto>(_publishMode, dtos)
                {
                    Processings = Transformations
                }
            )
            .ConfigureAwait(false);

        var response = result
            .ForEach(c => (isValid = c.IsValid) ? c.Id as object : c.ErrorMessages)
            .ToArray();
        return !isValid ? UnprocessableEntity(response) : Ok(response);
    }
}
