using System.Series.Tests;

namespace Undersoft.SDK.Benchmarks.Series
{
    using BenchmarkDotNet.Attributes;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Undersoft.SDK.Series;

    [MemoryDiagnoser]
    [RankColumn]
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
    [RPlotExporter]
    public class RegistryBenchmark
    {
        public static object holder = new object();
        public static int threadCount = 0;
        public Task[] s1 = new Task[10];
        public BenchmarkDictionaryHelper dhelper = new BenchmarkDictionaryHelper();
        public BenchmarkHelper chelper = new BenchmarkHelper();
        public IList<KeyValuePair<object, string>> collection;

        public RegistryBenchmark()
        {
            Setup();
        }

        [GlobalSetup]
        public void Setup()
        {
            dhelper = new BenchmarkDictionaryHelper();
            chelper = new BenchmarkHelper();

            chelper.registry = new Registry<string>();

            DefaultTraceListener Logfile = new DefaultTraceListener();
            Logfile.Name = "Logfile";
            Trace.Listeners.Add(Logfile);
            Logfile.LogFileName = $"Catalog64_{DateTime.Now.ToFileTime().ToString()}_Test.log";

            collection = dhelper.identifierKeyTestCollection;
        }

        [Benchmark]
        public void Registry_Add_Test()
        {
            chelper.Add_Test(collection);
        }

        [Benchmark]
        public void Registry_GetByKey_Test()
        {
            chelper.GetByKey_From_Indexer_Test(collection);
        }

        [Benchmark]
        public void Registry_ContainsKey_Test()
        {
            chelper.ContainsKey_Test(collection);
        }

        [Benchmark]
        public void Registry_GetByIndex_Test()
        {
            chelper.GetByIndexer_Test(collection);
        }

        [Benchmark]
        public void Registry_Iteration_Test()
        {
            chelper.Iteration_Test(collection);
        }

        [Benchmark]
        public void Registry_Remove_Test()
        {
            chelper.Remove_Test(collection);
        }

        [Benchmark]
        public void Dictionary_Add_Test()
        {
            dhelper.Add_Test(collection);
        }

        [Benchmark]
        public void Dictionary_GetByKey_Test()
        {
            dhelper.GetByKey_Test(collection);
        }

        [Benchmark]
        public void Dictionary_ContainsKey_Test()
        {
            dhelper.ContainsKey_Test(collection);
        }

        [Benchmark]
        public void Dictionary_Contains_Test()
        {
            dhelper.Contains_Test(collection);
        }

        [Benchmark]
        public void Registry_Contains_Test()
        {
            chelper.Contains_Test(collection);
        }

        [Benchmark]
        public void Dictionary_GetByIndex_Test()
        {
            dhelper.GetByIndex_Test(collection);
        }

        [Benchmark]
        public void Dictionary_GetLast_Test()
        {
            dhelper.GetLast_Test(collection);
        }

        [Benchmark]
        public void Registry_GetLast_Test()
        {
            chelper.Last_Test(null);
        }

        [Benchmark]
        public void Dictionary_Remove_Test()
        {
            dhelper.Remove_Test(collection);
        }

        [Benchmark]
        public void Dictionary_Iteration_Test()
        {
            dhelper.Iteration_Test(collection);
        }
    }
}
