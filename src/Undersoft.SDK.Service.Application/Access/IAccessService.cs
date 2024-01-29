using Microsoft.AspNetCore.Components.Authorization;
using Undersoft.SDK.Security;

namespace Undersoft.SDK.Service.Application.Access
{
    public interface IAccessService
    {
        Task SignIn(IAuthorization authorization);
        Task SignUp(IAuthorization authorization);
        Task SignOut();
    }
}