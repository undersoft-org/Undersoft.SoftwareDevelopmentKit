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

    public class ListingTest : RegistryTestHelper
    {
        public static object holder = new object();
        public static int threadCount = 0;
        public Task[] s1 = new Task[10];

        public ListingTest() : base()
        {
            registry = new Listing<string>();
            DefaultTraceListener Logfile = new DefaultTraceListener();
            Logfile.Name = "Logfile";
            Trace.Listeners.Add(Logfile);
            Logfile.LogFileName = $"Lsting__{DateTime.Now.ToFileTime().ToString()}_Test.log";
        }

        [Fact]
        public void Listing_Identifiable_Sync_Integrated_Test()
        {
            Registry_Sync_Integrated_Test_Helper(identifierKeyTestCollection.Take(100000).ToArray());
        }

        [Fact]
        public void Listing_Integer_Keys__Sync_Integrated_Test()
        {
            Registry_Sync_Integrated_Test_Helper(intKeyTestCollection.Take(100000).ToArray());
        }

        [Fact]
        public void Listing_Long_Keys__Sync_Integrated_Test()
        {
            Registry_Sync_Integrated_Test_Helper(longKeyTestCollection.Take(100000).ToArray());
        }

        [Fact]
        public void Listing_String_Keys__Sync_Integrated_Test()
        {
            Registry_Sync_Integrated_Test_Helper(stringKeyTestCollection.Take(100000).ToArray());
        }
    }
}
