namespace System.Series.Tests
{
    using NetTopologySuite.Utilities;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using Undersoft.SDK.Series;

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

        public void SharedDeck_Integrated_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            Deck_Add_Test(testCollection);
            Deck_Count_Test(100000);
            Deck_First_Test(testCollection[0].Value);
            Deck_Last_Test(testCollection[99999].Value);
            Deck_Get_Test(testCollection);
            Deck_GetItem_Test(testCollection);
            Deck_Remove_Test(testCollection);
            Deck_Count_Test(70000);
            Deck_Enqueue_Test(testCollection);
            Deck_Count_Test(70005);
            Deck_Dequeue_Test(testCollection);
            Deck_Contains_Test(testCollection);
            Deck_ContainsKey_Test(testCollection);
            Deck_Put_Test(testCollection);
            Deck_Count_Test(100000);
            Deck_Clear_Test();
            Deck_Add_V_Test(testCollection);
            Deck_Count_Test(100000);
            Deck_Remove_V_Test(testCollection);
            Deck_Count_Test(70000);
            Deck_Put_V_Test(testCollection);
            Deck_IndexOf_Test(testCollection);
            Deck_GetByIndexer_Test(testCollection);
            Deck_Count_Test(100000);
        }

        public void SharedDeck_ThreadIntegrated_Test(
            IList<KeyValuePair<object, string>> testCollection
        )
        {
            SharedDeck_Add_Test(testCollection);
            SharedDeck_Get_Test(testCollection);
            SharedDeck_GetItem_Test(testCollection);
            SharedDeck_Remove_Test(testCollection);
            SharedDeck_Contains_Test(testCollection);
            SharedDeck_ContainsKey_Test(testCollection);
            SharedDeck_Put_Test(testCollection);
            SharedDeck_GetByIndexer_Test(testCollection);

            Debug.WriteLine($"Thread no {testCollection[0].Key}_{registry.Count} ends");

            Thread.Sleep(1000);
        }

        private void Deck_Add_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            foreach (var item in testCollection)
            {
                registry.Add(item.Key, item.Value);
            }
        }

        private void Deck_Add_V_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            foreach (var item in testCollection)
            {
                registry.Add(item.Value);
            }
        }

        private void Deck_Clear_Test()
        {
            registry.Clear();
        }

        private void Deck_Contains_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<bool> items = new List<bool>();
            foreach (var item in testCollection)
            {
                if (registry.Contains(registry.NewItem(item.Key, item.Value)))
                    items.Add(true);
            }
            Assert.Equals(70000, items.Count);
        }

        private void Deck_ContainsKey_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<bool> items = new List<bool>();
            foreach (var item in testCollection)
            {
                if (registry.ContainsKey(item.Key))
                    items.Add(true);
            }
            Assert.Equals(70000, items.Count);
        }

        private void Deck_CopyTo_Test() { }

        private void Deck_Count_Test(int count)
        {
            Assert.Equals(count, registry.Count);
        }

        private void Deck_Dequeue_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<string> items = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                string output = null;
                if (registry.TryDequeue(out output))
                    items.Add(output);
            }
            Assert.Equals(5, items.Count);
        }

        private void Deck_Enqueue_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<bool> items = new List<bool>();
            foreach (var item in testCollection.Skip(70000).Take(5))
            {
                if (registry.Enqueue(item.Key, item.Value))
                    items.Add(true);
            }
            Assert.Equals(5, items.Count);
        }

        private void Deck_First_Test(string firstValue)
        {
            Assert.Equals(registry.First.Next.Value, firstValue);
        }

        private void Deck_Get_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<string> items = new List<string>();
            foreach (var item in testCollection)
            {
                string r = registry.Get(item.Key);
                if (r != null)
                    items.Add(r);
            }
            Assert.Equals(100000, items.Count);
        }

        private void Deck_GetByIndexer_Test(IList<KeyValuePair<object, string>> testCollection)
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

        private void Deck_GetItem_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<ISeriesItem<string>> items = new List<ISeriesItem<string>>();
            foreach (var item in testCollection)
            {
                var r = registry.GetItem(item.Key);
                if (r != null)
                    items.Add(r);
            }
            Assert.Equals(100000, items.Count);
        }

        private void Deck_IndexOf_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<int> items = new List<int>();
            foreach (var item in testCollection.Skip(5000).Take(100))
            {
                int r = registry.IndexOf(item.Value);
                items.Add(r);
            }
        }

        private void Deck_Last_Test(string lastValue)
        {
            Assert.Equals(registry.Last.Value, lastValue);
        }

        private void Deck_Put_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            foreach (var item in testCollection)
            {
                registry.Put(item.Key, item.Value);
            }
        }

        private void Deck_Put_V_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            foreach (var item in testCollection)
            {
                registry.Put(item.Value);
            }
        }

        private void Deck_Remove_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<string> items = new List<string>();
            foreach (var item in testCollection.Skip(70000))
            {
                var r = registry.Remove(item.Key);
                if (r != null)
                    items.Add(r);
            }
            Assert.Equals(30000, items.Count);
        }

        private void Deck_Remove_V_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<string> items = new List<string>();
            foreach (var item in testCollection.Skip(70000))
            {
                string r = registry.Remove(item.Value);
                items.Add(r);
            }
            Assert.Equals(30000, items.Count);
        }

        private void SharedDeck_Add_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            foreach (var item in testCollection)
            {
                registry.Add(item.Key, item.Value);
            }
        }

        private void SharedDeck_Add_V_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            foreach (var item in testCollection)
            {
                registry.Add(item.Value);
            }
        }

        private void SharedDeck_Contains_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<bool> items = new List<bool>();
            foreach (var item in testCollection)
            {
                if (registry.Contains(registry.NewItem(item.Key, item.Value)))
                    items.Add(true);
            }
        }

        private void SharedDeck_ContainsKey_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<bool> items = new List<bool>();
            foreach (var item in testCollection)
            {
                if (registry.ContainsKey(item.Key))
                    items.Add(true);
            }
        }

        private void SharedDeck_Dequeue_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<string> items = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                string output = null;
                if (registry.TryDequeue(out output))
                    items.Add(output);
            }
            Assert.Equals(5, items.Count);
        }

        private void SharedDeck_Enqueue_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<bool> items = new List<bool>();
            foreach (var item in testCollection.Skip(5000).Take(5))
            {
                if (registry.Enqueue(item.Key, item.Value))
                    items.Add(true);
            }
            Assert.Equals(5, items.Count);
        }

        private void SharedDeck_Get_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<string> items = new List<string>();
            foreach (var item in testCollection)
            {
                string r = registry.Get(item.Key);
                if (r != null)
                    items.Add(r);
            }
        }

        private void SharedDeck_GetByIndexer_Test(
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

        private void SharedDeck_GetItem_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<ISeriesItem<string>> items = new List<ISeriesItem<string>>();
            foreach (var item in testCollection)
            {
                var r = registry.GetItem(item.Key);
                if (r != null)
                    items.Add(r);
            }
        }

        private void SharedDeck_Put_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            foreach (var item in testCollection)
            {
                registry.Put(item.Key, item.Value);
            }
        }

        private void SharedDeck_Put_V_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            foreach (var item in testCollection)
            {
                registry.Put(item.Value);
            }
        }

        private void SharedDeck_Remove_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            List<string> items = new List<string>();
            foreach (var item in testCollection.Skip(5000))
            {
                var r = registry.Remove(item.Key);
                if (r != null)
                    items.Add(r);
            }
        }

        private void SharedDeck_Remove_V_Test(IList<KeyValuePair<object, string>> testCollection)
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
