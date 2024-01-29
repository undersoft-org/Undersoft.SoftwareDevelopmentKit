using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;

namespace Undersoft.SDK.Service.Server.Controller.Open;

using Uniques;

public interface IOpenServiceController<TKey, TService, TDto> where TDto : class
{
    Task<IActionResult> Action([FromBody] ODataActionParameters parameters);

    Task<IActionResult> Access([FromBody] ODataActionParameters parameters);

    Task<IActionResult> Setup([FromBody] ODataActionParameters parameters);
}