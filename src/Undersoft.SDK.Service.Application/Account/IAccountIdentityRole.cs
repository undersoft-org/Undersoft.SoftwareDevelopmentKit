using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Undersoft.SDK.Service.Application.Account
{
    public interface IAccountIdentityRole<TKey> : IUniqueIdentifiable, IEntity where TKey : IEquatable<TKey>
    {
        IdentityRole<TKey> Info { get; set; }

        Claim Role { get; }

        IEnumerable<IAccountIdentityRoleClaim<TKey>> Claims { get; set; }
    }
}