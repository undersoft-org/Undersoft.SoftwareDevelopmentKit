using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Undersoft.SDK.Service.Data.Query
{
    [Serializable]
    [DataContract]
    public class QuerySet
    {
        [DataMember(Order = 1)]
        public List<FilterItem> FilterItems { get; set; } = new();

        [DataMember(Order = 2)]
        public List<SortItem> SortItems { get; set; } = new();

    }
}
