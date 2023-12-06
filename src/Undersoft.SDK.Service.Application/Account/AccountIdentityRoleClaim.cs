using IdentityModel.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Undersoft.SDK.Logging;
using Undersoft.SDK.Uniques;
using System.Security.Claims;

namespace Undersoft.SDK.Service.Application.Account;

public class AccountIdentityRoleClaim<TKey> : UniqueIdentifiable, IAccountIdentityRoleClaim<TKey> where TKey : IEquatable<TKey>
{
    public IdentityRoleClaim<TKey> Info { get; set; } = new IdentityRoleClaim<TKey>();

    public Claim Claim => Info.ToClaim();
}
