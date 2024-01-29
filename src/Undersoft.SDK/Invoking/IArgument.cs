using Undersoft.SDK.Series;

namespace Undersoft.SDK.Invoking
{
    public interface IArgument : IIdentifiable
    {
        string Name { get; set; }
        object Value { get; set; }
        string TypeName { get; set; }
        int Position { get; set; }
        Type Type { get; }
    }
}