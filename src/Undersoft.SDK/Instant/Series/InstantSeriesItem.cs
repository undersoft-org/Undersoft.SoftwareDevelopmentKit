namespace Undersoft.SDK.Instant.Series
{
    using SDK.Extracting;
    using System.Linq;
    using System.Runtime.InteropServices;
    using SDK.Series;
    using SDK.Series.Base;
    using SDK.Uniques;
    using Rubrics;
    using Undersoft.SDK.Instant.Updating;

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public class InstantSeriesItem : SeriesItemBase<IInstant>, IInstant, IEquatable<IInstant>, IComparable<IInstant>
    {
        private ISeries<object> presets;

        public InstantSeriesItem(IInstantSeries series)
        {
            Figures = series;
        }

        public InstantSeriesItem(object key, IInstant value, IInstantSeries series) : base(key, value)
        {
            Figures = series;
        }

        public InstantSeriesItem(ulong key, IInstant value, IInstantSeries series) : base(key, value)
        {
            Figures = series;
        }

        public InstantSeriesItem(IInstant value, IInstantSeries series) : base(value)
        {
            Figures = series;
            CompactKey();
        }

        public InstantSeriesItem(ISeriesItem<IInstant> value, IInstantSeries series) : base(value)
        {
            Figures = series;
            CompactKey();
        }

        public object this[int fieldId]
        {
            get => GetPreset(fieldId);
            set => SetPreset(fieldId, value);
        }
        public object this[string propertyName]
        {
            get => GetPreset(propertyName);
            set => SetPreset(propertyName, value);
        }

        public override void Set(object key, IInstant value)
        {
            this.value = value;
            this.value.Id = key.UniqueKey();
        }

        public override void Set(IInstant value)
        {
            this.value = value;
        }

        public override void Set(ISeriesItem<IInstant> item)
        {
            this.value = item.Value;
        }

        public override bool Equals(long key)
        {
            return Id == key;
        }

        public override bool Equals(object y)
        {
            return Id.Equals(y.UniqueKey());
        }

        public bool Equals(IInstant other)
        {
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Value.GetIdBytes().BitAggregate64to32().ToInt32();
        }

        public override int CompareTo(object other)
        {
            return (int)(Id - other.UniqueKey64());
        }

        public override int CompareTo(long key)
        {
            return (int)(Id - key);
        }

        public override int CompareTo(ISeriesItem<IInstant> other)
        {
            return (int)(Id - other.Id);
        }

        public int CompareTo(IInstant other)
        {
            return (int)(Id - other.Id);
        }

        public override byte[] GetBytes()
        {
            if (!Figures.Prime && presets != null)
            {
                IInstant f = Figures.NewInstant();
                f.PutFrom(this);
                f.Code = value.Code;
                byte[] ba = f.GetBytes();
                f = null;
                return ba;
            }
            else
                return value.GetBytes();
        }

        public unsafe override byte[] GetIdBytes()
        {
            return value.GetIdBytes();
        }

        public override int[] UniqueOrdinals()
        {
            return Figures.KeyRubrics.Ordinals;
        }

        public override object[] UniqueValues()
        {
            int[] ordinals = UniqueOrdinals();
            if (ordinals != null)
                return ordinals.Select(x => value[x]).ToArray();
            return null;
        }

        public override long CompactKey()
        {
            long key = value.Id;
            if (key == 0)
            {
                IRubrics r = Figures.KeyRubrics;
                var objs = r.Ordinals.Select(x => value[x]).ToArray();
                if (objs.Any())
                {
                    key = objs.UniqueKey64(r.BinarySizes, r.BinarySize);
                }
                else
                {
                    key = Unique.NewId;
                }
                value.Id = key;
            }
            return key;
        }

        public override long Id
        {
            get => value.Id;
            set => this.value.Id = value;
        }              

        public Uscn Code
        {
            get => value.Code;
            set => this.value.Code = value;
        }

        public IInstantSeries Figures { get; set; }

        public object GetPreset(int fieldId)
        {
            if (presets != null && !Figures.Prime)
            {
                object val = presets.Get(fieldId);
                if (val != null)
                    return val;
            }
            return value[fieldId];
        }

        public object GetPreset(string propertyName)
        {
            if (presets != null && !Figures.Prime)
            {
                MemberRubric rubric = Figures.Rubrics[propertyName.UniqueKey()];
                if (rubric != null)
                {
                    object val = presets.Get(rubric.FieldId);
                    if (val != null)
                        return val;
                }
                else
                    throw new IndexOutOfRangeException("Field doesn't exist");
            }
            return value[propertyName];
        }

        public ISeriesItem<object>[] GetPresets()
        {
            return presets.AsItems().ToArray();
        }

        public void SetPreset(int fieldId, object value)
        {
            if (GetPreset(fieldId).Equals(value))
                return;
            if (!Figures.Prime)
            {
                if (presets == null)
                    presets = new Catalog<object>(9);
                presets.Put(fieldId, value);
            }
            else
                this.value[fieldId] = value;
        }

        public void SetPreset(string propertyName, object value)
        {
            MemberRubric rubric = Figures.Rubrics[propertyName.UniqueKey()];
            if (rubric != null)
                SetPreset(rubric.FieldId, value);
            else
                throw new IndexOutOfRangeException("Field doesn't exist");
        }

        public void WritePresets()
        {
            foreach (var c in presets.AsItems())
                value[(int)c.Id] = c.Value;
            presets = null;
        }

        public bool HavePresets => presets != null ? true : false;

        public object[] ValueArray { get => presets.ToArray(); set => presets.Put(value) ; }
    }
}
