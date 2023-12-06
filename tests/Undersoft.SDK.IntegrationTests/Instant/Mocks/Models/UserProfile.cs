using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Undersoft.SDK.Service.Data.Entity;

namespace Undersoft.SDK.IntegrationTests.Instant
{
    public class UserProfile : Entity
    {
        [ForeignKey("User")]
        public Guid UserId { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Surname { get; set; }

        public int? ProvinceId { get; set; }

        [StringLength(100)]
        public string City { get; set; }

        [StringLength(100)]
        public string Street { get; set; }

        [StringLength(50)]
        public string BuildingNumber { get; set; }

        [StringLength(50)]
        public string ApartmentNumber { get; set; }

        [StringLength(6)]
        public string Postcode { get; set; }

        [StringLength(50)]
        public string PhoneNumber { get; set; }

        [StringLength(255)]
        public string Email { get; set; }

        public long ChipNumber { get; set; }

        [StringLength(2), MinLength(2)]
        [Column(TypeName = "char(2)")]
        public string Language { get; set; }

        [StringLength(100)]
        public string PayrollNumber { get; set; }

        public int CustomerExternalId { get; set; }

        [Description("Nazwa klienta")]
        public string CustomerName { get; set; }

        [Description("Facebook ID")]
        public string FacebookId { get; set; }

        [StringLength(11)]
        public string Pesel { get; set; }

        public Guid SSOID { get; set; }

        public User User { get; set; }
    }
}
