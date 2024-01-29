using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Undersoft.SDK.Security;
using Claim = System.Security.Claims.Claim;

namespace Undersoft.SDK.Service.Server.Accounts;

public class AccountManager : TypedRegistry<IAccount>, IAccountManager
{
    public AccountManager() { }

    public AccountManager(
        UserManager<AccountUser> user,
        RoleManager<Role> role,
        SignInManager<AccountUser> signIn,
        AccountTokenGenerator token, 
        IStoreRepository<IAccountStore, Account> accounts
    )
    {
        User = user;
        Role = role;
        SignIn = signIn;
        Token = token;        
    }

    public IStoreRepository<IAccountStore, Account> Accounts { get; set; }

    public UserManager<AccountUser> User { get; set; }

    public RoleManager<Role> Role { get; set; }

    public SignInManager<AccountUser> SignIn { get; set; }

    public AccountTokenGenerator Token { get; set; }

    public async Task<string> GetToken(string email, string password)
    {
        string token = null;
        var account = await CheckPassword(email, password);
        if (account != null)
            token = Token.Generate(account.GetClaims());
        return token;
    }
    public string GetToken(IAuthorization auth)
    {
        if (!TryGetByEmail(auth.Credentials.Email, out IAccount account))
            return null;
        return Token.Generate(account.GetClaims());
    }

    public async Task<bool> CheckToken(string token)
    {
        return (await Token.Validate(token)).IsValid;
    }

    public async Task<string> RenewToken(string token)
    {
        string _token = null;
        var validation = await Token.Validate(token);
        if (validation.IsValid)
        {
            var emailClaim = validation.ClaimsIdentity.Claims.Where(c => c.Type == JwtClaimTypes.Email).FirstOrDefault();
            if (emailClaim != null)
                _token = GetToken(new Authorization() { Credentials = new Credentials() { Email = emailClaim.Value } });
        }
        return _token;
    }

    public async Task<Account> CheckPassword(string email, string password)
    {
        if (!TryGetByEmail(email, out IAccount account))
            return null;
        if (account.User != null && await User.CheckPasswordAsync(account.User, password))
            return (Account)account;
        return null;
    }

    public async Task<Account> SetUser(string username, string email, string password, IEnumerable<string> roles, IEnumerable<string> scopes = null)
    {
        if (!TryGetByEmail(email, out IAccount account))
        {
            account = new Account(username, email, roles);
            if (!(await User.CreateAsync(account.User, password)).Succeeded)
                return null;
        }
        else
        {
            await User.RemoveFromRolesAsync(account.User, account.Roles.Select(r => r.Name));
            await User.RemoveClaimsAsync(account.User, account.GetClaims());
        }

        await User.AddToRolesAsync(account.User, roles);
        await User.AddClaimsAsync(
            account.User,
            new Claim[]
            {
                new Claim(JwtClaimTypes.Id, account.Id.ToString()),
                new Claim("user_id", account.UserId.ToString()),
                new Claim(JwtClaimTypes.Email, account.User.Email),
                new Claim(JwtClaimTypes.Name, account.User.UserName),
                new Claim("code_no", account.CodeNo),
            }
            .Concat(roles?.Select(r => new Claim(JwtClaimTypes.Role, r)))
            .Concat(scopes?.Select(r => new Claim(JwtClaimTypes.Scope, r)))
        );
        var _account = (Account)account;
        await MapAccount(_account);
        Put(_account);
        return _account;
    }

    public async Task<bool> Delete(string email)
    {
        if (!TryGetByEmail(email, out var account))
            return false;
        await User.DeleteAsync(account.User);
        Remove(account.Id);
        return true;
    }

    public Role SetUserRole(string email, string current, string previous = null)
    {
        if (!TryGetByEmail(email, out var account))
            return null;

        var currentRoleId = (email + current).UniqueKey64();
        var role = new Role() { Id = currentRoleId, Name = current };
       
        if (role != null)
        {
            account.Roles.Add(role);
            User.AddToRoleAsync(account.User, current);
            if (previous != null)
            {
                account.Roles.Remove((email + previous).UniqueKey64());
                User.RemoveFromRoleAsync(account.User, previous);
            }
        }
        return role;
    }

    public async Task<bool> SetUserClaim(string email, Claim claim)
    {
        if (!TryGetByEmail(email, out var account))
            return false;
        var id = (email + claim.Type).UniqueKey64();

        if (claim != null)
        { 
            var _claim = new AccountClaim() { ClaimType = claim.Type, ClaimValue = claim.Value, Id = id, UserId = account.UserId }; 
            account.Claims.Add(_claim);
            await User.RemoveClaimAsync(account.User, claim);
            var result = await User.AddClaimAsync(account.User, claim);
            if (result.Succeeded)
                return true;
        }
        return false;
    }

    public async Task<Role> SetRole(string roleName)
    {
        var role = Role.Roles.Where(r => r.Name == roleName).FirstOrDefault();
        if (role == null)
        {
            role = new Role(roleName);
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

    public bool TryGetByEmail(string email, out IAccount account)
    {
        account = GetByEmail(email).GetAwaiter().GetResult();
        if (account != null)
            return true;
        return false;
    }

    public bool TryGetById(long id, out IAccount account)
    {
        if (TryGet(id, out account))
            return true;

        account = GetById(id).GetAwaiter().GetResult();

        if (account == null)
            return false;

        return true;
    }

    public async Task<Account> GetByName(string name)
    {
        var user = await User.FindByNameAsync(name);
        if (user != null)
            return await GetById(user.Id);
        return null;
    }

    public async Task<Account> GetByEmail(string email)
    {
        if (TryGet(email, out IAccount account))
            return (Account)account;
        var _account = new Account();        
        _account.User = await User.FindByEmailAsync(email);        
        if ((await MapAccount(_account)).User != null)
        {
            Put(_account?.User?.Email, _account);
            account.UserId = _account.User.Id;
        }
        Put(_account);
        return _account;
    }

    public async Task<Account> GetById(long id)
    {
        if (TryGet(id, out IAccount account))
            return (Account)account;
        var _account = new Account();
        _account.User = await User.FindByIdAsync(_account.Id.ToString());
        if ((await MapAccount(_account)).User != null)
        {
            Put(_account?.User?.Email, _account);
            account.UserId = _account.User.Id;
        }
        Put(_account);        
        return _account;
    }

    public async Task<Account> MapAccount(Account account)
    {
        if (account.User != null)
        {
            account.Credentials.PatchFrom(account.User);
            account.Roles = (await User.GetRolesAsync(account.User))
                .Select(async r => await Role.FindByNameAsync(r))
                .Select(t => t.Result)
                .ToList()
                .Select(
                    async r => new Role(r.Name)
                    {
                        Claims = (await Role.GetClaimsAsync(r))
                                .Select(
                                    c =>
                                        new RoleClaim()
                                        {

                                            ClaimType = c.Type,
                                            ClaimValue = c.Value,
                                            RoleId = r.Id
                                        }
                                )
                                .ToListing()
                    }              
                )
                .Select(r => r.Result)
                .ToListing();

            account.Claims = (await User.GetClaimsAsync(account.User))
                .Select(
                    c =>
                        new AccountClaim()
                        {
                                ClaimType = c.Type,
                                ClaimValue = c.Value,
                                UserId = account.Id                            
                        }
                )
                .ToListing();
        }
        return account;
    }
}
