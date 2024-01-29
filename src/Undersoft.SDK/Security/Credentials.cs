using System.Runtime.Serialization;
using Undersoft.SDK.Instant.Proxies;

namespace Undersoft.SDK.Security
{
    [Serializable]
    public enum ServiceSite
    {
        Client,
        Server
    }

    public enum DataSite
    {
        None,
        Client,
        Endpoint
    }

    public enum IdentityType
    {
        User,
        Server,
        Service
    }

    [Serializable]
    [DataContract]
    public class Credentials : InnerProxy, ICredentials
    {
        [DataMember(Order = 0)]
        public ServiceSite? Site { get; set; }

        [DataMember(Order = 1)]
        public IdentityType? Type { get; set; }

        [DataMember(Order = 2)]
        public string UserName { get; set; }

        [DataMember(Order = 3)]
        public string NormalizedUserName { get; set; }

        [DataMember(Order = 4)]
        public string Email { get; set; }

        [DataMember(Order = 5)]
        public string OldPassword { get; set; }

        [DataMember(Order = 6)]
        public string Password { get; set; }

        [DataMember(Order = 7)]
        public string PhoneNumber { get; set; }

        [DataMember(Order = 8)]
        public bool EmailConfirmed { get; set; }

        [DataMember(Order = 9)]
        public bool PhoneNumberConfirmed { get; set; }

        [DataMember(Order = 10)]
        public bool RegistrationCompleted { get; set; }

        [DataMember(Order = 11)]
        public string SessionToken { get; set; }

        [DataMember(Order = 12)]
        public string PasswordResetToken { get; set; }

        [DataMember(Order = 13)]
        public string EmailConfirmationToken { get; set; }

        [DataMember(Order = 14)]
        public string PhoneNumberConfirmationToken { get; set; }

        [DataMember(Order = 15)]
        public string RegistrationCompleteToken { get; set; }

        [DataMember(Order = 16)]
        public int AccessFailedCount { get; set; }

        [DataMember(Order = 17)]
        public bool SaveAccountInCookies { get; set; }

        [DataMember(Order = 18)]
        public bool Authorized { get; set; }

        [DataMember(Order = 19)]
        public bool Authenticated { get; set; }
    }
}
