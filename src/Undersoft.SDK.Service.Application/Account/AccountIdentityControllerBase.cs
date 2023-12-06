using Microsoft.AspNetCore.Authorization;

namespace Undersoft.SDK.Service.Application.Account
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.OData.Formatter;
    using Microsoft.AspNetCore.OData.Routing.Attributes;
    using System.Text;
    using Undersoft.SDK.Service;
    using Undersoft.SDK.Service.Application.Controller.Open;
    using Undersoft.SDK.Service.Application.Operation.Command;
    using Undersoft.SDK.Service.Data;

    [AllowAnonymous]
    [OpenDataActionService]
    [ODataRouteComponent("data/open/[controller]")]
    public abstract class AccountIdentityControllerBase<TStore, TService, TDto>
        : OpenDataActionController<TStore, TService, TDto>
        where TDto : class, IAccountIdentity<long>, new()
        where TService : class
        where TStore : IDataServiceStore
    {
        public AccountIdentityControllerBase(IServicer servicer) : base(servicer) { }

        [HttpPost(nameof(DataActionKind.SignIn))]
        public virtual async Task<IActionResult> SignIn(ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var identityDetails = new TDto()
            {
                Credentials = (Credentials)parameters["credentials"]
            };

            var result = await _servicer.Send(
                new Execute<IIdentityStore, TService, TDto>(
                    DataActionKind.SignIn,
                    identityDetails
                )
            );
            return !result.IsValid
                ? UnprocessableEntity(result.ErrorMessages)
                : Created(result.Id.ToString());
        }

        [HttpPost(nameof(DataActionKind.SignUp))]
        public virtual async Task<IActionResult> SignUp(ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var identityDetails = new TDto()
            {
                Credentials = (Credentials)parameters["credentials"]
            };

            var result = await _servicer
                .Send(
                    new Execute<IIdentityStore, TService, TDto>(
                        DataActionKind.SignUp,
                        identityDetails
                    )
                )
                .ConfigureAwait(false);
            return !result.IsValid
                ? UnprocessableEntity(result.ErrorMessages)
                : Created(result.Id.ToString());
        }

        [HttpPost(nameof(DataActionKind.SetPassword))]
        public virtual async Task<IActionResult> SetPassword(ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _servicer
                .Send(
                    new Execute<IIdentityStore, TService, TDto>(
                        DataActionKind.SetPassword, 
                        new TDto() { Credentials = (Credentials)parameters["credentials"] })
                )
                .ConfigureAwait(false);
            return !result.IsValid
                ? UnprocessableEntity(result.ErrorMessages)
                : Created(result.Id.ToString());
        }

        [HttpPost(nameof(DataActionKind.SetEmail))]
        public virtual async Task<IActionResult> SetEmail(ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _servicer
                .Send(
                    new Execute<IIdentityStore, TService, TDto>(
                        DataActionKind.SetEmail,
                        new TDto() { Credentials = (Credentials)parameters["credentials"] }
                    )
                )
                .ConfigureAwait(false);
            return !result.IsValid
                ? UnprocessableEntity(result.ErrorMessages)
                : Created(result.Id.ToString());
        }

        [HttpPost(nameof(DataActionKind.ConfirmPassword))]
        public virtual async Task<IActionResult> ConfirmPassword(ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _servicer
                .Send(
                    new Execute<IIdentityStore, TService, TDto>(
                        DataActionKind.ConfirmPassword,
                        new TDto() { Credentials = (Credentials)parameters["credentials"] }
                    )
                )
                .ConfigureAwait(false);
            return !result.IsValid
                ? UnprocessableEntity(result.ErrorMessages)
                : Created(result.Id.ToString());
        }

        [HttpPost(nameof(DataActionKind.ConfirmEmail))]
        public virtual async Task<IActionResult> ConfirmEmail(ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _servicer
                .Send(
                    new Execute<IIdentityStore, TService, TDto>(
                        DataActionKind.ConfirmEmail,
                        new TDto() { Credentials = (Credentials)parameters["credentials"] }
                    )
                )
                .ConfigureAwait(false);
            return !result.IsValid
                ? UnprocessableEntity(result.ErrorMessages)
                : Created(result.Id.ToString());
        }

        [HttpPost(nameof(DataActionKind.CompleteRegistration))]
        public virtual async Task<IActionResult> CompleteRegistration(ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _servicer
                .Send(
                    new Execute<IIdentityStore, TService, TDto>(
                        DataActionKind.ConfirmEmail,
                        new TDto() { Credentials = (Credentials)parameters["credentials"] }
                    )
                )
                .ConfigureAwait(false);
            return !result.IsValid
                ? UnprocessableEntity(result.ErrorMessages)
                : Created(result.Id.ToString());
        }

        [HttpPost(nameof(DataActionKind.Token))]
        public virtual async Task<IActionResult> Token(ODataActionParameters parameters)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _servicer
                .Send(
                    new Execute<IIdentityStore, TService, TDto>(
                        DataActionKind.Token,
                        new TDto() { Credentials = (Credentials)parameters["credentials"] })
                )
                .ConfigureAwait(false);
            return !result.IsValid
                ? UnprocessableEntity(result.ErrorMessages)
                : Created(result.Id.ToString());
        }

        [HttpGet(nameof(DataActionKind.Token))]
        public virtual async Task<IActionResult> Get([FromHeader] string authorization)
        {
            var encoding = Encoding.GetEncoding("iso-8859-1");
            authorization = encoding.GetString(Convert.FromBase64String(authorization));
            int separator = authorization.IndexOf(':');

            var identityDetails = new TDto()
            {
                Credentials = new Credentials()
                {
                    Email = authorization.Substring(0, separator),
                    Password = authorization.Substring(separator + 1)
                }
            };

            var result = await _servicer
                .Send(
                    new Execute<IIdentityStore, TService, TDto>(
                        DataActionKind.Token,
                        identityDetails
                    )
                )
                .ConfigureAwait(false);
            return !result.IsValid
                ? Unauthorized(result.ErrorMessages)
                : Ok(result.Output.ToString());
        }
    }
}
