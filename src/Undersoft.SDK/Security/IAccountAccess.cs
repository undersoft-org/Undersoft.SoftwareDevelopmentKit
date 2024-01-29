namespace Undersoft.SDK.Security
{
    public interface IAccountAccess
    {
        Task<IAuthorization> CompleteRegistration(IAuthorization account);
        Task<IAuthorization> ConfirmEmail(IAuthorization account);
        Task<IAuthorization> Renew(IAuthorization account);
        Task<IAuthorization> ResetPassword(IAuthorization account);
        Task<IAuthorization> SignIn(IAuthorization account);
        Task<IAuthorization> SignOut(IAuthorization account);
        Task<IAuthorization> SignUp(IAuthorization account);
    }
}