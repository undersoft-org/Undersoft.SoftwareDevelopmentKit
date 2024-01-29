using System;
using System.Series.Tests;

namespace Undersoft.SDK.IntegrationTests.Series
{
    using Undersoft.SDK.Series;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using Xunit;

    public class ChainTest : CatalogTestHelper
    {
        public ChainTest()
        {
            registry = new Chain<string>();
            DefaultTraceListener Logfile = new DefaultTraceListener();
            Logfile.Name = "Logfile";
            Trace.Listeners.Add(Logfile);
            Logfile.LogFileName = $"Chain_{DateTime.Now.ToFileTime().ToString()}_Test.log";
        }

        [Fact]
        public void Chain_Identifiable_Sync_Integrated_Test()
        {
            Catalog_Sync_Integrated_Test_Helper(identifierKeyTestCollection.Take(100000).ToArray());
        }

        [Fact]
        public void Chain_Integer_Keys_Sync_Integrated_Test()
        {
            Catalog_Sync_Integrated_Test_Helper(intKeyTestCollection.Take(100000).ToArray());
        }

        [Fact]
        public void Chain_Long_Keys_Sync_Integrated_Test()
        {
            Catalog_Sync_Integrated_Test_Helper(longKeyTestCollection.Take(100000).ToArray());
        }

        [Fact]
        public void Chain_String_Keys_Sync_Integrated_Test()
        {
            Catalog_Sync_Integrated_Test_Helper(stringKeyTestCollection.Take(100000).ToArray());
        }
    }
}
