using Undersoft.SDK.Service.Data.Entity;

namespace Undersoft.SDK.IntegrationTests.Instant
{
    public class AgreementFormat : Entity
    {
        public string Name { get; set; }

        public virtual Agreements Agreements { get; } = new Agreements();
    }


}
