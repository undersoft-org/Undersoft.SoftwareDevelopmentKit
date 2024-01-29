using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Undersoft.SDK.Service.Server.Controller.Open;

using Microsoft.AspNetCore.OData.Formatter;
using Undersoft.SDK.Service;
using Undersoft.SDK.Service.Data.Store;
using Undersoft.SDK.Service.Server.Operation.Remote.Invocation;
using Undersoft.SDK.Service.Data.Client.Attributes;

[OpenServiceRemote]
public abstract class OpenServiceRemoteController<TStore, TService, TDto>
    : ODataController,
        IOpenDataActionRemoteController<TStore, TService, TDto>
    where TService : class
    where TDto : class
    where TStore : IDataServiceStore
{
    protected readonly IServicer _servicer;

    protected OpenServiceRemoteController() { }

    public OpenServiceRemoteController(IServicer servicer)
    {
        _servicer = servicer;
    }

    [HttpPost]
    public virtual async Task<IActionResult> Access([FromBody]
       ODataActionParameters arguments
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicer.Entry(
            new RemoteAction<TStore, TService, TDto>(arguments["Name"].ToString(), new Arguments((Dictionary<string, object>)arguments))
        );

        return (!result.IsValid ? BadRequest(result.ErrorMessages) : Ok(result.Response));
    }

    [HttpPost]
    public virtual async Task<IActionResult> Action([FromBody]
       ODataActionParameters arguments
 )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicer.Entry(
            new RemoteAction<TStore, TService, TDto>(arguments["Name"].ToString(), new Arguments((Dictionary<string, object>)arguments))
        );

        return (!result.IsValid ? BadRequest(result.ErrorMessages) : Ok(result.Response));
    }

    [HttpPost]
    public virtual async Task<IActionResult> Setup([FromBody]
       ODataActionParameters arguments
)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicer.Report(
            new RemoteAction<TStore, TService, TDto>(arguments["Name"].ToString(), new Arguments((Dictionary<string, object>)arguments))
        );

        return (!result.IsValid ? BadRequest(result.ErrorMessages) : Ok(result.Response));
    }
}
