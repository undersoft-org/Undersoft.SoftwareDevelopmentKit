using Undersoft.SDK.Uniques;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Undersoft.SDK
{
    public interface IOrigin<V> : IOrigin
    {
    }

    public interface IOrigin : IUnique, INotifyPropertyChanged
    {
        string CodeNo { get; set; }
        DateTime Created { get; set; }
        string Creator { get; set; }
        DateTime Modified { get; set; }
        string Modifier { get; set; }
        int OriginKey { get; set; }
        string OriginName { get; set; }
        DateTime Time { get; set; }
        long AutoId();
        byte GetPriority();
        TEntity Sign<TEntity>(TEntity entity) where TEntity : class, IOrigin;
        TEntity Stamp<TEntity>(TEntity entity) where TEntity : class, IOrigin;
        void GetFlag(StateFlags state);
        void SetFlag(StateFlags state, bool flag);
        long SetId(long id);
        long SetId(object id);
    }
}
