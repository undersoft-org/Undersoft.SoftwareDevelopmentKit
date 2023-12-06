using Microsoft.AspNetCore.Mvc;

namespace Undersoft.SDK.Service.Application.Controller.Crud;

using Undersoft.SDK;


public interface ICrudDataActionRemoteController<TStore, TDto, TModel> where TDto : class, IOrigin where TModel : class, IOrigin
{
    Task<IActionResult> Post([FromRoute] string kind, [FromBody] TModel dto);
}