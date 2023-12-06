using IdentityModel.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Undersoft.SDK.Logging;
using Undersoft.SDK.Service.Data.Repository;
using Undersoft.SDK.Service.Host;
using Undersoft.SDK.Uniques;

namespace Undersoft.SDK.Service.Application.Account;

public static class AccountIdentityServicerExtensions
{
    public static AccountIdentityManager GetIdentity(this IServicer servicer)
    {
        return servicer.Registry.GetRequiredService<AccountIdentityManager>();
    }
}
