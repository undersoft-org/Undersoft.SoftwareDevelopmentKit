using IdentityModel.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Undersoft.SDK.Logging;
using Undersoft.SDK.Series;
using Undersoft.SDK.Uniques;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations.Schema;

namespace Undersoft.SDK.Service.Server.Accounts;

public class AccountRole : IdentityUserRole<long>, IIdentifiable
{
    public long Id { get; set; }

    public long TypeId { get; set; }

    public long AccountId { get; set; }
    public virtual Account Account { get; set; }

    public long AccountRoleId { get; set; }
    public virtual Role Role { get; set; }
}
