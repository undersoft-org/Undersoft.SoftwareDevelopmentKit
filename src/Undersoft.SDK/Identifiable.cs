using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Undersoft.SDK.Instant.Attributes;
using Undersoft.SDK.Instant.Rubrics.Attributes;
using Undersoft.SDK.Uniques;

namespace Undersoft.SDK
{
    [DataContract]
    [StructLayout(LayoutKind.Sequential)]
    public class Identifiable : IIdentifiable, INotifyPropertyChanged
    {
        public Identifiable() : this(true) { }

        public Identifiable(bool autoId)
        {
            if (!autoId)
                return;

            code.SetId(Unique.NewId);
            IsNew = true;
        }

        [NotMapped]
        [JsonIgnore]
        [IgnoreDataMember]
        protected Uscn code;

        public event PropertyChangedEventHandler PropertyChanged;

        [Key]
        [KeyRubric]
        [IdentityRubric]
        [DataMember(Order = 1)]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual long Id
        {
            get => code.Id;
            set
            {
                if (value != 0 && !code.Equals(value) && IsNew)
                {
                    code.SetId(value);
                    IsNew = false;
                }
            }
        }

        [IdentityRubric]
        [DataMember(Order = 2)]
        [Column(Order = 2)]
        public virtual long TypeId
        {
            get =>
                code.TypeId == 0
                    ? code.SetTypeId(GetType().UniqueKey32())
                    : code.TypeId;
            set
            {
                if (value != 0 && value != code.TypeId)
                    code.SetTypeId(GetType().UniqueKey32());
            }
        }

        [IdentityRubric]
        [DataMember(Order = 3)]
        [Column(Order = 3)]
        public virtual int OriginKey
        {
            get => (int)code.OriginKey;
            set => code.SetOriginKey(value);
        }

        [Required]
        [IdentityRubric]
        [StringLength(32)]
        [ConcurrencyCheck]
        [DataMember(Order = 4)]
        [Column(Order = 4)]
        [InstantAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public virtual string CodeNo
        {
            get => code;
            set => code.FromTetrahex(value.ToCharArray());
        }

        [NotMapped]
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual DateTime Time
        {
            get => DateTime.FromBinary(code.Time);
            set => code.SetDateLong(value.ToBinary());
        }

        [NotMapped]
        [JsonIgnore]
        [IgnoreDataMember]
        private bool IsNew { get; set; }

        public virtual long AutoId()
        {
            var key = code.Id;
            return key != 0
                ? key
                : code.SetId(Unique.NewId);
        }

        public virtual byte GetPriority()
        {
            return code.GetPriority();
        }

        public virtual void SetFlag(StateFlags state, bool flag)
        {
            code.SetFlag(state, flag);
        }

        public virtual void GetFlag(StateFlags state)
        {
            code.GetFlag(state);
        }

        public virtual long SetId(long id)
        {
            long longid = id;
            long key = Id;
            if (longid != 0 && key != longid)
                return Id = longid;
            return AutoId();
        }

        public virtual long SetId(object id)
        {
            if (id == null)
                return AutoId();
            else if (id.GetType().IsPrimitive)
                return SetId((long)id);
            else
                return SetId(id.UniqueKey64());
        }
    }



}
