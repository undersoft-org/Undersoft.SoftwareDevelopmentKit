namespace Undersoft.SDK.IntegrationTests.Instant.Math;

using System.Linq;
using Undersoft.SDK.Instant;
using Undersoft.SDK.Instant.Math;
using Undersoft.SDK.Instant.Math.Set;
using Undersoft.SDK.Instant.Proxies;
using Undersoft.SDK.Instant.Series;
using Undersoft.SDK.Instant.Series.Querying;
using Xunit;

public class InstantMathTest
{
    private InstantCreator instantCreator;
    private ProxySeriesCreator instatnProxiesCreator;
    private InstantMath instantMath;
    private IInstantSeries instantSeries;

    public InstantMathTest()
    {
        instatnProxiesCreator = new ProxySeriesCreator<InstantMathTestDataModel>();

        instantSeries = instatnProxiesCreator.Create();

        var price = nameof(InstantMathTestDataModel.NetPrice);
        var fee = nameof(InstantMathTestDataModel.SellFeeRate);

        for (int i = 0; i < 2000 * 1000; i++)
        {
            IProxy row = instantSeries.NewProxy();
            row.Target = new InstantMathTestDataModel();

            row[price] = (double)row[price] + i;
            row[fee] = (double)row[fee] / 2;
            instantSeries.Add(i, row);
        }
    }

    [Fact]
    public void InstantMath_Generic_Member_Computation_Test()
    {
        var genericInstantMath = new InstantMath<InstantMathTestDataModel>(instantSeries);

        var math0 = genericInstantMath[p => p.SellNetPrice];
        math0.Formula =
            math0[p => p.NetPrice] * (math0[p => p.SellFeeRate] / 100D) + math0[p => p.NetPrice];

        var math1 = genericInstantMath[p => p.SellGrossPrice];
        math1.Formula = math0 * math1[p => p.TaxRate];

        genericInstantMath.Compute();

        var standardCollectionMath = instantSeries.Cast<InstantMathTestDataModel>().ToArray();

        foreach (var sc in standardCollectionMath)
        {
            sc.SellNetPrice = sc.NetPrice * (sc.SellFeeRate / 100D) + sc.NetPrice;

            sc.SellGrossPrice = sc.SellNetPrice * sc.TaxRate;
        }
    }

    [Fact]
    public void InstantMath_Member_By_String_Computation_Test()
    {
        instantMath = new InstantMath(instantSeries);

        var ms0 = instantMath["SellNetPrice"];
        ms0.Formula = ms0["NetPrice"] * (ms0["SellFeeRate"] / 100D) + ms0["NetPrice"];

        var ms1 = instantMath["SellGrossPrice"];
        ms1.Formula = ms0 * ms1["TaxRate"];

        instantMath.Compute();

        var standardCollectionMath = instantSeries.Cast<InstantMathTestDataModel>().ToArray();

        foreach (var sc in standardCollectionMath)
        {
            sc.SellNetPrice = sc.NetPrice * (sc.SellFeeRate / 100D) + sc.NetPrice;

            sc.SellGrossPrice = sc.SellNetPrice * sc.TaxRate;
        }
    }

    [Fact]
    public void InstantMath_Generic_Member_Parallel_Computation_In_4_Chunks_Test()
    {
        var genericInstantMath = new InstantMath<InstantMathTestDataModel>(instantSeries);

        var math0 = genericInstantMath[p => p.SellNetPrice];
        math0.Formula =
            math0[p => p.NetPrice] * (math0[p => p.SellFeeRate] / 100D) + math0[p => p.NetPrice];

        var math1 = genericInstantMath[p => p.SellGrossPrice];
        math1.Formula = math0 * math1[p => p.TaxRate];

        genericInstantMath.ComputeInParallel(4);

        var standardCollectionMath = instantSeries.Cast<InstantMathTestDataModel>().ToArray();

        foreach (var sc in standardCollectionMath)
        {
            sc.SellNetPrice = sc.NetPrice * (sc.SellFeeRate / 100D) + sc.NetPrice;

            sc.SellGrossPrice = sc.SellNetPrice * sc.TaxRate;
        }
    }

    [Fact]
    public void InstantMath_Member_By_String_Computation_LogicOnStack_Test()
    {
        
        instantMath = new InstantMath(instantSeries);

        MathSet ml = instantMath["SellNetPrice"];

        ml.Formula =
            (ml["NetPrice"] < 10 | ml["NetPrice"] > 50)
            * (ml["NetPrice"] * (ml["SellFeeRate"] / 100) + ml["NetPrice"]);

        MathSet ml2 = instantMath["SellGrossPrice"];

        ml2.Formula = ml * ml2["TaxRate"];

        instantMath.Compute();

        instantMath.ComputeInParallel();

        var standardCollectionMath = instantSeries.Cast<InstantMathTestDataModel>().ToArray();

        foreach (var sc in standardCollectionMath)
        {
            if (sc.NetPrice < 10 || sc.NetPrice > 50)
            {
                sc.SellNetPrice = sc.NetPrice * (sc.SellFeeRate / 100D) + sc.NetPrice;
            }
            sc.SellGrossPrice = sc.SellNetPrice * sc.TaxRate;
        }
    }
}
