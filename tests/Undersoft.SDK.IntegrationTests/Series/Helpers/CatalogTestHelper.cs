namespace System.Series.Tests
{
    using Undersoft.SDK.Series;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Series;
    using System.Threading;

    using Xunit;

    public class CatalogTestHelper
    {
        public CatalogTestHelper()
        {
            stringKeyTestCollection = PrepareTestListings.prepareStringKeyTestCollection();
            intKeyTestCollection = PrepareTestListings.prepareIntKeyTestCollection();
            longKeyTestCollection = PrepareTestListings.prepareLongKeyTestCollection();
            identifierKeyTestCollection = PrepareTestListings.prepareIdentifierKeyTestCollection();
        }

        public IList<KeyValuePair<object, string>> identifierKeyTestCollection { get; set; }

        public IList<KeyValuePair<object, string>> intKeyTestCollection { get; set; }

        public IList<KeyValuePair<object, string>> longKeyTestCollection { get; set; }

        public ISeries<string> registry { get; set; }

        public IList<KeyValuePair<object, string>> stringKeyTestCollection { get; set; }

        public void Catalog_Sync_Integrated_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            Catalog_Sync_Add_Test(testCollection);
            Catalog_Sync_Count_Test(100000);
            Catalog_Sync_First_Test(testCollection[0].Value);
            Catalog_Sync_Last_Test(testCollection[99999].Value);
            Catalog_Sync_Get_Test(testCollection);
            Catalog_Sync_GetCard_Test(testCollection);
            Catalog_Sync_Remove_Test(testCollection);
            Catalog_Sync_Count_Test(70000);
            Catalog_Sync_Enqueue_Test(testCollection);
            Catalog_Sync_Count_Test(70005);
            Catalog_Sync_Dequeue_Test(testCollection);
            Catalog_Sync_Contains_Test(testCollection);
            Catalog_Sync_ContainsKey_Test(testCollection);
            Catalog_Sync_Put_Test(testCollection);
            Catalog_Sync_Count_Test(100000);
            Catalog_Sync_Clear_Test();
            Catalog_Sync_Add_V_Test(testCollection);
            Catalog_Sync_Count_Test(100000);
            Catalog_Sync_Remove_V_Test(testCollection);
            Catalog_Sync_Count_Test(70000);
            Catalog_Sync_Put_V_Test(testCollection);
            Catalog_Sync_IndexOf_Test(testCollection);
            Catalog_Sync_GetByIndexer_Test(testCollection);
            Catalog_Sync_Count_Test(100000);
        }

        public void Catalog_Async__ThreadIntegrated_Test(
            IList<KeyValuePair<object, string>> testCollection
        )
        {
            Catalog_Async__Add_Test(testCollection);
            Catalog_Async__Get_Test(testCollection);
            Catalog_Async__GetCard_Test(testCollection);
            Catalog_Async__Remove_Test(testCollection);
            Catalog_Async__Contains_Test(testCollection);
            Catalog_Async__ContainsKey_Test(testCollection);
            Catalog_Async__Put_Test(testCollection);
            Catalog_Async__GetByIndexer_Test(testCollection);

            Debug.WriteLine($"Thread no {testCollection[0].Key}_{registry.Count} ends");

            Thread.Sleep(1000);
        }

        private void Catalog_Sync_Add_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            foreach (var item in testCollection)
            {
                registry.Add(item.Key, item.Value);
            }
        }

        private void Catalog_Sync_Add_V_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            foreach (var item in testCollection)
            {
                registry.Add(item.Value);
            }
        }

        private void Catalog_Sync_Clear_Test()
        {
            registry.Clear();
            Assert.Empty(registry);
        }

        private void Catalog_Sync_Contains_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<bool> items = new List<bool>();
            foreach (var item in testCollection)
            {
                if (registry.Contains(registry.NewItem(item.Key, item.Value)))
                    items.Add(true);
            }
            Assert.Equal(70000, items.Count);
        }

        private void Catalog_Sync_ContainsKey_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<bool> items = new List<bool>();
            foreach (var item in testCollection)
            {
                if (registry.ContainsKey(item.Key))
                    items.Add(true);
            }
            Assert.Equal(70000, items.Count);
        }

        private void Catalog_Sync_CopyTo_Test() { }

        private void Catalog_Sync_Count_Test(int count)
        {
            Assert.Equal(count, registry.Count);
        }

        private void Catalog_Sync_Dequeue_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<string> items = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                string output = null;
                if (registry.TryDequeue(out output))
                    items.Add(output);
            }
            Assert.Equal(5, items.Count);
        }

        private void Catalog_Sync_Enqueue_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<bool> items = new List<bool>();
            foreach (var item in testCollection.Skip(70000).Take(5))
            {
                if (registry.Enqueue(item.Key, item.Value))
                    items.Add(true);
            }
            Assert.Equal(5, items.Count);
        }

        private void Catalog_Sync_First_Test(string firstValue)
        {
            Assert.Equal(registry.First.Next.Value, firstValue);
        }

        private void Catalog_Sync_Get_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<string> items = new List<string>();
            foreach (var item in testCollection)
            {
                string r = registry.Get(item.Key);
                if (r != null)
                    items.Add(r);
            }
            Assert.Equal(100000, items.Count);
        }

        private void Catalog_Sync_GetByIndexer_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<string> items = new List<string>();
            int i = 0;
            foreach (var item in testCollection.Take(1000))
            {
                string a = registry[i];
                string b = item.Value;
                i++;
            }
        }

        private void Catalog_Sync_GetCard_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<ISeriesItem<string>> items = new List<ISeriesItem<string>>();
            foreach (var item in testCollection)
            {
                var r = registry.GetItem(item.Key);
                if (r != null)
                    items.Add(r);
            }
            Assert.Equal(100000, items.Count);
        }

        private void Catalog_Sync_IndexOf_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<int> items = new List<int>();
            foreach (var item in testCollection.Skip(5000).Take(100))
            {
                int r = registry.IndexOf(item.Value);
                items.Add(r);
            }
        }

        private void Catalog_Sync_Last_Test(string lastValue)
        {
            Assert.Equal(registry.Last.Value, lastValue);
        }

        private void Catalog_Sync_Put_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            foreach (var item in testCollection)
            {
                registry.Put(item.Key, item.Value);
            }
        }

        private void Catalog_Sync_Put_V_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            foreach (var item in testCollection)
            {
                registry.Put(item.Value);
            }
        }

        private void Catalog_Sync_Remove_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<string> items = new List<string>();
            foreach (var item in testCollection.Skip(70000))
            {
                var r = registry.Remove(item.Key);
                if (r != null)
                    items.Add(r);
            }
            Assert.Equal(30000, items.Count);
        }

        private void Catalog_Sync_Remove_V_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<string> items = new List<string>();
            foreach (var item in testCollection.Skip(70000))
            {
                string r = registry.Remove(item.Value);
                items.Add(r);
            }
            Assert.Equal(30000, items.Count);
        }

        private void Catalog_Async__Add_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            foreach (var item in testCollection)
            {
                registry.Add(item.Key, item.Value);
            }
        }

        private void Catalog_Async__Add_V_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            foreach (var item in testCollection)
            {
                registry.Add(item.Value);
            }
        }

        private void Catalog_Async__Contains_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<bool> items = new List<bool>();
            foreach (var item in testCollection)
            {
                if (registry.Contains(registry.NewItem(item.Key, item.Value)))
                    items.Add(true);
            }
        }

        private void Catalog_Async__ContainsKey_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<bool> items = new List<bool>();
            foreach (var item in testCollection)
            {
                if (registry.ContainsKey(item.Key))
                    items.Add(true);
            }
        }

        private void Catalog_Async__Dequeue_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<string> items = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                string output = null;
                if (registry.TryDequeue(out output))
                    items.Add(output);
            }
            Assert.Equal(5, items.Count);
        }

        private void Catalog_Async__Enqueue_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<bool> items = new List<bool>();
            foreach (var item in testCollection.Skip(5000).Take(5))
            {
                if (registry.Enqueue(item.Key, item.Value))
                    items.Add(true);
            }
            Assert.Equal(5, items.Count);
        }

        private void Catalog_Async__Get_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<string> items = new List<string>();
            foreach (var item in testCollection)
            {
                string r = registry.Get(item.Key);
                if (r != null)
                    items.Add(r);
            }
        }

        private void Catalog_Async__GetByIndexer_Test(
            IList<KeyValuePair<object, string>> testCollection
        )
        {
            List<string> items = new List<string>();
            int i = 0;
            foreach (var item in testCollection)
            {
                string a = registry[i];
                string b = item.Value;
            }
        }

        private void Catalog_Async__GetCard_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<ISeriesItem<string>> items = new List<ISeriesItem<string>>();
            foreach (var item in testCollection)
            {
                var r = registry.GetItem(item.Key);
                if (r != null)
                    items.Add(r);
            }
        }

        private void Catalog_Async__Put_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            foreach (var item in testCollection)
            {
                registry.Put(item.Key, item.Value);
            }
        }

        private void Catalog_Async__Put_V_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            foreach (var item in testCollection)
            {
                registry.Put(item.Value);
            }
        }

        private void Catalog_Async__Remove_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<string> items = new List<string>();
            foreach (var item in testCollection.Skip(5000))
            {
                var r = registry.Remove(item.Key);
                if (r != null)
                    items.Add(r);
            }
        }

        private void Catalog_Async__Remove_V_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<string> items = new List<string>();
            foreach (var item in testCollection.Skip(5000))
            {
                string r = registry.Remove(item.Value);
                items.Add(r);
            }
        }
    }
}
