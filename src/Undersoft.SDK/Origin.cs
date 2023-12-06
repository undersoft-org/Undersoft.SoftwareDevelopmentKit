using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Undersoft.SDK.Instant.Attributes;
using Undersoft.SDK.Instant.Rubrics.Attributes;
using Undersoft.SDK.Uniques;

namespace Undersoft.SDK
{
    [DataContract]
    [StructLayout(LayoutKind.Sequential)]
    public class Origin : Identifiable, IOrigin
    {
        public Origin() : base(true) { }

        public Origin(bool autoId) : base(autoId) { }

        [StringLength(32)]
        [DataMember(Order = 5)]
        [Column(Order = 5)]
        [InstantAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public virtual string OriginName { get; set; }

        [IdentityRubric]
        [Column(TypeName = "timestamp", Order = 6)]
        [DataMember(Order = 6)]
        [InstantAs(UnmanagedType.I8, SizeConst = 8)]
        public virtual DateTime Modified { get; set; } = DateTime.Now;

        [IdentityRubric]
        [StringLength(32)]
        [Column(Order = 7)]
        [DataMember(Order = 7)]
        [InstantAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public virtual string Modifier { get; set; }

        [IdentityRubric]
        [Column(TypeName = "timestamp", Order = 8)]
        [DataMember(Order = 8)]
        [InstantAs(UnmanagedType.I8, SizeConst = 8)]
        public virtual DateTime Created { get; set; }

        [IdentityRubric]
        [StringLength(32)]
        [Column(Order = 9)]
        [DataMember(Order = 9)]
        [InstantAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public virtual string Creator { get; set; }

        [DataMember(Order = 10)]
        [Column(Order = 10)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Ordinal { get; set; }

        [Column(Order = 11)]
        [StringLength(32)]
        [DataMember(Order = 11)]
        [InstantAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public virtual string Label { get; set; }

        public virtual TEntity Sign<TEntity>(TEntity entity = null) where TEntity : class, IOrigin
        {
            if (entity == null)
            {
                AutoId();
                Stamp<TEntity>();
                Created = Time;
                return default;
            }
            entity.AutoId();
            Stamp(entity);
            entity.Created = entity.Time;
            return entity;
        }

        public virtual TEntity Stamp<TEntity>(TEntity entity = null) where TEntity : class, IOrigin
        {
            if (entity == null)
            {
                Time = DateTime.Now;
                return default;
            }
            entity.Time = DateTime.Now;
            return entity;

        }

        public virtual byte[] GetBytes()
        {
            return ((IUnique)code).GetBytes();
        }

        public virtual byte[] GetIdBytes()
        {
            return ((IUnique)code).GetIdBytes();
        }

        public virtual bool Equals(IUnique other)
        {
            return ((IEquatable<IUnique>)code).Equals(other);
        }

        public virtual int CompareTo(IUnique other)
        {
            return ((IComparable<IUnique>)code).CompareTo(other);
        }
    }



}
