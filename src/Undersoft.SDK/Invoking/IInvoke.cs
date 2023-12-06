using System.Reflection;
using System.Threading.Tasks;

namespace Undersoft.SDK.Invoking
{
    using Uniques;

    public interface IInvoke : IUnique
    {
        string MethodName { get; set; }

        string TypeName { get; set; }
    }
}
