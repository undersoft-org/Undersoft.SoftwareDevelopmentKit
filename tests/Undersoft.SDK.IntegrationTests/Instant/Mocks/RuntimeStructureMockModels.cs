namespace Undersoft.SDK.IntegrationTests.Instant
{
    using Undersoft.SDK.Instant.Attributes;
    using Undersoft.SDK.Instant.Rubrics.Attributes;
    using Undersoft.SDK.Uniques;
    using System.Runtime.InteropServices;
    using System;

    [StructLayout(LayoutKind.Sequential)]
    public class FieldsAndPropertiesModel
    {
        public int Id { get; set; } = 404;

        [KeyRubric]
        public long Key = long.MaxValue;

        [InstantAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string Alias { get; set; } = "ProperSize";

        [InstantAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string Name { get; set; } = "SizeIsTwoTimesLonger";

        [InstantAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] ByteArray { get; set; }

        public double Factor { get; set; } = 2 * (long)int.MaxValue;

        public Guid GlobalId { get; set; } = new Guid();

        public bool Status { get; set; }

        public Uscn SystemKey { get; set; } = Uscn.New;

        public long time = 0;
        public DateTime Time
        {
            get => DateTime.FromBinary(time);
            set => time = value.ToBinary();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public class FieldsOnlyModel
    {
        [KeyRubric]
        public int Id = 404;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string Alias = "ProperSize";

        [InstantAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] ByteArray = new byte[10];
        public double Factor = 2 * (long)int.MaxValue;
        public Guid GlobalId = new Guid();
        public long Key = long.MaxValue;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string Name = "SizeIsTwoTimesLonger";

        [DisplayRubric("AvgPrice")]
        public double Price;
        public bool Status;
        public Uscn SystemKey = Uscn.Empty;
        public DateTime Time = DateTime.Now;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class PropertiesOnlyModel
    {
        [KeyRubric(IsAutoincrement = true, Order = 0)]
        public int Id { get; set; } = 405;

        [KeyRubric]
        public long Key { get; set; } = long.MaxValue;

        [InstantAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string Alias { get; set; } = "ProperSize";

        [InstantAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] ByteArray { get; set; }

        public double Factor { get; set; } = 2 * (long)int.MaxValue;

        public Guid GlobalId { get; set; } = new Guid();

        [KeyRubric(Order = 1)]
        [DisplayRubric("ProductName")]
        [InstantAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string Name { get; set; } = "SizeIsTwoTimesLonger";

        [DisplayRubric("AvgPrice")]
        public double Price { get; set; }

        public bool Status { get; set; }

        public Uscn SystemKey { get; set; } = Uscn.Empty;

        public DateTime Time { get; set; } = DateTime.Now;
    }
}
