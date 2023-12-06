using System.Text.Json;

namespace Undersoft.SDK.Service.Data.Object
{
    public interface IDataObjectDetail<T> : IDataObjectDetail where T : class
    {
       T GetObject();
        void SetDocument(T structure);
    }


    public interface IDataObjectDetail: IDataObject
    {
        JsonDocument Document { get; set; }
        string TypeName { get; set; }

        T GetObject<T>();
        void SetDocument<T>(T structure);
    }
}