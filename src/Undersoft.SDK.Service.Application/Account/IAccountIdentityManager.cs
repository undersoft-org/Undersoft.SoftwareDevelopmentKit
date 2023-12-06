using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Security.Claims;

namespace Undersoft.SDK.Service.Application.Account
{
    public interface IAccountIdentityManager
    {
        IdentityDbContext<IdentityUser<long>, IdentityRole<long>, long> Context { get; set; }
        RoleManager<IdentityRole<long>> Role { get; set; }
        SignInManager<IdentityUser<long>> SignIn { get; set; }
        AccountIdentityJWTFactory Token { get; set; }
        UserManager<IdentityUser<long>> User { get; set; }

        Task<AccountIdentity> CheckPassword(string email, string password);
        Task<bool> CheckToken(string token);
        Task<bool> Delete(string email);
        Task<AccountIdentity> GetByEmail(string email);
        Task<AccountIdentity> GetById(long id);
        Task<AccountIdentity> GetByName(string name);
        Task<string> GetToken(string email, string password);
        Task<IdentityRole<long>> SetRole(string roleName);
        Task<bool> SetRoleClaim(string roleName, Claim claim);
        Task<AccountIdentity> SetUser(string username, string email, string password, IEnumerable<string> roles);
        Task<bool> SetUserClaim(string email, Claim claim);
        AccountIdentityRole<long> SetUserRole(string email, string current, string previous = null);
        bool TryGetByEmail(string email, out IAccountIdentity<long> account);
        bool TryGetById(long id, out IAccountIdentity<long> account);
        Task<AccountIdentity> MapAccount(AccountIdentity account);

    }
}