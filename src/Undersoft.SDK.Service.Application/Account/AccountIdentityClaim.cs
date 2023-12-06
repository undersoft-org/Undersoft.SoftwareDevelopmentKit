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

public class AccountIdentityClaim<TKey> : UniqueIdentifiable, IAccountIdentityClaim<TKey> where TKey : IEquatable<TKey>
{
    public IdentityUserClaim<TKey> Info { get; set; } = new IdentityUserClaim<TKey>();

    public Claim Claim => Info.ToClaim();
}
