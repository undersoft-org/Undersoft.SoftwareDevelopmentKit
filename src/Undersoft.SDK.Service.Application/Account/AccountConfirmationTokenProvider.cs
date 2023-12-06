using IdentityModel.Client;
using Microsoft.AspNetCore.DataProtection;
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

namespace Undersoft.SDK.Service.Application.Account;

public class AccountConfirmationTokenProvider<TUser>
                              : DataProtectorTokenProvider<TUser> where TUser : class
{
    public AccountConfirmationTokenProvider(
        IDataProtectionProvider dataProtectionProvider,
        IOptions<AccountEmailConfirmationTokenProviderOptions> options,
        ILogger<DataProtectorTokenProvider<TUser>> logger)
                                       : base(dataProtectionProvider, options, logger)
    {

    }
}
public class AccountEmailConfirmationTokenProviderOptions : DataProtectionTokenProviderOptions
{
    public AccountEmailConfirmationTokenProviderOptions()
    {
        Name = "EmailDataProtectorTokenProvider";
        TokenLifespan = TimeSpan.FromHours(4);
    }
}

public class AccountPasswordResetTokenProvider<TUser>
                              : DataProtectorTokenProvider<TUser> where TUser : class
{
    public AccountPasswordResetTokenProvider(
        IDataProtectionProvider dataProtectionProvider,
        IOptions<AccountPasswordResetTokenProviderOptions> options,
        ILogger<DataProtectorTokenProvider<TUser>> logger)
                                       : base(dataProtectionProvider, options, logger)
    {

    }
}
public class AccountPasswordResetTokenProviderOptions : DataProtectionTokenProviderOptions
{
    public AccountPasswordResetTokenProviderOptions()
    {
        Name = "PasswordResetDataProtectorTokenProvider";
        TokenLifespan = TimeSpan.FromHours(4);
    }
}
public class AccountChangeEmailTokenProvider<TUser>
                              : DataProtectorTokenProvider<TUser> where TUser : class
{
    public AccountChangeEmailTokenProvider(
        IDataProtectionProvider dataProtectionProvider,
        IOptions<AccountChangeEmailTokenProviderOptions> options,
        ILogger<DataProtectorTokenProvider<TUser>> logger)
                                       : base(dataProtectionProvider, options, logger)
    {

    }
}
public class AccountChangeEmailTokenProviderOptions : DataProtectionTokenProviderOptions
{
    public AccountChangeEmailTokenProviderOptions()
    {
        Name = "ChangeEmailDataProtectorTokenProvider";
        TokenLifespan = TimeSpan.FromHours(4);
    }
}

public class AccountRegistrationProcessTokenProvider<TUser>
                              : DataProtectorTokenProvider<TUser> where TUser : class
{
    public AccountRegistrationProcessTokenProvider(
        IDataProtectionProvider dataProtectionProvider,
        IOptions<AccountRegistrationConfirmationTokenProviderOptions> options,
        ILogger<DataProtectorTokenProvider<TUser>> logger)
                                       : base(dataProtectionProvider, options, logger)
    {

    }
}
public class AccountRegistrationConfirmationTokenProviderOptions : DataProtectionTokenProviderOptions
{
    public AccountRegistrationConfirmationTokenProviderOptions()
    {
        Name = "RegistrationDataProtectorTokenProvider";
        TokenLifespan = TimeSpan.FromHours(4);
    }
}
