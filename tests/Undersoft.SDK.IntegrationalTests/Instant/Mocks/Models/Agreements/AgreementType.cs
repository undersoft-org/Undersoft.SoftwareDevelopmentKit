using Undersoft.SDK.Service.Data.Entity;

namespace Undersoft.SDK.IntegrationTests.Instant
{
    public class AgreementType : Entity
    {
        public int CurrentVersion { get; set; }
        public string Name { get; set; }
        public bool IsGiodoType { get; set; }

        public virtual Agreements Agreements { get; set; } = new Agreements();
        public virtual AgreementVersions Versions { get; set; } = new AgreementVersions();
        public virtual OriginAgreementTypes Origins { get; set; } = new OriginAgreementTypes();
    }
}
