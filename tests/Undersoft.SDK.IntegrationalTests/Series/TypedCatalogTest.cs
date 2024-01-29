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
    using Undersoft.SDK.IntegrationTests.Instant;

    public class TypedCatalogTest : TypedCatalogTestHelper
    {
        public static object holder = new object();
        public static int threadCount = 0;
        public Task[] s1 = new Task[10];

        public TypedCatalogTest() : base()
        {
            typedRegistry = new TypedCatalog<Agreement>();
            DefaultTraceListener Logfile = new DefaultTraceListener();
            Logfile.Name = "Logfile";
            Trace.Listeners.Add(Logfile);
            Logfile.LogFileName = $"Registry__{DateTime.Now.ToFileTime().ToString()}_Test.log";
        }

        [Fact]
        public async Task Typed_Catalog_Async_Thread_Safe_Integrated_Test()
        {
            await Typed_Catalog_Async_Thread_Safe_Integrated_Test_Startup(identifiableObjectTestCollection).ConfigureAwait(true);
        }

        [Fact]
        public void Typed_Catalog_Integrational_Test()
        {
            Typed_Catalog_Sync_Integrated_Test_Helper(identifiableObjectTestCollection.Take(100000).ToArray());
        }

        private void Typed_Catalog_Async_Thread_Safe_Integrated_Test_Callback(Task[] t)
        {
            Debug.WriteLine($"Test Finished");
        }

        private Task Typed_Catalog_Async_Thread_Safe_Integrated_Test_Startup(IList<Agreement> collection)
        {
            Action publicTest = () =>
            {
                int c = 0;
                lock (holder)
                    c = threadCount++;

                Typed_Catalog_Async_Thread_Safe_Integrated_Test_Helper(collection.Skip(c * 10000).Take(10000).ToArray());
            };

            for (int i = 0; i < 10; i++)
            {
                s1[i] = Task.Factory.StartNew(publicTest);
            }

            return Task.Factory.ContinueWhenAll(
                s1,
                new Action<Task[]>(a =>
                {
                    Typed_Catalog_Async_Thread_Safe_Integrated_Test_Callback(a);
                })
            );
        }
    }
}
