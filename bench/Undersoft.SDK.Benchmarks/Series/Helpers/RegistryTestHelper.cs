namespace System.Series.Tests
{
    using NetTopologySuite.Utilities;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Undersoft.SDK.Series;

    public class BenchmarkHelper
    {
        public BenchmarkHelper()
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

        public IDictionary<object, string> dictionary { get; set; }

        public IList<KeyValuePair<object, string>> stringKeyTestCollection { get; set; }

        public void LogIntegrated_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            Add_Test(testCollection);
            Count_Test(100000);
            First_Test(testCollection[0].Value);
            Last_Test(testCollection[99999].Value);
            Get_Test(testCollection);
            GetItem_Test(testCollection);
            Remove_Test(testCollection);
            Count_Test(70000);
            Enqueue_Test(testCollection);
            Count_Test(70005);
            Dequeue_Test(testCollection);
            Contains_Test(testCollection);
            ContainsKey_Test(testCollection);
            Put_Test(testCollection);
            Count_Test(100000);
            Clear_Test();
            Add_V_Test(testCollection);
            Count_Test(100000);
            Remove_V_Test(testCollection);
            Count_Test(70000);
            Put_V_Test(testCollection);
            IndexOf_Test(testCollection);
            GetByIndexer_Test(testCollection);
            Count_Test(100000);
        }

        public void LogThreadIntegrated_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            LogAdd_Test(testCollection);
            LogGet_Test(testCollection);
            LogGetItem_Test(testCollection);
            LogRemove_Test(testCollection);
            LogEnqueue_Test(testCollection);
            LogDequeue_Test(testCollection);
            LogContains_Test(testCollection);
            LogContainsKey_Test(testCollection);
            LogPut_Test(testCollection);
            LogGetByIndexer_Test(testCollection);

            Debug.WriteLine($"Thread no {testCollection[0].Key.ToString()}_{registry.Count} ends");
        }

        public void Add_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            registry = new Registry<string>();
            foreach (var item in testCollection)
            {
                registry.Add(item.Key, item.Value);
            }
        }

        public void Add_V_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            registry = new Registry<string>();
            foreach (var item in testCollection)
            {
                registry.Add(item.Value);
            }
        }

        public void Clear_Test()
        {
            registry = new Registry<string>();
            foreach (var item in stringKeyTestCollection)
            {
                registry.Add(item.Key, item.Value);
            }
            registry.Clear();
        }

        public void Contains_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            registry = new Registry<string>();
            foreach (var item in testCollection)
            {
                registry.Add(item.Key, item.Value);
            }
            foreach (var item in testCollection)
            {
                registry.Contains(registry.NewItem(item.Key, item.Value));
            }
        }

        public void ContainsKey_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            registry = new Registry<string>();
            foreach (var item in testCollection)
            {
                registry.Add(item.Key, item.Value);
            }
            foreach (var item in testCollection)
            {
                registry.ContainsKey(item.Key);
            }
        }

        public void CopyTo_Test() { }

        public void Count_Test(int count)
        {
            Assert.Equals(count, registry.Count);
        }

        public void Dequeue_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            for (int i = 0; i < 5; i++)
            {
                string output = null;
                registry.TryDequeue(out output);
            }
        }

        public void Enqueue_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            foreach (var item in testCollection.Skip(70000).Take(5))
            {
                registry.Enqueue(item.Key, item.Value);
            }
        }

        public void First_Test(string firstValue)
        {
            registry = new Registry<string>();
            foreach (var item in identifierKeyTestCollection)
            {
                registry.Add(item.Key, item.Value);
            }
            Assert.Equals(registry.Next(registry.First).Value, firstValue);
        }

        public void Get_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            registry = new Registry<string>();
            foreach (var item in testCollection)
            {
                registry.Add(item.Key, item.Value);
            }
            foreach (var item in testCollection)
            {
                string r = registry.Get(item.Key);
            }
        }

        public void GetByIndex_From_Indexer_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            registry = new Registry<string>();
            foreach (var item in testCollection)
            {
                registry.Add(item.Key, item.Value);
            }
            int i = 0;
            foreach (var item in testCollection)
            {
                string a = registry[i++];
            }
        }

        public void GetByKey_From_Indexer_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            registry = new Registry<string>();
            foreach (var item in testCollection)
            {
                registry.Add(item.Key, item.Value);
            }
            int i = 0;
            foreach (var item in testCollection)
            {
                string a = registry[item.Key];
            }
        }

        public void GetByIndexer_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            registry = new Registry<string>();
            foreach (var item in testCollection)
            {
                registry.Add(item.Key, item.Value);
            }
            int i = 0;
            foreach (var item in testCollection)
            {
                string a = registry[i++];
            }
        }

        public void GetItem_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            registry = new Registry<string>();
            foreach (var item in testCollection)
            {
                registry.Add(item.Key, item.Value);
            }
            foreach (var item in testCollection)
            {
                registry.GetItem(item.Key);
            }
        }

        public void IndexOf_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            registry = new Registry<string>();
            foreach (var item in testCollection)
            {
                registry.Add(item.Key, item.Value);
            }
            foreach (var item in testCollection.Skip(5000).Take(100))
            {
                registry.IndexOf(item.Value);
            }
        }

        public void Iteration_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            registry = new Registry<string>();
            foreach (var item in testCollection)
            {
                registry.Add(item.Key, item.Value);
            }
            foreach (var item in registry)
            {
                object r = item;
            }
        }

        public void Last_Test(string lastValue)
        {
            registry = new Registry<string>();
            foreach (var item in identifierKeyTestCollection)
            {
                registry.Add(item.Key, item.Value);
            }
            Assert.Equals(registry.Last.Value, lastValue);
        }

        public void LogAdd_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            int counter = 0;
            foreach (var item in testCollection)
            {
                registry.Add(item.Key, item.Value);
                counter++;
            }
            Debug.WriteLine($"Add Thread no {testCollection[0].Key.ToString()}_{counter} ends");
        }

        public void LogAdd_V_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            int counter = 0;
            foreach (var item in testCollection)
            {
                registry.Add(item.Value);
                counter++;
            }
            Debug.WriteLine($"Add Thread no {testCollection[0].Key.ToString()}_{counter} ends");
        }

        public void LogContains_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            int counter = 0;
            foreach (var item in testCollection)
            {
                if (registry.Contains(registry.NewItem(item.Key, item.Value)))
                    counter++;
            }
            Debug.WriteLine(
                $"Get Card Thread no {testCollection[0].Key.ToString()}_{counter} ends"
            );
        }

        public void LogContainsKey_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            int counter = 0;
            foreach (var item in testCollection)
            {
                if (registry.ContainsKey(item.Key))
                    counter++;
            }
            Debug.WriteLine(
                $"Get Card Thread no {testCollection[0].Key.ToString()}_{counter} ends"
            );
        }

        public void LogDequeue_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            int counter = 0;
            for (int i = 0; i < 5; i++)
            {
                string output = null;
                if (registry.TryDequeue(out output))
                    counter++;
            }
            Debug.WriteLine(
                $"Get Card Thread no {testCollection[0].Key.ToString()}_{counter} ends"
            );
        }

        public void LogEnqueue_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            int counter = 0;
            foreach (var item in testCollection.Skip(5000).Take(5))
            {
                if (registry.Enqueue(item.Key, item.Value))
                    counter++;
            }
            Debug.WriteLine(
                $"Get Card Thread no {testCollection[0].Key.ToString()}_{counter} ends"
            );
        }

        public void LogGet_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            int counter = 0;
            foreach (var item in testCollection)
            {
                string r = registry.Get(item.Key);
                if (r != null)
                    counter++;
            }
            Debug.WriteLine($"Get Thread no {testCollection[0].Key.ToString()}_{counter} ends");
        }

        public void LogGetByIndexer_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            int counter = 0;
            int i = 0;
            foreach (var item in testCollection)
            {
                var r = registry[i];
                if (r != null)
                    counter++;
            }
            Debug.WriteLine(
                $"Get By Indexer Thread no {testCollection[0].Key.ToString()}_{counter} ends"
            );
        }

        public void LogGetItem_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            int counter = 0;
            foreach (var item in testCollection)
            {
                var r = registry.GetItem(item.Key);
                if (r != null)
                    counter++;
            }
            Debug.WriteLine(
                $"Get Card Thread no {testCollection[0].Key.ToString()}_{counter} ends"
            );
        }

        public void LogPut_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            int counter = 0;
            foreach (var item in testCollection)
            {
                registry.Put(item.Key, item.Value);
                counter++;
            }
            Debug.WriteLine($"Put Thread no {testCollection[0].Key.ToString()}_{counter} ends");
        }

        public void LogPut_V_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            int counter = 0;
            foreach (var item in testCollection)
            {
                var r = registry.Put(item.Value);
                if (r != null)
                    counter++;
            }
            Debug.WriteLine($"Removed Thread no {testCollection[0].Key.ToString()}_{counter} ends");
        }

        public void LogRemove_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            int counter = 0;
            foreach (var item in testCollection.Skip(5000))
            {
                var r = registry.Remove(item.Key);
                if (r != null)
                    counter++;
            }
            Debug.WriteLine($"Removed Thread no {testCollection[0].Key.ToString()}_{counter} ends");
        }

        public void LogRemove_V_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            int counter = 0;
            foreach (var item in testCollection.Skip(5000))
            {
                string r = registry.Remove(item.Value);
                if (r != null)
                    counter++;
            }
            Debug.WriteLine(
                $"Removed V Thread no {testCollection[0].Key.ToString()}_{counter} ends"
            );
        }

        public void Put_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            registry = new Registry<string>();
            foreach (var item in testCollection)
            {
                registry.Add(item.Key, item.Value);
            }
            foreach (var item in testCollection)
            {
                registry.Put(item.Key, item.Value);
            }
        }

        public void Put_V_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            registry = new Registry<string>();
            foreach (var item in testCollection)
            {
                registry.Add(item.Key, item.Value);
            }
            foreach (var item in testCollection)
            {
                registry.Put(item.Value);
            }
        }

        public void Remove_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            registry = new Registry<string>();
            foreach (var item in testCollection)
            {
                registry.Add(item.Key, item.Value);
            }
            foreach (var item in testCollection.Skip(100000))
            {
                registry.Remove(item.Key);
            }
        }

        public void Remove_V_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            registry = new Registry<string>();
            foreach (var item in testCollection)
            {
                registry.Add(item.Key, item.Value);
            }
            foreach (var item in testCollection.Skip(70000))
            {
                registry.Remove(item.Value);
            }
        }
    }
}
