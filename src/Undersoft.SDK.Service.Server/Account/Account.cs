using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using Undersoft.SDK.Security;
using Undersoft.SDK.Service.Data.Contract;

namespace Undersoft.SDK.Service.Server.Accounts;

public class Account : DataObject, IEntity, IAccount, IAuthorization
{
    public Account() { }

    public Account(string email)
    {
        User = new AccountUser(email);
        Roles.Add(new Role("guest"));
        UserId = User.Id;
    }

    public Account(string email, string role)
    {
        User = new AccountUser(email);
        Roles.Add(new Role(role));
        UserId = User.Id;
    }

    public Account(string userName, string email, IEnumerable<string> roles)
    {
        User = new AccountUser(userName, email);
        roles.ForEach(r => Roles.Add(new Role(r)));
        UserId = User.Id;
    }
    
    public long UserId { get; set; }
    public virtual AccountUser User { get; set; }

    public virtual Listing<Role> Roles { get; set; }

    public virtual Listing<AccountClaim> Claims { get; set; }

    public virtual Listing<AccountToken> Tokens { get; set; }

    public IEnumerable<Claim> GetClaims()
    {
        return Claims.Select(c => c.Claim);
    }

    [NotMapped]
    public Credentials Credentials { get; set; } = new Credentials();

    [NotMapped]
    public AuthorizationNotes Notes { get; set; } = new AuthorizationNotes();

    public bool Authorized { get; set; }

    public bool Authenticated { get; set; }
}
