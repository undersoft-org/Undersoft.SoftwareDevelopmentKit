using System;
using System.Series.Tests;

namespace Undersoft.SDK.Benchmarks.Series
{
    using BenchmarkDotNet.Attributes;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Undersoft.SDK.Series;

    public class CatalogBenchmark : CatalogTestHelper
    {
        public static int threadCount = 0;
        public object holder = new object();
        public Task[] s1 = new Task[6];

        public CatalogBenchmark() : base()
        {
            registry = new Catalog<string>();
            DefaultTraceListener Logfile = new DefaultTraceListener();
            Logfile.Name = "Logfile";
            Trace.Listeners.Add(Logfile);
            Logfile.LogFileName = $"Catalog_{DateTime.Now.ToFileTime().ToString()}_Test.log";
        }

        [Benchmark]
        public async Task Catalog_Concurrent_IndentifierKeys_Test()
        {
            Task t = Catalog_MultiThread_Test(identifierKeyTestCollection);
            await t.ConfigureAwait(true);
        }

        [Benchmark]
        public async Task Catalog_Concurrent_IntKeys_Test()
        {
            Task t = Catalog_MultiThread_Test(intKeyTestCollection);
            await t.ConfigureAwait(true);
        }

        [Benchmark]
        public async Task Catalog_Concurrent_LongKeys_Test()
        {
            Task t = Catalog_MultiThread_Test(longKeyTestCollection);
            await t.ConfigureAwait(true);
        }

        [Benchmark]
        public async Task Catalog_Concurrent_StringKeys_Test()
        {
            Task t = Catalog_MultiThread_Test(stringKeyTestCollection);
            await t.ConfigureAwait(true);
        }

        [Benchmark]
        public void Catalog_IndentifierKeys_Test()
        {
            SharedDeck_ThreadIntegrated_Test(identifierKeyTestCollection.Take(100000).ToArray());
        }

        [Benchmark]
        public void Catalog_IntKeys_Test()
        {
            SharedDeck_ThreadIntegrated_Test(intKeyTestCollection.Take(100000).ToArray());
        }

        [Benchmark]
        public void Catalog_LongKeys_Test()
        {
            SharedDeck_ThreadIntegrated_Test(longKeyTestCollection.Take(100000).ToArray());
        }

        [Benchmark]
        public void Catalog_StringKeys_Test()
        {
            SharedDeck_ThreadIntegrated_Test(stringKeyTestCollection.Take(100000).ToArray());
        }

        private Task Catalog_MultiThread_Test(IList<KeyValuePair<object, string>> collection)
        {
            registry = new Catalog<string>();
            Action publicTest = () =>
            {
                int c = 0;
                lock (holder)
                    c = threadCount++;

                SharedDeck_ThreadIntegrated_Test(collection.Skip(c * 10000).Take(10000).ToArray());
            };

            for (int i = 0; i < 6; i++)
            {
                s1[i] = Task.Factory.StartNew(publicTest);
            }

            return Task.Factory.ContinueWhenAll(
                s1,
                new Action<Task[]>(a =>
                {
                    publicBoard_MultiThread_TCallback_Test(a);
                })
            );
        }

        private void publicBoard_MultiThread_TCallback_Test(Task[] t)
        {
            Debug.WriteLine($"Test Finished");
        }
    }
}
