namespace Undersoft.SDK.Service.Application.Account;

public class AccountIdentityService
{
    private IServicer _servicer;
    private IAccountIdentityManager _manager;

    public AccountIdentityService(
        IServicer servicer,
        IAccountIdentityManager accountManager
    ) : base()
    {
        _servicer = servicer;
        _manager = accountManager;
    }

    public async Task<IAccountIdentity<long>> SignIn(IAccountIdentity<long> identity)
    {
        var account = await CompleteRegistration(
            await ConfirmEmail(await Authorize(Authenticate(identity)))
        );

        if (account.IsAuthorized)
        {
            var claims = account.GetClaims();
            account.Credentials.SessionToken = _manager.Token.Generate(claims);
            account.Notes = new AccountNotes()
            {
                Success = "Succesfully sign in!",
                Status = SigningStatus.Succeded
            };               
            await _manager.SignIn.SignInWithClaimsAsync(
                account.Info,
                account.Credentials.SaveAccountInCookies,
                claims
            );
        }
        return account;
    }

    public async Task<IAccountIdentity<long>> SignUp(IAccountIdentity<long> identity)
    {
        var _creds = identity.Credentials;
        if (!_manager.TryGetByEmail(_creds.Email, out var account))
            account = await _manager.SetUser(
                _creds.UserName,
                _creds.Email,
                _creds.Password,
                new string[] { "Subscriber" }
            );
        await _manager.SignIn.CreateUserPrincipalAsync(account.Info);
        account = await ConfirmEmail(await Authorize(Authenticate(identity)));
        account.IsAuthorized = false;
        return account;
    }

    public async Task<IAccountIdentity<long>> SignOut(IAccountIdentity<long> identity)
    {
        var account = Authenticate(identity);

        if (account.IsAuthenticated)
        {
            var claims = account.GetClaims();
            var principal = await _manager.SignIn.CreateUserPrincipalAsync(account.Info);
            if (_manager.SignIn.IsSignedIn(principal))
                await _manager.SignIn.SignOutAsync();
            account.Notes = new AccountNotes()
            {
                Success = "Succesfully sign out!",
                Status = SigningStatus.Succeded
            };
            await _manager.SignIn.SignInWithClaimsAsync(
                account.Info,
                account.Credentials.SaveAccountInCookies,
                claims
            );
        }
        return account;
    }

    public IAccountIdentity<long> Authenticate(IAccountIdentity<long> identity, bool isAuthorized = false)
    {
        var _creds = identity.Credentials;
        if (!_manager.TryGetByEmail(_creds.Email, out var account))
        {
            _creds.Password = null;
            return new AccountIdentity()
            {
                Notes = new AccountNotes()
                {
                    Errors = "Account doesn't exists",
                    Status = SigningStatus.WrongEmail
                },
                Credentials = _creds
            };
        }
        account.Credentials.PatchFrom(_creds);
        account.Credentials.PatchFrom(account.Info);
        account.IsAuthenticated = true;
        account.IsAuthorized = isAuthorized;
        return account;
    }

    public async Task<IAccountIdentity<long>> Authorize(IAccountIdentity<long> account)
    {
        if (account != null && account.IsAuthenticated)
        {
            var _creds = account.Credentials;
            account = await _manager.CheckPassword(_creds.Email, _creds.Password);
            if (account == null)
            {
                _creds.Password = null;
                return new AccountIdentity()
                {
                    Notes = new AccountNotes()
                    {
                        Errors = "Wrong password",
                        Status = SigningStatus.WrongPassword
                    },
                    Credentials = _creds
                };
            }
            else
            {
                account.IsAuthorized = true;                   
            }
        }
        account.Credentials.Password = null;
        return account;
    }

    public async Task<IAccountIdentity<long>> ConfirmEmail(IAccountIdentity<long> account)
    {
        if (account != null && account.IsAuthenticated && account.IsAuthorized)
        {
            var _creds = account.Credentials;
            if (!_creds.EmailConfirmed)
            {
                if (_creds.EmailConfirmationToken != null)
                {
                    if (
                        (await _manager.User.ConfirmEmailAsync(
                            account.Info,
                            _creds.EmailConfirmationToken
                        )).Succeeded
                    )
                    {
                        _creds.EmailConfirmed = true;
                        account.IsAuthorized = true;
                        account.Notes = new AccountNotes() { Info = "Email confirmed", };
                        return account;
                    }
                    _creds.EmailConfirmationToken = null;
                }
                _creds.EmailConfirmationToken =
                    await _manager.User.GenerateEmailConfirmationTokenAsync(account.Info);
                account.Notes = new AccountNotes()
                {
                    Info = "Please confirm your email address",
                    Status = SigningStatus.EmailNotConfirmed,
                };
                account.IsAuthorized = false;
            }
            else
            {
                account.Notes = new AccountNotes() { Info = "Your email was confirmed" };
                account.IsAuthorized = true;
            }
        }
        return account;
    }

    public async Task<IAccountIdentity<long>> ResetPassword(IAccountIdentity<long> account)
    {
        if (account != null && account.IsAuthenticated && account.IsAuthorized)
        {
            var _creds = account.Credentials;
            if (_creds.PasswordResetToken != null)
            {
                if (
                    (await _manager.User.ResetPasswordAsync(
                        account.Info,
                        _creds.EmailConfirmationToken,
                        _creds.Password
                    )).Succeeded
                )
                {
                    _creds.PasswordResetToken = null;
                    account.IsAuthorized = true;
                    account.Notes = new AccountNotes() { Info = "Password Successfully Changed", };
                    return account;
                }
                _creds.PasswordResetToken = null;
            }
            _creds.PasswordResetToken =
                await _manager.User.GeneratePasswordResetTokenAsync(account.Info);
            account.Notes = new AccountNotes()
            {
                Info = "Please confirm reset password by email",
                Status = SigningStatus.ResetPasswrdNotConfirmed,
            };
            account.IsAuthorized = false;
        }
        return (AccountIdentity)account;
    }

    public async Task<IAccountIdentity<long>> CompleteRegistration(
        IAccountIdentity<long> account
    )
    {
        if (
            account != null
            && account.IsAuthenticated
            && account.IsAuthorized
            && account.Credentials.EmailConfirmed
        )
        {
            var _creds = account.Credentials;
            if (!_creds.RegistrationCompleted)
            {
                if (_creds.RegistrationCompleteToken != null)
                {
                    if (
                        await _manager.User.VerifyUserTokenAsync(
                            account.Info,
                            "AccountRegistrationProcessTokenProvider",
                            "Registration",
                            _creds.RegistrationCompleteToken
                        )
                    )
                    {
                        _creds.RegistrationCompleted = true;
                        account.IsAuthorized = true;
                        account.Notes = new AccountNotes() { Info = "Registration completed", };
                        return account;
                    }
                    _creds.RegistrationCompleteToken = null;
                }
                _creds.RegistrationCompleteToken = await _manager.User.GenerateUserTokenAsync(
                    account.Info,
                    "AccountRegistrationProcessTokenProvider",
                    "Registration"
                );
                account.Notes = new AccountNotes()
                {
                    Info = "Please complete registration process",
                    Status = SigningStatus.NotFullyRegisterd
                };
                account.IsAuthorized = false;
            }
            else
                account.Notes = new AccountNotes() { Info = "Your registration was completed" };
        }
        return account;
    }
}

public enum SigningStatus
{
    Unsigned,
    Failure,
    Succeded,
    TryoutsOverlimit,
    WrongEmail,
    WrongPassword,
    NotFullyRegisterd,
    EmailNotConfirmed,
    ResetPasswrdNotConfirmed,
    NeedAction
}
