using Undersoft.SDK.Series;
using Undersoft.SDK.Service.Data.Entity;
using System.Collections.ObjectModel;

namespace Undersoft.SDK.IntegrationTests.Instant
{
    public class AgreementVersion : Entity
    {
        
        public int VersionNumber { get; set; }

        public int OriginId { get; set; }

        public virtual AgreementType Type { get; set; }
        public virtual Listing<Agreement> Agreements { get; set; }
        public virtual Listing<AgreementText> Texts { get; set; } 
    }

    public class AgreementVersions : KeyedCollection<long, AgreementVersion>
    {
        protected override long GetKeyForItem(AgreementVersion item)
        {
            return (item.Id == 0) ? (long)item.AutoId() : item.Id;
        }
    }
}
