using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;

namespace Undersoft.SDK.Service.Server.Controller.Open;
using Undersoft.SDK.Service.Data.Object;
using Uniques;

public interface IOpenDataRemoteController<TKey, TDto, TModel> where TModel : class, IOrigin
{
    Task<IActionResult> Delete([FromODataUri] TKey key);
    IQueryable<TModel> Get();
    Task<UniqueOne<TModel>> Get([FromODataUri] TKey key);
    Task<IActionResult> Patch([FromODataUri] TKey key, [FromODataBody] TModel model);
    Task<IActionResult> Post([FromODataBody] TModel model);
    Task<IActionResult> Put([FromODataUri] TKey key, [FromODataBody] TModel model);
}