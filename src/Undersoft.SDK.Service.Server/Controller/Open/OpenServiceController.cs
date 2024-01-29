using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Undersoft.SDK.Service.Server.Controller.Open;

using Microsoft.AspNetCore.OData.Formatter;
using Undersoft.SDK.Service;
using Undersoft.SDK.Service.Data.Store;
using Undersoft.SDK.Service.Server.Operation.Invocation;
using Undersoft.SDK.Service.Data.Client.Attributes;

[OpenService]
public abstract class OpenServiceController<TStore, TService, TModel>
    : ODataController,
        IOpenServiceController<TStore, TService, TModel>
    where TModel : class, IOrigin, IInnerProxy, new()
    where TService : class
    where TStore : IDataServerStore
{
    protected readonly IServicer _servicer;

    protected OpenServiceController() { }

    public OpenServiceController(IServicer servicer)
    {
        _servicer = servicer;
    }

    [HttpPost]
    public virtual async Task<IActionResult> Access([FromBody] ODataActionParameters arguments)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicer.Perform(
            new Action<TStore, TService, TModel>(
                arguments["Name"].ToString(),
                new Arguments((Dictionary<string, object>)arguments)
            )
        );

        return (!result.IsValid ? BadRequest(result.ErrorMessages) : Ok(result.Response));
    }

    [HttpPost]
    public virtual async Task<IActionResult> Action([FromBody] ODataActionParameters arguments)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicer.Perform(
            new Action<TStore, TService, TModel>(
                arguments["Name"].ToString(),
                new Arguments((Dictionary<string, object>)arguments)
            )
        );

        return (!result.IsValid ? BadRequest(result.ErrorMessages) : Ok(result.Response));
    }

    [HttpPost]
    public virtual async Task<IActionResult> Setup([FromBody] ODataActionParameters arguments)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicer.Perform(
            new Action<TStore, TService, TModel>(
                arguments["Name"].ToString(),
                new Arguments((Dictionary<string, object>)arguments)
            )
        );

        return (!result.IsValid ? BadRequest(result.ErrorMessages) : Ok(result.Response));
    }
}
