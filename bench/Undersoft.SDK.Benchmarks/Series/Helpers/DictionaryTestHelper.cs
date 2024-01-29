namespace System.Series.Tests
{
    using System.Collections.Generic;
    using System.Linq;

    public class BenchmarkDictionaryHelper
    {
        public BenchmarkDictionaryHelper()
        {
            stringKeyTestCollection = PrepareTestListings.prepareStringKeyTestCollection();
            intKeyTestCollection = PrepareTestListings.prepareIntKeyTestCollection();
            longKeyTestCollection = PrepareTestListings.prepareLongKeyTestCollection();
            identifierKeyTestCollection = PrepareTestListings.prepareIdentifierKeyTestCollection();
        }

        public IList<KeyValuePair<object, string>> identifierKeyTestCollection { get; set; }

        public IList<KeyValuePair<object, string>> intKeyTestCollection { get; set; }

        public IList<KeyValuePair<object, string>> longKeyTestCollection { get; set; }

        public IDictionary<string, string> registry { get; set; }

        public IList<KeyValuePair<object, string>> stringKeyTestCollection { get; set; }

        public void Add_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            registry = new Dictionary<string, string>();
            foreach (var item in testCollection)
            {
                registry.Add((string)item.Key.ToString(), item.Value);
            }
        }

        public void Contains_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            registry = new Dictionary<string, string>();
            foreach (var item in testCollection)
            {
                registry.Add((string)item.Key.ToString(), item.Value);
            }
            foreach (var item in testCollection)
            {
                registry.Contains(
                    new KeyValuePair<string, string>((string)item.Key.ToString(), item.Value)
                );
            }
        }

        public void ContainsKey_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            registry = new Dictionary<string, string>();
            foreach (var item in testCollection)
            {
                registry.Add((string)item.Key.ToString(), item.Value);
            }
            foreach (var item in testCollection)
            {
                registry.ContainsKey((string)item.Key.ToString());
            }
        }

        public void GetByKey_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            registry = new Dictionary<string, string>();
            foreach (var item in testCollection)
            {
                registry.Add((string)item.Key.ToString(), item.Value);
            }
            foreach (var item in testCollection)
            {
                string r = registry[(string)item.Key];
            }
        }

        public void GetByIndex_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            registry = new Dictionary<string, string>();
            foreach (var item in testCollection)
            {
                registry.Add((string)item.Key.ToString(), item.Value);
            }
            int i = 0;
            foreach (var item in testCollection)
            {
                string r = registry.Values.ElementAt(i++);
            }
        }

        public void GetLast_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            registry = new Dictionary<string, string>();
            foreach (var item in testCollection)
            {
                registry.Add((string)item.Key.ToString(), item.Value);
            }
            string r = registry.Last().Value;
        }

        public void Remove_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            registry = new Dictionary<string, string>();
            foreach (var item in testCollection)
            {
                registry.Add((string)item.Key.ToString(), item.Value);
            }
            foreach (var item in testCollection.Skip(100000))
            {
                registry.Remove((string)item.Key.ToString());
            }
        }

        public void Iteration_Test(IList<KeyValuePair<object, string>> testCollection)
        {
            registry = new Dictionary<string, string>();
            foreach (var item in testCollection)
            {
                registry.Add((string)item.Key.ToString(), item.Value);
            }
            foreach (var item in registry)
            {
                object r = item.Value;
            }
        }
    }
}
