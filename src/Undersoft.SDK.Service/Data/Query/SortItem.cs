using System.Runtime.Serialization;

namespace Undersoft.SDK.Service.Data.Query
{
    [Serializable]
    [DataContract]
    public class SortItem
    {
        [DataMember(Order = 1)]
        public string Direction { get; set; }

        [DataMember(Order = 2)]
        public string Property { get; set; }
    }
}
