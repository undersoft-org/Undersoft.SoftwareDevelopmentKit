namespace Undersoft.SDK.Benchmarks.Instant.Math
{
    using BenchmarkDotNet.Attributes;
    using System.Linq;
    using Undersoft.SDK.Benchmarks.Instant.Math.Mocks;
    using Undersoft.SDK.Instant;
    using Undersoft.SDK.Instant.Math;
    using Undersoft.SDK.Instant.Math.Set;
    using Undersoft.SDK.Instant.Proxies;
    using Undersoft.SDK.Instant.Series;

    [MemoryDiagnoser]
    [RankColumn]
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
    [RPlotExporter]
    public class InstantMathBenchmark
    {
        private InstantCreator figure;
        private InstantSeriesCreator factory;
        private InstantMath seriesMathA;
        private InstantMath seriesMathB;
        private IInstantSeries InstantSeries;

        public InstantMathBenchmark()
        {
            factory = new ProxySeriesCreator<InstantSeriesMathMockModel>(
                "InstantSeriesCreator_InstantSeriesMath_Test"
            );

            InstantSeries = factory.Create();

            InstantSeriesMathMockModel fom = new InstantSeriesMathMockModel();

            for (int i = 0; i < 2000 * 1000; i++)
            {
                IProxy f = InstantSeries.NewProxy();
                f.Target = new InstantSeriesMathMockModel();

                f["NetPrice"] = (double)f["NetPrice"] + i;
                f["SellFeeRate"] = (double)f["SellFeeRate"] / 2;
                InstantSeries.Add(i, f);
            }

            seriesMathB = new InstantMath(InstantSeries);

            MathSet mathsetB0 = seriesMathB["SellNetPrice"];

            mathsetB0.Formula = mathsetB0["NetPrice"] * (mathsetB0["SellFeeRate"] / 100D) + mathsetB0["NetPrice"];

            MathSet mathsetB1 = seriesMathB["SellGrossPrice"];

            mathsetB1.Formula = mathsetB0 * mathsetB1["TaxRate"];

            seriesMathB.Compute();
        }

        public void InstantSeriesMath_AsSynchronic_ForEach_Series_Item_With_Compilation()
        {
            seriesMathA = new InstantMath(InstantSeries);

            MathSet mathsetA0 = seriesMathA["SellNetPrice"];

            mathsetA0.Formula = mathsetA0["NetPrice"] * (mathsetA0["SellFeeRate"] / 100D) + mathsetA0["NetPrice"];

            MathSet mathsetA1 = seriesMathA["SellGrossPrice"];

            mathsetA1.Formula = mathsetA0 * mathsetA1["TaxRate"];

            seriesMathA.Compute();
        }

        [Benchmark]
        public void InstantSeriesMath_AsSynchronic_ForEach_Series_Item_Without_Compilation()
        {
            seriesMathB.Compute();
        }

        [Benchmark]
        public void DotNet_BuildIn_RegularMathExpression_AsParellel_Enumerable_ForEach_Collection_Item()
        {
            InstantSeries
                .AsParallel()
                .ForEach(
                    (c) =>
                    {
                        c["SellNetPrice"] = (double)c["NetPrice"] * ((double)c["SellFeeRate"] / 100D) + (double)c["NetPrice"];

                        c["SellGrossPrice"] = (double)c["SellNetPrice"] * (double)c["TaxRate"];
                    }
                );
        }
    }
}
