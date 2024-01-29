using Undersoft.SDK.Instant.Proxies;

namespace Undersoft.SDK.Security
{
    public interface IAuthorization : IOrigin, IInnerProxy
    {
        Credentials Credentials { get; set; }

        AuthorizationNotes Notes { get; set; }
    }
}