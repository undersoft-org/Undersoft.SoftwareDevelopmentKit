﻿using Microsoft.AspNetCore.Identity;

namespace Undersoft.SDK.Service.Server.Accounts.Tokens;

public class AccountRegistrationConfirmationTokenProviderOptions : DataProtectionTokenProviderOptions
{
    public AccountRegistrationConfirmationTokenProviderOptions()
    {
        Name = "RegistrationDataProtectorTokenProvider";
        TokenLifespan = TimeSpan.FromHours(4);
    }
}
