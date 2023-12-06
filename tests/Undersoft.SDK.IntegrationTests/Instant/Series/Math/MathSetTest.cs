namespace Undersoft.SDK.IntegrationTests.Instant.Series.Math;

using System.Linq;
using Undersoft.SDK.Instant.Series.Math.Set;
using Undersoft.SDK.Instant.Series.Querying;
using Undersoft.SDK.IntegrationTests.Instant.Series.Math.Mocks;
using Xunit;

public class MathSetTest
{
    private InstantCreator instantCreator;
    private InstantSeriesCreator instatnSeriesCreator;
    private InstantSeriesMath instatnSeriesMath;
    private IInstantSeries instantSeries;

    public MathSetTest()
    {
        instatnSeriesCreator = new InstantSeriesCreator(
            typeof(MathsetMockModel),
            "InstantSeries_Mathset_Test"
        );

        instantSeries = instatnSeriesCreator.Combine();

        MathsetMockModel fom = new MathsetMockModel();

        for (int i = 0; i < 2000 * 1000; i++)
        {
            IProxy f = instantSeries.NewProxy();
            f.Target = new MathsetMockModel();

            f["NetPrice"] = (double)f["NetPrice"] + i;
            f["SellFeeRate"] = (double)f["SellFeeRate"] / 2;
            instantSeries.Add(i, f);
        }
    }

    [Fact]
    public void Mathset_Computation_Formula_Test()
    {
        instatnSeriesMath = new InstantSeriesMath(instantSeries);

        MathSet ml = instatnSeriesMath.GetMathSet("SellNetPrice");

        ml.Formula =
            ml[nameof(MathsetMockModel.NetPrice)] * (ml["SellFeeRate"] / 100D) + ml["NetPrice"];

        MathSet ml2 = instatnSeriesMath.GetMathSet("SellGrossPrice");

        ml2.Formula = ml * ml2["TaxRate"];

        instatnSeriesMath.Compute();

        var a = instantSeries
            .Query(c => (double)c["NetPrice"] > 10 && (double)c["NetPrice"] < 50)
            .ToArray();

        ml.Formula = ml["NetPrice"] * (ml["SellFeeRate"] / 95D) + ml["NetPrice"];

        instatnSeriesMath.Compute();

        var b = instantSeries
            .Query(c => (double)c["NetPrice"] < 10 || (double)c["NetPrice"] > 50)
            .ToArray();

        instantSeries
            .AsValues()
            .ForEach(
                (c) =>
                {
                    c["SellNetPrice"] =
                        (double)c["NetPrice"] * ((double)c["SellFeeRate"] / 100D)
                        + (double)c["NetPrice"];

                    c["SellGrossPrice"] = (double)c["SellNetPrice"] * (double)c["TaxRate"];
                }
            );
    }

    [Fact]
    public void Mathset_Computation_LogicOnStack_Formula_Test()
    {
        instantSeries
            .AsValues()
            .ForEach(
                (c) =>
                {
                    if ((double)c["NetPrice"] < 10 || (double)c["NetPrice"] > 50)
                    {
                        c["SellNetPrice"] =
                            (double)c["NetPrice"] * ((double)c["SellFeeRate"] / 100D)
                            + (double)c["NetPrice"];
                    }

                    c["SellGrossPrice"] = (double)c["SellNetPrice"] * (double)c["TaxRate"];
                }
            );

        instatnSeriesMath = new InstantSeriesMath(instantSeries);

        MathSet ml = instatnSeriesMath.GetMathSet("SellNetPrice");

        ml.Formula =
            (ml["NetPrice"] < 10 | ml["NetPrice"] > 50)
            * (ml["NetPrice"] * (ml["SellFeeRate"] / 100) + ml["NetPrice"])
        ;

        MathSet ml2 = instatnSeriesMath.GetMathSet("SellGrossPrice");

        ml2.Formula = ml * ml2["TaxRate"];

        instatnSeriesMath.Compute();

        instatnSeriesMath.Compute();
    }
}
