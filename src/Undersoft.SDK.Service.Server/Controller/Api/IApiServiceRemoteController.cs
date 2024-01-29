using Microsoft.AspNetCore.Mvc;

namespace Undersoft.SDK.Service.Server.Controller.Crud;

using System.Text.Json;
using Undersoft.SDK;


public interface IApiServiceRemoteController<TStore, TService, TModel> where TModel : class, IOrigin where TService : class
{
    Task<IActionResult> Action([FromBody] Arguments arguments);

    Task<IActionResult> Access([FromBody] Arguments arguments);

    Task<IActionResult> Setup([FromBody] Arguments arguments);
}