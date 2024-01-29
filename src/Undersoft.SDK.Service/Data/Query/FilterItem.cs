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
    public class FilterItem
    {
        [DataMember(Order = 1)]
        public string Property { get; set; }

        [DataMember(Order = 2)]
        public string Operand { get; set; }

        [DataMember(Order = 3)]
        public string Data { get; set; }

        [DataMember(Order = 4)]                        
        public object Value { get; set; }

        [DataMember(Order = 5)]
        public string Type { get; set; }

        [DataMember(Order = 6)]
        public string Logic { get; set; } = "And";
    }
}
