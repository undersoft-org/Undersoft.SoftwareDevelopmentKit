using System.Series.Tests;

namespace Undersoft.SDK.IntegrationTests.Series
{
    using Undersoft.SDK.Series;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using Xunit;

    public class RegistryTest : CatalogTestHelper
    {
        public static object holder = new object();
        public static int threadCount = 0;
        public Task[] s1 = new Task[10];

        public RegistryTest() : base()
        {
            registry = new Registry64<string>();
            DefaultTraceListener Logfile = new DefaultTraceListener();
            Logfile.Name = "Logfile";
            Trace.Listeners.Add(Logfile);
            Logfile.LogFileName = $"Catalog64_{DateTime.Now.ToFileTime().ToString()}_Test.log";
        }

        [Fact]
        public async Task Catalog64_Concurrent_IndentifierKeys_TestAsync()
        {
            await catalog64_MultiThread_Test(identifierKeyTestCollection).ConfigureAwait(true);
        }

        [Fact]
        public async Task Catalog64_Concurrent_IntKeys_Test()
        {
            await catalog64_MultiThread_Test(intKeyTestCollection).ConfigureAwait(true);
        }

        [Fact]
        public async Task Catalog64_Concurrent_LongKeys_Test()
        {
            await catalog64_MultiThread_Test(longKeyTestCollection).ConfigureAwait(true);
        }

        [Fact]
        public async Task Catalog64_Concurrent_StringKeys_Test()
        {
            await catalog64_MultiThread_Test(stringKeyTestCollection).ConfigureAwait(true);
        }

        [Fact]
        public void Catalog64_IndentifierKeys_Test()
        {
            Catalog_Sync_Integrated_Test(identifierKeyTestCollection.Take(100000).ToArray());
        }

        [Fact]
        public void Catalog64_IntKeys_Test()
        {
            Catalog_Sync_Integrated_Test(intKeyTestCollection.Take(100000).ToArray());
        }

        [Fact]
        public void Catalog64_LongKeys_Test()
        {
            Catalog_Sync_Integrated_Test(longKeyTestCollection.Take(100000).ToArray());
        }

        [Fact]
        public void Catalog64_StringKeys_Test()
        {
            Catalog_Sync_Integrated_Test(stringKeyTestCollection.Take(100000).ToArray());
        }

        private void catalog64_MultiThread_TCallback_Test(Task[] t)
        {
            Debug.WriteLine($"Test Finished");
        }

        private Task catalog64_MultiThread_Test(IList<KeyValuePair<object, string>> collection)
        {
            Action publicTest = () =>
            {
                int c = 0;
                lock (holder)
                    c = threadCount++;

                Catalog_Async__ThreadIntegrated_Test(collection.Skip(c * 10000).Take(10000).ToArray());
            };

            for (int i = 0; i < 10; i++)
            {
                s1[i] = Task.Factory.StartNew(publicTest);
            }

            return Task.Factory.ContinueWhenAll(
                s1,
                new Action<Task[]>(a =>
                {
                    catalog64_MultiThread_TCallback_Test(a);
                })
            );
        }
    }
}
