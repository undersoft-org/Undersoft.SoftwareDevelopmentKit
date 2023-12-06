using Undersoft.SDK.Series;
using Undersoft.SDK.Service.Data.Entity;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System;

namespace Undersoft.SDK.IntegrationTests.Instant
{
    public class Agreement : InnerProxy
    {
        public AgreementKind Kind { get; set; }
        [Key]
        public Guid UserId { get; set; }
        public long VersionId { get; set; }
        public long FormatId { get; set; }
        public string Language { get; set; } = "pl";
        public string Comments { get; set; } = "Comments";
        public string Email { get; set; } = "fdfds";
        public string PhoneNumber { get; set; } = "3453453";

        public virtual Listing<AgreementFormat> Formats { get; set; } = null;
        public virtual Listing<AgreementVersion> Versions { get; set; } = null;
        public virtual AgreementType Type { get; set; } = null;
    }

    public class Agreements : KeyedCollection<long, Agreement>
    {
        protected override long GetKeyForItem(Agreement item)
        {
            return (item.Id == 0) ? (long)item.AutoId() : item.Id;
        }
    }
}
