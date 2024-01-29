using Microsoft.AspNetCore.Mvc;

namespace Undersoft.SDK.Service.Server.Controller.Crud;
using Undersoft.SDK;
using Undersoft.SDK.Service;
using Undersoft.SDK.Service.Data.Client.Attributes;
using Undersoft.SDK.Service.Data.Store;
using Undersoft.SDK.Service.Server.Operation.Remote.Invocation;


[ApiController]
[ApiServiceRemote]
public abstract class ApiServiceRemoteController<TStore, TService, TModel>
    : ControllerBase,
        IApiServiceRemoteController<TStore, TService, TModel>
    where TModel : class, IOrigin
    where TService : class
    where TStore : IDataServiceStore
{    
    protected readonly IServicer _servicer;    

    protected ApiServiceRemoteController() { }

    protected ApiServiceRemoteController(
        IServicer servicer
    )
    {
        _servicer = servicer;            
    }

    [HttpPost("Action")]
    public virtual async Task<IActionResult> Action([FromBody] Arguments arguments)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicer.Entry(
            new RemoteAction<TStore, TService, TModel>(arguments["Name"].ToString(), arguments)
        );

        return !result.IsValid
            ? BadRequest(result.ErrorMessages)
            : Ok(result.Response);
    }

    [HttpPost("Access")]
    public virtual async Task<IActionResult> Access([FromBody] Arguments arguments)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicer.Entry(
            new RemoteAction<TStore, TService, TModel>(arguments["Name"].ToString(), arguments)
        );

        return !result.IsValid
            ? BadRequest(result.ErrorMessages)
            : Ok(result.Response);
    }

    [HttpPost("Setup")]
    public virtual async Task<IActionResult> Setup([FromBody] Arguments arguments)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _servicer.Entry(
            new RemoteAction<TStore, TService, TModel>(arguments["Name"].ToString(), arguments)
        );

        return !result.IsValid
            ? BadRequest(result.ErrorMessages)
            : Ok(result.Response);
    }
}
