using System.Text.Json;

namespace Undersoft.SDK.Serialization;

public interface ISerializableObject 
{
    JsonDocument Document { get; set; }

    string TypeName { get; set; }

    T GetObject<T>();

    void SetDocument<T>(T structure);
}
