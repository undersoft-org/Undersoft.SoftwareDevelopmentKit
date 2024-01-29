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
    using Undersoft.SDK.Uniques;

    public class TypedRegistryTest : TypedRegistryTestHelper
    {
        public static object holder = new object();
        public static int threadCount = 0;
        public Task[] s1 = new Task[10];

        public TypedRegistryTest() : base()
        {
            typedRegistry = new TypedRegistry<Agreement>();
            DefaultTraceListener Logfile = new DefaultTraceListener();
            Logfile.Name = "Logfile";
            Trace.Listeners.Add(Logfile);
            Logfile.LogFileName = $"Registry__{DateTime.Now.ToFileTime().ToString()}_Test.log";
        }

        [Fact]
        public async Task Typed_Registry_Async_Thread_Safe_Integrated_Test()
        {
            await Typed_Registry_Async_Thread_Safe_Integrated_Test_Startup(identifiableObjectTestCollection).ConfigureAwait(true);
        }

        [Fact]
        public void Typed_Registry_Sync_Integrated_Test()
        {
            Typed_Registry_Sync_Integrated_Test_Helper(identifiableObjectTestCollection.Take(100000).ToArray());
        }

        [Fact]
        public void Typed_Registry_Seeded_Hash_Keys_Test()
        {
            var uniques = Unique.Bit64;
            var list = identifiableObjectTestCollection.Take(100000).ToArray();
            var key0 = uniques.Key(list[0], list[0].TypeId);
            var key1 = uniques.Key(list[0], list[0].TypeId);
            
            Assert.Equal(key0, key1);
            
            typedRegistry.Add(key0, list[0]);
            
            var item = typedRegistry.Get(key1);

            Assert.NotNull(item);
            
            var key2 = uniques.Key(list[1], list[1].TypeId);
            var key3 = uniques.Key(list[1], list[1].TypeId);

            Assert.Equal(key2, key3);

            typedRegistry.Add(list[1]);

            item = typedRegistry.Get(key2);

            Assert.NotNull(item);

            item = typedRegistry.Get(key3);

            Assert.NotNull(item);

            item = typedRegistry.Get(list[1]);

            Assert.NotNull(item);
        }

        private void Typed_Registry_Async_Thread_Safe_Integrated_Test_Callback(Task[] t)
        {
            Debug.WriteLine($"Test Finished");
        }

        private Task Typed_Registry_Async_Thread_Safe_Integrated_Test_Startup(IList<Agreement> collection)
        {
            Action publicTest = () =>
            {
                int c = 0;
                lock (holder)
                    c = threadCount++;

                Typed_Registry_Async_Thread_Safe_Integrated_Test_Helper(collection.Skip(c * 10000).Take(10000).ToArray());
            };

            for (int i = 0; i < 10; i++)
            {
                s1[i] = Task.Factory.StartNew(publicTest);
            }

            return Task.Factory.ContinueWhenAll(
                s1,
                new Action<Task[]>(a =>
                {
                    Typed_Registry_Async_Thread_Safe_Integrated_Test_Callback(a);
                })
            );
        }
    }
}
