using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;


namespace Undersoft.SDK.Service.Server.Controller.Open
{
    public interface IOpenEventRemoteController<TKey, TModel> where TModel : class, IOrigin, IInnerProxy
    {
        Task<IActionResult> Delete([FromODataUri] TKey key);
        IQueryable<TModel> Get();
        Task<UniqueOne<TModel>> Get([FromODataUri] TKey key);
        Task<IActionResult> Patch([FromODataUri] TKey key, TModel dto);
        Task<IActionResult> Post([FromODataBody] TModel dto);
        Task<IActionResult> Put([FromODataUri] TKey key, TModel dto);
    }
}