using Microsoft.AspNetCore.Identity;
using Undersoft.SDK.Uniques;
using System.Security.Claims;

namespace Undersoft.SDK.Service.Application.Account
{
    public interface IAccountIdentity<TKey> : IOrigin where TKey : IEquatable<TKey>
    {
        IdentityUser<TKey> Info { get; set; }

        Registry<AccountIdentityRole<TKey>> Roles { get; set; }

        Registry<AccountIdentityClaim<TKey>> Claims { get; set; }

        IEnumerable<Claim> GetClaims();

        Credentials Credentials { get; set; }

        AccountNotes Notes { get; set; }

        bool IsAuthorized { get; set; }

        bool IsAuthenticated { get; set; }
    }
}
