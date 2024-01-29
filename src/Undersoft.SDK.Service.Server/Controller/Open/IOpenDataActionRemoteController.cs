using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;

namespace Undersoft.SDK.Service.Server.Controller.Open;

using Uniques;

public interface IOpenDataActionRemoteController<TStore, TDto, TModel>
    where TDto : class
    where TModel : class
{
    Task<IActionResult> Action([FromBody] ODataActionParameters arguments);
}
