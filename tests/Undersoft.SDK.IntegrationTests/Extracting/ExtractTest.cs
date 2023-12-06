using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Xunit;

namespace Undersoft.SDK.IntegrationTests.Extracting;

using Instant.Series;
using Mocks;

public class ExtractTest
{
    internal byte[] destinationBuffer = new byte[ushort.MaxValue];
    internal byte[] sourceBuffer = new byte[ushort.MaxValue];
    internal IInstant instant = null;
    internal IInstantSeries instantSeries = null;
    internal InstantCreator instantCreator = null;
    internal byte[] serializedBytesA = null;
    internal byte[] serializedBytesB = null;
    internal InstantSeriesCreator instantSeriesCreator = null;

    public ExtractTest()
    {
        Random r = new Random();
        r.NextBytes(sourceBuffer);

        instantCreator = new InstantCreator(
            InstantMocks.Instant_MemberRubric_FieldsAndPropertiesModel(),
            "Instant_MemberRubric_FieldsAndPropertiesModel_ValueType"
        );
        FieldsAndPropertiesModel fom = new FieldsAndPropertiesModel();
        instantSeriesCreator = new InstantSeriesCreator(instantCreator, "InstantSeries_Compilation_Test");
        instantSeries = instantSeriesCreator.Combine();

        instant = instantSeries.NewInstant();

        foreach (var rubric in instantCreator.Rubrics.AsValues())
        {
            if (rubric.FieldId > -1)
            {
                var field = fom.GetType()
                    .GetField(
                        rubric.FigureField.Name,
                        BindingFlags.NonPublic | BindingFlags.Instance
                    );
                if (field == null)
                    field = fom.GetType().GetField(rubric.RubricName);
                if (field == null)
                {
                    var prop = fom.GetType().GetProperty(rubric.RubricName);
                    if (prop != null)
                        instant[rubric.FieldId] = prop.GetValue(fom);
                }
                else
                    instant[rubric.FieldId] = field.GetValue(fom);
            }
        }

        for (int i = 0; i < 1000; i++)
        {
            IInstant nrcstr = instantSeries.NewInstant();
            instantSeries.Add(i, nrcstr);
        }

        serializedBytesA = new byte[instantSeries.InstantSize];
        serializedBytesB = new byte[instantSeries.InstantSize];

        instant.StructureTo(ref serializedBytesA, 0);
    }

    [Fact]
    public unsafe void Extract_BytesToStruct_FromType_Test()
    {
        object os = instantSeries.NewInstant();
        Extract.BytesToStructure(serializedBytesA, os, 0);
        bool equal = instant.StructureEqual(os);
        Assert.True(equal);
    }

    [Fact]
    public unsafe void Extract_CopyBlock_ByteArray_UInt_Test()
    {
        Random r = new Random();
        r.NextBytes(sourceBuffer);
        destinationBuffer.Initialize();

        Extract.CopyBlock(destinationBuffer, 0, sourceBuffer, 0, sourceBuffer.Length);
        bool equal = destinationBuffer.BlockEqual(sourceBuffer);
        Assert.True(equal);
    }

    [Fact]
    public unsafe void Extract_CopyBlock_ByteArray_Ulong_Test()
    {
        Random r = new Random();
        r.NextBytes(sourceBuffer);
        destinationBuffer.Initialize();

        Extract.CopyBlock(destinationBuffer, 0, sourceBuffer, 0, (ulong)sourceBuffer.Length);
        bool equal = destinationBuffer.BlockEqual(sourceBuffer);
        Assert.True(equal);
    }

    [Fact]
    public unsafe void Extract_CopyBlock_Pointer_UInt_Test()
    {
        Random r = new Random();
        r.NextBytes(sourceBuffer);
        destinationBuffer.Initialize();

        fixed (
            byte* psrc = sourceBuffer,
                pdst = destinationBuffer
        )
        {
            Extract.CopyBlock(pdst, 0, psrc, 0, sourceBuffer.Length);
            bool equal = destinationBuffer.BlockEqual(sourceBuffer);
            Assert.True(equal);
        }
    }

    [Fact]
    public unsafe void Extract_CopyBlock_Pointer_Ulong_Test()
    {
        Random r = new Random();
        r.NextBytes(sourceBuffer);
        destinationBuffer.Initialize();

        Extract.CopyBlock(destinationBuffer, 0, sourceBuffer, 0, sourceBuffer.Length);
        bool equal = destinationBuffer.BlockEqual(sourceBuffer);
        Assert.True(equal);
    }

    [Fact]
    public unsafe void Extract_PointerToStructure_Type_Test()
    {
        fixed (byte* b = serializedBytesA)
        {
            object os = Extract.PointerToStructure(b, instantSeries.InstantType, 0);
            bool equal = instant.StructureEqual(os);
            Assert.True(equal);
        }
    }

    [Fact]
    public unsafe void Extract_PointerToStructure_Test()
    {
        fixed (byte* b = serializedBytesA)
        {
            object os = instantSeries.NewInstant();
            Extract.PointerToStructure(b, os);
            bool equal = instant.StructureEqual(os);
            Assert.True(equal);
        }
    }

    [Fact]
    public unsafe void Extract_StructModel_Test()
    {
        Assemblies.ResolveExecuting();

        StructModel[] structure = new StructModel[]
        {
            new StructModel(83948930),
            new StructModel(45453),
            new StructModel(5435332)
        };
        structure[0].Alias = "FirstAlias";
        structure[0].Name = "FirstName";
        structure[1].Alias = "SecondAlia";
        structure[1].Name = "SecondName";
        structure[2].Alias = "ThirdAlias";
        structure[2].Name = "ThirdName";

        StructModels structures = new StructModels(structure);

        int size = Marshal.SizeOf(structure[0]);

        byte* pserial = Extract.GetStructurePointer(structure[0]);

        StructModel structure2 = new StructModel();
        ValueType o = structure2;

        o = Extract.PointerToStructure(pserial, o);

        structure2 = (StructModel)o;

        structure2.Alias = "FirstChange";
    }

    [Fact]
    public unsafe void Extract_StructToBytesArray_Test()
    {
        byte[] b = instant.GetStructureBytes();
        bool equal = b.BlockEqual(serializedBytesA);
        Assert.True(equal);
        object ro = instant;
        serializedBytesB = new byte[instant.GetSize()];
        Extract.StructureToBytes(ro, ref serializedBytesB, 0);
        bool equal2 = serializedBytesB.BlockEqual(serializedBytesA);
        Assert.True(equal2);
    }

    [Fact]
    public unsafe void Extract_StructToPointer_Test()
    {
        GCHandle gcptr = GCHandle.Alloc(serializedBytesA, GCHandleType.Pinned);
        byte* ptr = (byte*)gcptr.AddrOfPinnedObject();

        instant.StructureTo(ptr);

        instant["Id"] = 88888;
        instant["Name"] = "Zmiany";

        instant.StructureTo(ptr);

        instant["Id"] = 5555555;
        instant["Name"] = "Zm342";

        instant.StructureFrom(ptr);

        Assert.Equal(88888, instant["Id"]);
    }
}
