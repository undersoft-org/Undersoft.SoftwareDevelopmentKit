namespace Undersoft.SDK.Security
{
    public enum SigningStatus
    {
        Unsigned,
        Failure,
        Succeed,
        SignedIn,
        SignedOut,
        TryoutsOverlimit,
        InvalidEmail,
        InvalidPassword,
        RegistrationNotCompleted,
        EmailNotConfirmed,
        ResetPasswordNotConfirmed,
        ActionRequired
    }
}
