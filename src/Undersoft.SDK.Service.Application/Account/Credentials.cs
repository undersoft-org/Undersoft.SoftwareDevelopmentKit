using Microsoft.AspNetCore.Identity;

namespace Undersoft.SDK.Service.Application.Account;

public class Credentials : DataObject
{
    public Credentials() { }

    public void MapUser(IdentityUser<long> account)
    {
        UserName = account.UserName;
        Email = account.Email;
        PhoneNumber = account.PhoneNumber;
        EmailConfirmed = account.EmailConfirmed;
        PhoneNumberConfirmed = account.PhoneNumberConfirmed;
        AccessFailedCount = account.AccessFailedCount;
        NormalizedUserName = account.NormalizedUserName;
    }

    public string UserName { get; set; }

    public string NormalizedUserName { get; set; }

    public string Email { get; set; }

    public string OldPassword { get; set; }

    public string Password { get; set; }

    public string PhoneNumber { get; set; }

    public bool EmailConfirmed { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public bool RegistrationCompleted { get; set; }

    public string SessionToken { get; set; }

    public string PasswordResetToken { get; set; }

    public string EmailConfirmationToken { get; set; }

    public string PhoneNumberConfirmationToken { get; set; }

    public string RegistrationCompleteToken { get; set; }

    public int AccessFailedCount { get; set; }

    public bool SaveAccountInCookies { get; set; }
}
