using Undersoft.SDK.Instant.Proxies;
using Undersoft.SDK.Instant.Updating;

namespace Undersoft.SDK.Security
{
    [Serializable]
    public class Authorization : Origin, IInnerProxy, IAuthorization
    {
        public Credentials Credentials { get; set; } = new Credentials();

        public AuthorizationNotes Notes { get; set; } = new AuthorizationNotes();

        public bool Authorized { get; set; }

        public bool Authenticated { get; set; }

        public void Map(object user)
        {
            this.PatchFrom(user);
        }

        IProxy IInnerProxy.Proxy => throw new NotImplementedException();

        object IInnerProxy.this[int fieldId] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        object IInnerProxy.this[string propertyName] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
