using Undersoft.SDK.Service.Data.Entity;
using System.Collections.ObjectModel;

namespace Undersoft.SDK.IntegrationTests.Instant
{
    public class OriginAgreementType : Entity
    {
        public long TypeId { get; set; }

        public virtual AgreementType Type { get; set; }
    }

    public class OriginAgreementTypes : KeyedCollection<long, OriginAgreementType>
    {
        protected override long GetKeyForItem(OriginAgreementType item)
        {
            return (item.Id == 0) ? (long)item.AutoId() : item.Id;
        }
    }
}
