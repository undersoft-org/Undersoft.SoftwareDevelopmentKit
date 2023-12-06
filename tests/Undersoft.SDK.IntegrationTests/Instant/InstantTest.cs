using System;


namespace Undersoft.SDK.IntegrationTests.Instant
{
    using Undersoft.SDK.Instant;
    using Undersoft.SDK.Uniques;
    using System.Reflection;

    using Xunit;

    public class InstantTest
    {
        public InstantTest() { }

        [Fact]
        public void Instant_Memberinfo_FieldsAndPropertiesModel_Compilation_Test()
        {
            InstantCreator derivedType = new InstantCreator<Agreement>(InstantType.Derived);
            IInstant InstantA = Instant_Compilation_Helper_Test(derivedType, new Agreement());

            InstantCreator referenceType = new InstantCreator<Agreement>();
            IInstant InstantB = Instant_Compilation_Helper_Test(referenceType, new Agreement());

            //  InstantCreator valueType = new InstantCreator<Agreement>(InstantType.ValueType);
            //  IInstant InstantC = Instant_Compilation_Helper_Test(valueType, new Agreement());
        }

        [Fact]
        public void Instant_Memberinfo_FieldsOnlyModel_Compilation_Test()
        {
            InstantCreator derivedType = new InstantCreator(typeof(FieldsOnlyModel), InstantType.Derived);
            IInstant InstantA = Instant_Compilation_Helper_Test(derivedType, new FieldsOnlyModel());

            InstantCreator referenceType = new InstantCreator(typeof(FieldsOnlyModel), InstantType.Reference);
            IInstant InstantB = Instant_Compilation_Helper_Test(referenceType, new FieldsOnlyModel());

            InstantCreator valueType = new InstantCreator(typeof(FieldsOnlyModel), InstantType.ValueType);
            IInstant InstantC = Instant_Compilation_Helper_Test(valueType, new FieldsOnlyModel());
        }

        [Fact]
        public void Instant_Memberinfo_PropertiesOnlyModel_Compilation_Test()
        {
            InstantCreator derivedType = new InstantCreator(typeof(PropertiesOnlyModel), InstantType.Derived);
            IInstant InstantA = Instant_Compilation_Helper_Test(
                derivedType,
                new PropertiesOnlyModel()
            );

            InstantCreator referenceType = new InstantCreator(typeof(PropertiesOnlyModel), InstantType.Reference);
            IInstant InstantB = Instant_Compilation_Helper_Test(
                referenceType,
                new PropertiesOnlyModel()
            );

            InstantCreator valueType = new InstantCreator(typeof(PropertiesOnlyModel), InstantType.ValueType);
            IInstant InstantC = Instant_Compilation_Helper_Test(valueType, new PropertiesOnlyModel());
        }

        [Fact]
        public void Instant_MemberRubric_FieldsOnlyModel_Compilation_Test()
        {
            InstantCreator referenceType = new InstantCreator(
                InstantMocks.Instant_MemberRubric_FieldsOnlyModel(),
                "Instant_MemberRubric_FieldsOnlyModel_Reference"
            );
            FieldsOnlyModel fom = new FieldsOnlyModel();
            IInstant InstantA = Instant_Compilation_Helper_Test(referenceType, fom);

            InstantCreator valueType = new InstantCreator(
                InstantMocks.Instant_Memberinfo_FieldsOnlyModel(),
                "Instant_MemberRubric_FieldsOnlyModel_ValueType",
                 InstantType.ValueType
            );
            fom = new FieldsOnlyModel();
            IInstant InstantB = Instant_Compilation_Helper_Test(valueType, fom);
        }

        [Fact]
        public void Instant_ValueArray_GetSet_Test()
        {
            InstantCreator referenceType = new InstantCreator(typeof(FieldsAndPropertiesModel));

            Instant_Compilation_Helper_Test(
                referenceType,
                Instant_Compilation_Helper_Test(referenceType, new FieldsAndPropertiesModel())
            );

            InstantCreator valueType = new InstantCreator(typeof(PropertiesOnlyModel), InstantType.ValueType);

            Instant_Compilation_Helper_Test(
                valueType,
                Instant_Compilation_Helper_Test(valueType, new FieldsAndPropertiesModel())
            );
        }

        private IInstant Instant_Compilation_Helper_Test(InstantCreator str, Agreement fom)
        {
            IInstant rts = str.Create();
            fom.Kind = AgreementKind.Agree;
            rts[0] = 1;
            Assert.Equal(fom.Kind, rts[0]);
            rts["Id"] = 555L;
            Assert.NotEqual(fom.Id, rts[nameof(fom.Id)]);
            rts[nameof(fom.Comments)] = fom.Comments;
            Assert.Equal(fom.Comments, rts[nameof(fom.Comments)]);
            rts.Code = new Uscn(DateTime.Now.ToBinary());
            string hexTetra = rts.Code.ToString();
            Uscn ssn = new Uscn(hexTetra);
            Assert.Equal(ssn, rts.Code);

            for (int i = 1; i < str.Rubrics.Count; i++)
            {
                var ri = str.Rubrics[i].RubricInfo;
                if (ri.MemberType == MemberTypes.Field)
                {
                    var fi = fom.GetType().GetField(((FieldInfo)ri).Name);
                    if (fi != null)
                        rts[ri.Name] = fi.GetValue(fom);
                }
                if (ri.MemberType == MemberTypes.Property)
                {
                    var pi = fom.GetType().GetProperty(((PropertyInfo)ri).Name);
                    if (pi != null)
                    {
                        var value = pi.GetValue(fom);
                        if (value != null)
                            rts[ri.Name] = value;
                    }
                }
            }

            for (int i = 1; i < str.Rubrics.Count; i++)
            {
                var r = str.Rubrics[i].RubricInfo;
                if (r.MemberType == MemberTypes.Field)
                {
                    var fi = fom.GetType().GetField(((FieldInfo)r).Name);
                    if (fi != null)
                        Assert.Equal(rts[r.Name], fi.GetValue(fom));
                }
                if (r.MemberType == MemberTypes.Property)
                {
                    var pi = fom.GetType().GetProperty(((PropertyInfo)r).Name);
                    if (pi != null)
                    {
                        var value = pi.GetValue(fom);
                        if (value != null)
                            Assert.Equal(rts[r.Name], pi.GetValue(fom));
                    }
                }
            }
            return rts;
        }

        private IInstant Instant_Compilation_Helper_Test(InstantCreator str, FieldsAndPropertiesModel fom)
        {
            IInstant rts = str.Create();
            fom.Id = 202;
            rts[0] = 202;
            Assert.Equal(fom.Id, rts[0]);
            rts["Id"] = 404;
            Assert.NotEqual(fom.Id, rts[nameof(fom.Id)]);
            rts[nameof(fom.Name)] = fom.Name;
            Assert.Equal(fom.Name, rts[nameof(fom.Name)]);
            rts.Code = new Uscn(DateTime.Now.ToBinary());
            string hexTetra = rts.Code.ToString();
            Uscn ssn = new Uscn(hexTetra);
            Assert.Equal(ssn, rts.Code);

            for (int i = 1; i < str.Rubrics.Count; i++)
            {
                var ri = str.Rubrics[i].RubricInfo;
                if (ri.MemberType == MemberTypes.Field)
                {
                    var fi = fom.GetType().GetField(((FieldInfo)ri).Name);
                    if (fi != null)
                        rts[ri.Name] = fi.GetValue(fom);
                }
                if (ri.MemberType == MemberTypes.Property)
                {
                    var pi = fom.GetType().GetProperty(((PropertyInfo)ri).Name);
                    if (pi != null)
                        rts[ri.Name] = pi.GetValue(fom);
                }
            }

            for (int i = 1; i < str.Rubrics.Count; i++)
            {
                var r = str.Rubrics[i].RubricInfo;
                if (r.MemberType == MemberTypes.Field)
                {
                    var fi = fom.GetType().GetField(((FieldInfo)r).Name);
                    if (fi != null)
                        Assert.Equal(rts[r.Name], fi.GetValue(fom));
                }
                if (r.MemberType == MemberTypes.Property)
                {
                    var pi = fom.GetType().GetProperty(((PropertyInfo)r).Name);
                    if (pi != null)
                        Assert.Equal(rts[r.Name], pi.GetValue(fom));
                }
            }
            return rts;
        }

        private IInstant Instant_Compilation_Helper_Test(InstantCreator str, FieldsOnlyModel fom)
        {
            IInstant rts = str.Create();
            fom.Id = 202;
            rts["Id"] = 404;
            Assert.NotEqual(fom.Id, rts[nameof(fom.Id)]);
            rts[nameof(fom.Name)] = fom.Name;
            Assert.Equal(fom.Name, rts[nameof(fom.Name)]);

            rts.Code = new Uscn(DateTime.Now.ToBinary());
            string hexTetra = rts.Code.ToString();
            Uscn ssn = new Uscn(hexTetra);
            Assert.Equal(ssn, rts.Code);

            for (int i = 1; i < str.Rubrics.Count; i++)
            {
                var r = str.Rubrics[i].RubricInfo;
                if (r.MemberType == MemberTypes.Field)
                {
                    var fi = fom.GetType().GetField(((FieldInfo)r).Name);
                    if (fi != null)
                        rts[r.Name] = fi.GetValue(fom);
                }
                if (r.MemberType == MemberTypes.Property)
                {
                    var pi = fom.GetType().GetProperty(((PropertyInfo)r).Name);
                    if (pi != null)
                        rts[r.Name] = pi.GetValue(fom);
                }
            }

            for (int i = 1; i < str.Rubrics.Count; i++)
            {
                var r = str.Rubrics[i].RubricInfo;
                if (r.MemberType == MemberTypes.Field)
                {
                    var fi = fom.GetType().GetField(((FieldInfo)r).Name);
                    if (fi != null)
                        Assert.Equal(rts[r.Name], fi.GetValue(fom));
                }
                if (r.MemberType == MemberTypes.Property)
                {
                    var pi = fom.GetType().GetProperty(((PropertyInfo)r).Name);
                    if (pi != null)
                        Assert.Equal(rts[r.Name], pi.GetValue(fom));
                }
            }
            return rts;
        }

        private void Instant_Compilation_Helper_Test(InstantCreator str, IInstant Instant)
        {
            //IInstant rts = str.Combine();
            //object[] values = rts.ValueArray;
            //rts.ValueArray = Instant.ValueArray;
            //for (int i = 0; i < values.Length; i++)
            //    Assert.Equal(Instant[i], rts.ValueArray[i]);
        }

        private IInstant Instant_Compilation_Helper_Test(InstantCreator str, PropertiesOnlyModel fom)
        {
            IInstant rts = str.Create();
            fom.Id = 202;
            rts["Id"] = 404;
            Assert.NotEqual(fom.Id, rts[nameof(fom.Id)]);
            rts[nameof(fom.Name)] = fom.Name;
            Assert.Equal(fom.Name, rts[nameof(fom.Name)]);
            rts.Code = new Uscn(DateTime.Now.ToBinary());
            string hexTetra = rts.Code.ToString();
            Uscn ssn = new Uscn(hexTetra);
            Assert.Equal(ssn, rts.Code);

            for (int i = 1; i < str.Rubrics.Count; i++)
            {
                var r = str.Rubrics[i].RubricInfo;
                if (r.MemberType == MemberTypes.Field)
                {
                    var fi = fom.GetType().GetField(((FieldInfo)r).Name);
                    if (fi != null)
                        rts[r.Name] = fi.GetValue(fom);
                }
                if (r.MemberType == MemberTypes.Property)
                {
                    var pi = fom.GetType().GetProperty(((PropertyInfo)r).Name);
                    if (pi != null)
                        rts[r.Name] = pi.GetValue(fom);
                }
            }

            for (int i = 1; i < str.Rubrics.Count; i++)
            {
                var r = str.Rubrics[i].RubricInfo;
                if (r.MemberType == MemberTypes.Field)
                {
                    var fi = fom.GetType().GetField(((FieldInfo)r).Name);
                    if (fi != null)
                        Assert.Equal(rts[r.Name], fi.GetValue(fom));
                }
                if (r.MemberType == MemberTypes.Property)
                {
                    var pi = fom.GetType().GetProperty(((PropertyInfo)r).Name);
                    if (pi != null)
                        Assert.Equal(rts[r.Name], pi.GetValue(fom));
                }
            }
            return rts;
        }
    }
}
