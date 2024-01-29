using System.Reflection;
using System.Threading.Tasks;

namespace Undersoft.SDK.Invoking
{
    using Undersoft.SDK.Instant;
    using Undersoft.SDK.Series;
    using Uniques;

    public interface IInvokable : IIdentifiable, IValueArray, IInstant
    {
        string Name { get; set; }

        string QualifiedName { get; set; }

        string MethodName { get; }

        AssemblyName AssemblyName { get; }       

        string TypeName { get; }

        Type Type { get; }

        MethodInfo Info { get; set; }

        Arguments Arguments { get; set; }

        Type ReturnType { get; }
    }
}
