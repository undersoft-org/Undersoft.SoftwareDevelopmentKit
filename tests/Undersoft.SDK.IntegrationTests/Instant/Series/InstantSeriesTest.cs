namespace Undersoft.SDK.IntegrationTests.Instant
{
    using System.Collections.Generic;
    using System.Reflection;
    using Undersoft.SDK.Instant;
    using Undersoft.SDK.Instant.Proxies;
    using Undersoft.SDK.Instant.Series;
    using Xunit;

    public class InstantSeriesTest
    {
        private IInstant instant;
        private IInstantSeries instantSeries;
        private InstantSeriesCreator instantSeriesCreator;
        private InstantCreator instantCreator;

        [Fact]
        public void InstantSeries_Compile_Test()
        {
            var Instant2 = new ProxyCreator<FieldsAndPropertiesModel>();

            FieldsAndPropertiesModel fom = new FieldsAndPropertiesModel();

            var IInstant2 = Proxy_Compilation_Helper_Test(Instant2, fom);

            var IProxy = new FieldsAndPropertiesModel().ToProxy();

            var InstantSeries2 = new InstantProxiesCreator<FieldsAndPropertiesModel>();

            var rttab = InstantSeries2.Combine();

            for (int i = 0; i < 10000; i++)
            {
                rttab.Add((long)int.MaxValue + i, rttab.NewInstant());
            }

            for (int i = 9999; i > -1; i--)
            {
                rttab[i] = rttab.Get(i + (long)int.MaxValue);
            }
        }

        [Fact]
        public void InstantSeries_MutatorAndAccessorById_Test()
        {
            instantCreator = new InstantCreator(typeof(FieldsAndPropertiesModel));
            FieldsAndPropertiesModel fom = new FieldsAndPropertiesModel();
            instant = Instant_Compilation_Helper_Test(instantCreator, fom);

            instantSeriesCreator = new InstantSeriesCreator(
                instantCreator,
                "InstantSequence_Compilation_Test"
            );

            instantSeries = instantSeriesCreator.Combine();

            instantSeries.Add(instantSeries.NewInstant());
            instantSeries[0, 4] = instant[4];

            Assert.Equal(instant[4], instantSeries[0, 4]);
        }

        [Fact]
        public void InstantSeries_MutatorAndAccessorByName_Test()
        {
            instantCreator = new InstantCreator(typeof(FieldsAndPropertiesModel));
            FieldsAndPropertiesModel fom = new FieldsAndPropertiesModel();
            instant = Instant_Compilation_Helper_Test(instantCreator, fom);

            instantSeriesCreator = new InstantSeriesCreator(
                instantCreator,
                "InstantSequence_Compilation_Test"
            );

            instantSeries = instantSeriesCreator.Combine();

            instantSeries.Add(instantSeries.NewInstant());
            instantSeries[0, nameof(fom.Name)] = instant[nameof(fom.Name)];

            Assert.Equal(
                instant[nameof(fom.Name)],
                instantSeries[0, nameof(fom.Name)]
            );
        }

        [Fact]
        public void InstantSeries_NewItem_Test()
        {
            instantCreator = new InstantCreator(typeof(FieldsAndPropertiesModel));
            FieldsAndPropertiesModel fom = new FieldsAndPropertiesModel();
            instant = Instant_Compilation_Helper_Test(instantCreator, fom);

            instantSeriesCreator = new InstantSeriesCreator(
                instantCreator,
                "InstantSequence_Compilation_Test"
            );

            instantSeries = instantSeriesCreator.Combine();

            IInstant rcst = instantSeries.NewInstant();

            Assert.NotNull(rcst);
        }

        [Fact]
        public void InstantSeries_SetRubrics_Test()
        {
            instantCreator = new InstantCreator(typeof(FieldsAndPropertiesModel));
            FieldsAndPropertiesModel fom = new FieldsAndPropertiesModel();
            instant = Instant_Compilation_Helper_Test(instantCreator, fom);

            instantSeriesCreator = new InstantSeriesCreator(
                instantCreator,
                "InstantSequence_Compilation_Test"
            );

            var rttab = instantSeriesCreator.Combine();

            Assert.Equal(rttab.Rubrics, instantSeriesCreator.Rubrics);
        }

        private IInstant Instant_Compilation_Helper_Test(
            InstantCreator Instant,
            FieldsAndPropertiesModel fom
        )
        {
            IInstant rts = Instant.Create();

            for (int i = 1; i < Instant.Rubrics.Count; i++)
            {
                var r = Instant.Rubrics[i].RubricInfo;
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
            return rts;
        }

        private IInstant Proxy_Compilation_Helper_Test(
            ProxyCreator Instant2,
            FieldsAndPropertiesModel fom
        )
        {
            List<IProxy> list = new List<IProxy>();
            for (int y = 0; y < 300000; y++)
            {
                var rts0 = Instant2.Create();

                for (int i = 1; i < Instant2.Rubrics.Count; i++)
                {
                    var r = Instant2.Rubrics[i].RubricInfo;
                    if (r.MemberType == MemberTypes.Field)
                    {
                        var fi = fom.GetType().GetField(((FieldInfo)r).Name);
                        if (fi != null)
                            rts0[r.Name] = fi.GetValue(fom);
                    }
                    if (r.MemberType == MemberTypes.Property)
                    {
                        var pi = fom.GetType().GetProperty(((PropertyInfo)r).Name);
                        if (pi != null)
                            rts0[r.Name] = pi.GetValue(fom);
                    }
                }

                list.Add(rts0);
            }
            return list[0];
        }
    }
}
