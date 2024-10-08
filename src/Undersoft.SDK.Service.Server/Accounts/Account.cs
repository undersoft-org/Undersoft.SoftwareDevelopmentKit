﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using Undersoft.SDK.Service.Access;
using Undersoft.SDK.Service.Server.Accounts.Identity;
using Undersoft.SDK.Service.Server.Accounts.Licensing;
using Undersoft.SDK.Service.Server.Accounts.MultiTenancy;
using Undersoft.SDK.Service.Server.Accounts.Tokens;

namespace Undersoft.SDK.Service.Server.Accounts;

[StructLayout(LayoutKind.Sequential)]
public class Account : Authorization, IEntity, IAccount
{
    public Account() { }

    public Account(string email)
    {
        User = new AccountUser(email);
        Roles = new ObjectSet<Role>();
        Roles.Add(new Role("guest"));
        UserId = User.Id;
        Id = User.Id;
    }

    public Account(string email, string role)
    {
        User = new AccountUser(email);
        Roles = new ObjectSet<Role>();
        Roles.Add(new Role(role));
        UserId = User.Id;
        Id = User.Id;
    }

    public Account(string userName, string email, IEnumerable<string> roles)
    {
        User = new AccountUser(userName, email);
        Roles = new ObjectSet<Role>();
        roles.ForEach(r => Roles.Add(new Role(r)));
        UserId = User.Id;
        Id = User.Id;
    }

    public long? UserId { get; set; }

    [NotMapped]
    public AccountUser User { get; set; }

    public virtual ObjectSet<Role> Roles { get; set; }

    [NotMapped]
    public ObjectSet<AccountClaim> Claims { get; set; }

    public virtual ObjectSet<AccountToken> Tokens { get; set; }

    public long? PersonalId { get; set; }
    public virtual AccountPersonal Personal { get; set; }

    public long? AddressId { get; set; }
    public virtual AccountAddress Address { get; set; }

    public long? ProfessionalId { get; set; }
    public virtual AccountProfessional Professional { get; set; }

    public long? OrganizationId { get; set; }
    public virtual AccountOrganization Organization { get; set; }

    public long? ConsentId { get; set; }
    public virtual AccountConsent Consent { get; set; }

    public long? SubscriptionId { get; set; }
    public virtual AccountSubscription Subscription { get; set; }

    public long? PaymentId { get; set; }
    public virtual AccountPayment Payment { get; set; }

    public long? TenantId { get; set; }
    public virtual AccountTenant Tenant { get; set; }

    public IEnumerable<System.Security.Claims.Claim> GetClaims()
    {
        return Claims.Select(c => c.Claim);
    }

}
