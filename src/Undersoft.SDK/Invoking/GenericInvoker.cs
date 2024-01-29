using System.Reflection;
using System.Reflection.Emit;
using Undersoft.SDK.Series;
using Undersoft.SDK.Uniques;

namespace Undersoft.SDK.Invoking
{
    public class Invoker<T> : Invoker
    {
        public Invoker() : base(typeof(T)) { }

        public Invoker(params object[] constructorParams) : base(typeof(T), constructorParams) { }

        public Invoker(IInvokable invoke) : base(invoke.TypeName, invoke.MethodName) { }

        public Invoker(Func<T, Delegate> method)
            : base(typeof(T), method(typeof(T).New<T>()).Method.Name) { }

        public Invoker(Func<T, Delegate> method, params object[] constructorParams)
            : base(
                typeof(T),
                method(typeof(T).New<T>(constructorParams)).Method.Name,
                constructorParams
            ) { }

        public Invoker(Func<T, Delegate> method, params Type[] parameterTypes)
            : base(typeof(T), method(typeof(T).New<T>()).Method.Name, parameterTypes) { }

        public Invoker(T targetObject, Func<T, Delegate> method)
            : base(targetObject, method(targetObject).Method.Name) { }

        public Invoker(Type[] parameterTypes, params object[] constructorParams)
            : base(typeof(T), parameterTypes, constructorParams) { }

        public Invoker(string methodName) : base(typeof(T), methodName) { }

        public Invoker(string methodName, params Type[] parameterTypes)
            : base(typeof(T), methodName, parameterTypes) { }

        public Invoker(string methodName, Type[] parameterTypes, params object[] constructorParams)
            : base(typeof(T), methodName, parameterTypes, constructorParams) { }

        public Invoker(string methodName, params object[] constructorParams)
            : base(typeof(T), methodName, constructorParams) { }
    }
}
