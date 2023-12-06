using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;

namespace Undersoft.SDK.Service.Data.Query
{
    [Serializable]
    public class QuerySet
    {
        public List<FilterItem> FilterItems { get; set; } = new();

        public List<SortItem> SortItems { get; set; } = new();

    }
}
