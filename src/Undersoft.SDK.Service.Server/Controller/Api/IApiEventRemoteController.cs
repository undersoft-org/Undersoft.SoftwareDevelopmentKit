using Microsoft.AspNetCore.Mvc;

namespace Undersoft.SDK.Service.Server.Controller.Crud;
public interface IApiEventRemoteController<TKey, TDto, TModel> where TModel : class, IOrigin, IInnerProxy
{
    Task<IActionResult> Count();
    Task<IActionResult> Delete([FromBody] TModel[] dtos);
    Task<IActionResult> Delete([FromRoute] TKey key, [FromBody] TModel dto);
    Task<IActionResult> Get();
    Task<IActionResult> Get(int offset, int limit);
    Task<IActionResult> Get(TKey key);
    Task<IActionResult> Patch([FromBody] TModel[] dtos);
    Task<IActionResult> Patch([FromRoute] TKey key, [FromBody] TModel dto);
    Task<IActionResult> Post(int offset, int limit, QuerySet query);
    Task<IActionResult> Post([FromBody] TModel[] dtos);
    Task<IActionResult> Post([FromRoute] TKey key, [FromBody] TModel dto);
    Task<IActionResult> Put([FromBody] TModel[] dtos);
    Task<IActionResult> Put([FromRoute] TKey key, [FromBody] TModel dto);
}