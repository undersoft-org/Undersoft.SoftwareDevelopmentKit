using BenchmarkDotNet.Running;
using Undersoft.SDK.Benchmarks.Series;

namespace Undersoft.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            //  BenchmarkRunner.Run<MathsetBenchmark>();

            // var metod = new RegistryBenchmark();

            //  metod.Dictionary_Add_Test();

            BenchmarkRunner.Run<RegistryBenchmark>();

        }
    }
}
