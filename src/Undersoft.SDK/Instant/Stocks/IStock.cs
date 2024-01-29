using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Undersoft.SDK.Instant.Stocks;

namespace Undersoft.SDK.Instant.Stocks
{
    public interface IStock 
    {
        public object this[int index]
        {
            get;
            set;
        }
        public object this[int index, int field, Type type]
        {
            get;
            set;
        }

        void Write();
        void Read();
        void Open();
        void Close();
    }
}
