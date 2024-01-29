using System.Runtime.Serialization;
using Undersoft.SDK.Instant.Proxies;

namespace Undersoft.SDK.Security
{
    [DataContract]
    public class AuthorizationNotes: InnerProxy
    {
        [DataMember(Order = 0)]
        public string Errors { get; set; }

        [DataMember(Order = 1)]
        public string Success { get; set; }

        [DataMember(Order = 2)]
        public string Info { get; set; }

        [DataMember(Order = 3)]
        public SigningStatus? Status { get; set; }
    }
}
