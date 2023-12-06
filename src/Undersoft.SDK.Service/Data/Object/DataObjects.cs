using System.Collections.ObjectModel;
using System.Linq;

using Undersoft.SDK.Series;
using Undersoft.SDK.Service.Data.Object;
using Undersoft.SDK.Uniques;

namespace Undersoft.SDK.Service.Data.Object
{

    public class DataObjects<TDto> : KeyedCollection<long, TDto>, IFindableSeries where TDto : class, IDataObject
    {
        public DataObjects() { }

        public DataObjects(IEnumerable<TDto> list) { list.ForEach(item => base.Add(item)); }

        protected override long GetKeyForItem(TDto item)
        {
            return item.Id == 0 ? item.AutoId() : item.Id;
        }

        public TDto Single
        { 
            get => this.FirstOrDefault();
        }

        public object this[object key]
        {
            get
            {
                TryGetValue((long)key.UniqueKey64(), out TDto result);
                return result;
            }
            set
            {
                Dictionary[(long)key.UniqueKey64()] = (TDto)value;
            }
        }
    }
}