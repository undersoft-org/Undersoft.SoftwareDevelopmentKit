using Undersoft.SDK.Security;

namespace Undersoft.SDK.Service.Server.Accounts;

public class AccountService : IAccountAccess
{
    private IServicer _servicer;
    private IAccountManager _manager;

    public AccountService(IServicer servicer, IAccountManager accountManager)
    {
        _servicer = servicer;
        _manager = accountManager;
    }

    public async Task<IAuthorization> SignIn(IAuthorization identity)
    {
        var account = await CompleteRegistration(
            await ConfirmEmail(await Authorize(Authenticate(identity)))
        );

        if (account.Credentials.Authorized)
        {
            account.Credentials.SessionToken = _manager.GetToken(account);
            account.Notes = new AuthorizationNotes()
            {
                Success = "Signed in",
                Status = SigningStatus.SignedIn
            };
            var user = await _manager.GetByEmail(account.Credentials.Email);
            await _manager.SignIn.SignInWithClaimsAsync(
                user.User,
                account.Credentials.SaveAccountInCookies,
                user.GetClaims()
            );
        }
        return account;
    }

    public async Task<IAuthorization> SignUp(IAuthorization identity)
    {
        var _creds = identity.Credentials;
        if (!_manager.TryGetByEmail(_creds.Email, out var account))
            account = await _manager.SetUser(
                _creds.UserName,
                _creds.Email,
                _creds.Password,
                new string[] { "User" }
            );
        await _manager.SignIn.CreateUserPrincipalAsync(account.User);
        return await ConfirmEmail(await Authorize(Authenticate(identity)));
    }

    public async Task<IAuthorization> SignOut(IAuthorization identity)
    {
        var account = Authenticate(identity);

        if (account.Credentials.Authenticated)
        {
            var principal = await _manager.SignIn.CreateUserPrincipalAsync(
                await _manager.User.FindByEmailAsync(account.Credentials.Email)
            );
            if (_manager.SignIn.IsSignedIn(principal))
                await _manager.SignIn.SignOutAsync();
            account.Notes = new AuthorizationNotes()
            {
                Success = "Signed out",
                Status = SigningStatus.SignedOut
            };
        }
        return account;
    }

    public async Task<IAuthorization> Renew(IAuthorization identity)
    {
        var account = Authenticate(identity);

        if (account.Credentials.Authenticated)
        {
            var token = await _manager.RenewToken(
                identity.Credentials.SessionToken
            );
            if (token != null)
            {
                account.Credentials.SessionToken = token;
                account.Notes = new AuthorizationNotes()
                {
                    Success = "Token renewed",
                    Status = SigningStatus.Succeed
                };
            }
            else
            {
                account.Credentials.SessionToken = null;
                account.Notes = new AuthorizationNotes()
                {
                    Errors = "Invalid token ",
                    Status = SigningStatus.Failure
                };
            }
        }
        return account;
    }

    public IAuthorization Authenticate(IAuthorization identity, bool isAuthorized = false)
    {
        var _creds = identity.Credentials;
        if (!_manager.TryGetByEmail(_creds.Email, out var account))
        {
            _creds.Password = null;
            return new Authorization() { Credentials = _creds };
        }
        var creds = account.Credentials;
        creds.PatchFrom(_creds);
        creds.Authenticated = true;
        creds.Authorized = isAuthorized;
        return account;
    }

    public async Task<IAuthorization> Authorize(IAuthorization account)
    {
        var _creds = account?.Credentials;
        if (_creds.Authenticated)
        {
            if (await _manager.CheckPassword(_creds.Email, _creds.Password) == null)
            {
                _creds.Password = null;
                return new Authorization()
                {
                    Notes = new AuthorizationNotes()
                    {
                        Errors = "Invalid password",
                        Status = SigningStatus.InvalidPassword
                    },
                    Credentials = _creds
                };
            }
            else
            {
                _creds.Authorized = true;
            }
        }
        _creds.Password = null;
        return account;
    }

    public async Task<IAuthorization> ConfirmEmail(IAuthorization account)
    {
        if (account != null && account.Credentials.Authenticated && account.Credentials.Authorized)
        {
            var _creds = account.Credentials;
            if (!_creds.EmailConfirmed)
            {
                if (_creds.EmailConfirmationToken != null)
                {
                    if (
                        (
                            await _manager.User.ConfirmEmailAsync(
                                (await _manager.GetByEmail(_creds.Email)).User,
                                _creds.EmailConfirmationToken
                            )
                        ).Succeeded
                    )
                    {
                        _creds.EmailConfirmationToken = null;
                        _creds.EmailConfirmed = true;
                        account.Credentials.Authorized = true;
                        account.Notes = new AuthorizationNotes()
                        {
                            Info = "Email has been confirmed",
                        };
                        return account;
                    }
                }
                _creds.EmailConfirmationToken =
                    await _manager.User.GenerateEmailConfirmationTokenAsync(
                        (await _manager.GetByEmail(_creds.Email)).User
                    );
                account.Notes = new AuthorizationNotes()
                {
                    Info = "Please confirm your email",
                    Status = SigningStatus.EmailNotConfirmed,
                };
                account.Credentials.Authorized = false;
            }
            else
            {
                account.Notes = new AuthorizationNotes() { Info = "Email has been confirmed" };
                account.Credentials.Authorized = true;
            }
        }
        return account;
    }

    public async Task<IAuthorization> ResetPassword(IAuthorization account)
    {
        if (account != null && account.Credentials.Authenticated && account.Credentials.Authorized)
        {
            var _creds = account.Credentials;
            if (_creds.PasswordResetToken != null)
            {
                if (
                    (
                        await _manager.User.ResetPasswordAsync(
                            (await _manager.GetByEmail(_creds.Email)).User,
                            _creds.EmailConfirmationToken,
                            _creds.Password
                        )
                    ).Succeeded
                )
                {
                    _creds.PasswordResetToken = null;
                    account.Credentials.Authorized = true;
                    account.Notes = new AuthorizationNotes() { Info = "Password has been reset", };
                    return account;
                }
                _creds.PasswordResetToken = null;
            }
            _creds.PasswordResetToken = await _manager.User.GeneratePasswordResetTokenAsync(
                (await _manager.GetByEmail(_creds.Email)).User
            );
            account.Notes = new AuthorizationNotes()
            {
                Info = "Please confirm reset password by email",
                Status = SigningStatus.ResetPasswordNotConfirmed,
            };
            account.Credentials.Authorized = false;
        }
        return account;
    }

    public async Task<IAuthorization> CompleteRegistration(IAuthorization account)
    {
        if (
            account != null
            && account.Credentials.Authenticated
            && account.Credentials.Authorized
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
                            (await _manager.GetByEmail(_creds.Email)).User,
                            "AccountRegistrationProcessTokenProvider",
                            "Registration",
                            _creds.RegistrationCompleteToken
                        )
                    )
                    {
                        _creds.RegistrationCompleted = true;
                        account.Credentials.Authorized = true;
                        account.Notes = new AuthorizationNotes()
                        {
                            Info = "Registration completed",
                        };
                        return account;
                    }
                    _creds.RegistrationCompleteToken = null;
                }
                _creds.RegistrationCompleteToken = await _manager.User.GenerateUserTokenAsync(
                    (await _manager.GetByEmail(_creds.Email)).User,
                    "AccountRegistrationProcessTokenProvider",
                    "Registration"
                );
                account.Notes = new AuthorizationNotes()
                {
                    Info = "Please complete registration process",
                    Status = SigningStatus.RegistrationNotCompleted
                };
                account.Credentials.Authorized = false;
            }
            else
                account.Notes = new AuthorizationNotes() { Info = "Registration completed" };
        }
        return account;
    }
}
