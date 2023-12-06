using System.Collections.ObjectModel;

namespace Undersoft.SDK.Service.Data.Branch
{

    public class BranchSet<TDto> : KeyedCollection<long, TDto>, IFindableSeries where TDto : IOrigin
    {
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
                TryGetValue(key.UniqueKey64(), out TDto result);
                return result;
            }
            set
            {
                Dictionary[key.UniqueKey64()] = (TDto)value;
            }
        }
    }
}