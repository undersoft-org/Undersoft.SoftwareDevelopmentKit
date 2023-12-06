using Microsoft.AspNetCore.Mvc;

namespace Undersoft.SDK.Service.Application.Controller.Crud;



public interface ICrudDataRemoteController<TKey, TDto, TModel> where TModel : class, IDataObject
{
    Task<IActionResult> Count();
    Task<IActionResult> Delete([FromBody] TModel[] models);
    Task<IActionResult> Delete([FromRoute] TKey key, [FromBody] TModel model);
    Task<IActionResult> Get();
    Task<IActionResult> Get(int offset, int limit);
    Task<IActionResult> Get(TKey key);
    Task<IActionResult> Patch([FromBody] TModel[] models);
    Task<IActionResult> Patch([FromRoute] TKey key, [FromBody] TModel model);
    Task<IActionResult> Post(int offset, int limit, QuerySet query);
    Task<IActionResult> Post([FromBody] TModel[] models);
    Task<IActionResult> Post([FromRoute] TKey key, [FromBody] TModel model);
    Task<IActionResult> Put([FromBody] TModel[] models);
    Task<IActionResult> Put([FromRoute] TKey key, [FromBody] TModel model);
}