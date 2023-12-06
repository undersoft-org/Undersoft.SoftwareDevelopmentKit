using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Undersoft.SDK.Logging;
using Undersoft.SDK.Series;
using Undersoft.SDK.Uniques;
using System.IdentityModel.Claims;
using System.Security;
using System.Security.Claims;
using Claim = System.Security.Claims.Claim;

namespace Undersoft.SDK.Service.Application.Account;

public class AccountIdentityManager : TypedRegistry<IAccountIdentity<long>>, IAccountIdentityManager
{
    public AccountIdentityManager() { }

    public AccountIdentityManager(
        IdentityDbContext<IdentityUser<long>, IdentityRole<long>, long> context,
        UserManager<IdentityUser<long>> user,
        RoleManager<IdentityRole<long>> role,
        SignInManager<IdentityUser<long>> signIn,
        AccountIdentityJWTFactory token
    )
    {
        Context = context;
        User = user;
        Role = role;
        SignIn = signIn;
        Token = token;
    }

    public IdentityDbContext<IdentityUser<long>, IdentityRole<long>, long> Context { get; set; }

    public UserManager<IdentityUser<long>> User { get; set; }

    public RoleManager<IdentityRole<long>> Role { get; set; }

    public SignInManager<IdentityUser<long>> SignIn { get; set; }

    public AccountIdentityJWTFactory Token { get; set; }

    public async Task<string> GetToken(string email, string password)
    {
        string token = null;
        var account = await CheckPassword(email, password);
        if (account != null)
            token = Token.Generate(account.Claims.Select(c => c.Claim));
        return token;
    }

    public async Task<bool> CheckToken(string token)
    {
        return (await Token.Validate(token)).IsValid;
    }

    public async Task<AccountIdentity> CheckPassword(string email, string password)
    {
        if (!this.TryGetByEmail(email, out IAccountIdentity<long> account))
            return null;
        if (account.Info != null && await User.CheckPasswordAsync(account.Info, password))
            return (AccountIdentity)account;
        return null;
    }

    public async Task<AccountIdentity> SetUser(string username, string email, string password, IEnumerable<string> roles)
    {
        if (!TryGetByEmail(email, out IAccountIdentity<long> account))
        {
            account = new AccountIdentity();
            account.Id = (long)email.UniqueKey64();
            account.Info.Id = account.Id;
            account.Info.Email = email;
            account.Info.UserName = username;
            if (!(await User.CreateAsync(account.Info, password)).Succeeded)
                return null;
        }
        await User.AddToRolesAsync(account.Info, roles);
        await User.AddClaimsAsync(
            account.Info,
            new Claim[]
            {
                new Claim(JwtClaimTypes.Id, account.Id.ToString()),
                new Claim(JwtClaimTypes.Email, account.Info.Email),
                new Claim("code_no", account.CodeNo),
                new Claim(JwtClaimTypes.ClientId, "")
            }.Concat(roles.Select(r => new Claim("role", r)))
        );
        var _account = (AccountIdentity)account;
        await MapAccount(_account);
        this.Put(_account);
        return _account;
    }

    public async Task<bool> Delete(string email)
    {
        if (!TryGetByEmail(email, out var account))
            return false;
        await User.DeleteAsync(account.Info);
        this.Remove(account.Id);
        return true;
    }

    public AccountIdentityRole<long> SetUserRole(string email, string current, string previous = null)
    {
        if (!TryGetByEmail(email, out var account))
            return null;

        var currentRoleId = (int)(email + current).UniqueKey32();

        var role = account.Roles.Put(currentRoleId, new AccountIdentityRole<long>() { Info = new IdentityRole<long>(current) { Id = currentRoleId } });
        if (role != null)
        {
            User.AddToRoleAsync(account.Info, current);
            if ((previous != null))
            {
                role.Value = account.Roles.Remove(previous);
                User.RemoveFromRoleAsync(account.Info, current);
            }
        }
        return role.Value;
    }

    public async Task<bool> SetUserClaim(string email, Claim claim)
    {
        if (!TryGetByEmail(email, out var account))
            return false;
        var id = (int)(email + claim.Type).UniqueKey32();
        var _claim = account.Claims.Put(id, new AccountIdentityClaim<long>()
        {
            Info = new IdentityUserClaim<long>() { ClaimType = claim.Type, ClaimValue = claim.Value, Id = id, UserId = account.Id }
        });
        if (_claim != null)
        {
            await User.RemoveClaimAsync(account.Info, claim);
            var result = await User.AddClaimAsync(account.Info, claim);
            if (result.Succeeded)
                return true;
        }
        return false;
    }

    public async Task<IdentityRole<long>> SetRole(string roleName)
    {
        var role = Role.Roles.Where(r => r.Name == roleName).FirstOrDefault();
        if (role == null)
        {
            role = new IdentityRole<long>(roleName);
            await Role.CreateAsync(role);
        }
        return role;
    }

    public async Task<bool> SetRoleClaim(string roleName, Claim claim)
    {
        var role = await SetRole(roleName);
        var roleclaims = await Role.GetClaimsAsync(role);
        var toset = roleclaims.Where(rc => rc.Type == claim.Type & rc.Value != claim.Value).FirstOrDefault();
        if (toset != null)
            await Role.RemoveClaimAsync(role, toset);
        return (await Role.AddClaimAsync(role, claim)).Succeeded;
    }

    public bool TryGetByEmail(string email, out IAccountIdentity<long> account)
    {
        return TryGetById((long)email.UniqueKey64(), out account);
    }

    public bool TryGetById(long id, out IAccountIdentity<long> account)
    {
        if (this.TryGet(id, out account))
            return true;

        var accountTask = GetById(id);
        accountTask.Wait(100 * 100);
        account = accountTask.Result;

        if (account == null)
            return false;

        return true;
    }

    public async Task<AccountIdentity> GetByName(string name)
    {
        var user = await User.FindByNameAsync(name);
        if (user != null)
            return await GetById(user.Id);
        return null;
    }

    public async Task<AccountIdentity> GetByEmail(string email)
    {
        return await GetById((long)email.UniqueKey64());
    }

    public async Task<AccountIdentity> GetById(long id)
    {
        if (this.TryGet((ulong)id, out IAccountIdentity<long> account))
            return (AccountIdentity)account;
        var _account = new AccountIdentity();
        _account.Id = id;
        _account.Info = await User.FindByIdAsync(_account.Id.ToString());
        await MapAccount(_account);
        this.Put(_account);
        return _account;
    }

    public async Task<AccountIdentity> MapAccount(AccountIdentity account)
    {
        if (account.Info != null)
        {
            account.Credentials.MapUser(account.Info);
            account.Roles = (await User.GetRolesAsync(account.Info))
                .Select(async r => await Role.FindByNameAsync(r))
                .Select(t => t.Result)
                .ToList()
                .Select(
                    async r =>
                        new AccountIdentityRole<long>()
                        {
                            Info = r,
                            Claims = (await Role.GetClaimsAsync(r))
                                .Select(
                                    c =>
                                        new AccountIdentityRoleClaim<long>()
                                        {
                                            Info = new IdentityRoleClaim<long>()
                                            {
                                                ClaimType = c.Type,
                                                ClaimValue = c.Value,
                                                RoleId = r.Id
                                            }
                                        }
                                )
                                .ToRegistry()
                        }
                )
                .Select(r => r.Result)
                .ToRegistry();

            account.Claims = (await User.GetClaimsAsync(account.Info))
                .Select(
                    c =>
                        new AccountIdentityClaim<long>()
                        {
                            Info = new IdentityUserClaim<long>()
                            {
                                ClaimType = c.Type,
                                ClaimValue = c.Value,
                                UserId = account.Id
                            }
                        }
                )
                .ToRegistry();
        }
        return account;
    }
}
