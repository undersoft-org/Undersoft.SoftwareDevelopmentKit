using System.Reflection;
using System.Reflection.Emit;
using Undersoft.SDK.Series;
using Undersoft.SDK.Uniques;

namespace Undersoft.SDK.Invoking
{
    public class Invoker : Origin, IInvoker
    {
        private Uscn serialcode;
        private event InvokerDelegate routine;

        public Invoker() { }

        public Invoker(object targetObject, MethodInfo methodInvokeInfo)
        {
            TargetObject = targetObject;
            MethodInfo m = methodInvokeInfo;

            if (m.GetParameters().Any())
            {
                NumberOfArguments = m.GetParameters().Length;
                Info = m;
                Parameters = m.GetParameters();
                Arguments = new Arguments(Parameters);
            }
            createUniqueKey();
        }

        public Invoker(Delegate targetMethod)
        {
            TargetObject = targetMethod.Target;
            Type t = TargetObject.GetType();
            MethodInfo m = targetMethod.Method;

            if (m.GetParameters().Any())
            {
                NumberOfArguments = m.GetParameters().Length;
                Info = m;
                Parameters = m.GetParameters();
                Arguments = new Arguments(Parameters);
            }
            createUniqueKey();
        }

        public Invoker(object targetObject, string methodName, params Type[] parameterTypes)
        {
            TargetObject = targetObject;
            Type t = TargetObject.GetType();

            MethodInfo m =
                parameterTypes != null
                    ? t.GetMethod(methodName, parameterTypes)
                    : t.GetMethod(methodName);

            if (m != null)
            {
                if (m.GetParameters().Any())
                {
                    Parameters = m.GetParameters();
                    NumberOfArguments = Parameters.Length;
                    Arguments = new Arguments(Parameters);
                }
                Info = m;
            }
            createUniqueKey();
        }

        public Invoker(Type targetType, Type[] parameterTypes)
            : this(
                targetType
                    .GetMethods()
                    .Where(
                        m =>
                            m.IsPublic
                            && m.GetParameters().All(p => parameterTypes.Contains(p.ParameterType))
                    )
                    .FirstOrDefault()
            ) { }

        public Invoker(
            Type targetType,
            Type[] parameterTypes,
            params object[] constructorParameters
        )
            : this(
                targetType
                    .GetMethods()
                    .FirstOrDefault(
                        m =>
                            m.IsPublic
                            && m.GetParameters().All(p => parameterTypes.Contains(p.ParameterType))
                    ),
                constructorParameters
            ) { }

        public Invoker(Type targetType, params object[] constructorParameters)
            : this(targetType.GetMethods().FirstOrDefault(m => m.IsPublic), constructorParameters)
        { }

        public Invoker(Type targetType)
            : this(targetType.GetMethods().FirstOrDefault(m => m.IsPublic)) { }

        public Invoker(object targetObject, string methodName)
            : this(targetObject, methodName, null) { }

        public Invoker(
            object targetObject,
            string methodName,
            params object[] constructorParameters
        ) : this(targetObject, methodName, null) { }

        public Invoker(Type targetType, string methodName)
            : this(Instances.New(targetType), methodName, null) { }

        public Invoker(Type targetType, string methodName, params Type[] parameterTypes)
            : this(Instances.New(targetType), methodName, parameterTypes) { }

        public Invoker(Type targetType, string methodName, params object[] constructorParams)
            : this(Instances.New(targetType, constructorParams), methodName) { }

        public Invoker(
            Type targetType,
            string methodName,
            Type[] parameterTypes,
            params object[] constructorParams
        ) : this(Instances.New(targetType, constructorParams), methodName, parameterTypes) { }

        public Invoker(string targetName, string methodName)
            : this(Instances.New(targetName), methodName, null) { }

        public Invoker(string targetName, string methodName, params Type[] parameterTypes)
            : this(Instances.New(targetName), methodName, parameterTypes) { }

        public Invoker(
            string targetName,
            string methodName,
            Type[] parameterTypes,
            params object[] constructorParams
        ) : this(Instances.New(targetName, constructorParams), methodName, parameterTypes) { }

        public Invoker(MethodInfo methodInvokeInfo)
            : this(methodInvokeInfo.DeclaringType.New(), methodInvokeInfo) { }

        public Invoker(MethodInfo methodInvokeInfo, params object[] constructorParams)
            : this(methodInvokeInfo.DeclaringType.New(constructorParams), methodInvokeInfo) { }

        public Uscn Code
        {
            get => serialcode;
            set => serialcode = value;
        }

        public string Name { get; set; }

        public string MethodName => Info.Name;

        public override string TypeName { get => Type.FullName; }

        public string QualifiedName { get; set; }

        public object this[int fieldId]
        {
            get => (fieldId < NumberOfArguments) ? Arguments[fieldId].Value : null;
            set
            {
                if (fieldId < NumberOfArguments)
                    Arguments[fieldId].Value = value;
            }
        }
        public object this[string argumentName]
        {
            get
            {
                if (Arguments.TryGet(argumentName.UniqueKey(), out Argument arg))
                    return arg.Value;
                return null;
            }
            set
            {
                if (Arguments.TryGet(argumentName.UniqueKey(), out Argument arg))
                    arg.Value = value;
            }
        }

        public InvokerDelegate MethodInvoker
        {
            get
            {
                if (routine == null)
                    routine += (InvokerDelegate)Method;
                return routine;
            }
        }

        public Object TargetObject { get; set; }

        public Delegate Method { get; set; }

        public MethodInfo Info { get; set; }

        public ParameterInfo[] Parameters { get; set; }

        public Arguments Arguments { get; set; }

        public Type ReturnType => Info.ReturnType;

        public int NumberOfArguments { get; set; }

        public StateOn StateOn { get; set; }

        public IUnique Empty => Uscn.Empty;

        public object[] ValueArray
        {
            get => Arguments.Select(a => a.Value).ToArray();
            set =>
                Arguments.ForEach(
                    (a, x) =>
                    {
                        if (a.Type == value[x].GetType())
                            a.Value = value[x];
                    }
                );
        }

        public Type Type => TargetObject.GetType();

        public AssemblyName AssemblyName => Type.Assembly.GetName();

        public virtual Task Fire(params object[] parameters)
        {
            try
            {
                return Task.Run(
                    () => (Method ?? InvokingIL.Create(Info)).DynamicInvoke(TargetObject, parameters)
                );
            }
            catch (Exception e)
            {
                throw new TargetInvocationException(e);
            }
        }

        public virtual Task Fire(bool withTarget, object target, params object[] parameters)
        {
            try
            {
                if (!withTarget)
                {
                    parameters = new[] { target }.Concat(parameters).ToArray();
                    target = TargetObject;
                }

                if (Method == null)
                {
                    Method = InvokingIL.Create(Info);
                }

                return Task.Run(() => Method.DynamicInvoke(target, parameters));
            }
            catch (Exception e)
            {
                throw new TargetInvocationException(e);
            }
        }

        public virtual object Invoke(params object[] parameters)
        {
            try
            {
                if (Method == null)
                {
                    Method = InvokingIL.Create(Info);
                }

                var obj = Method.DynamicInvoke(TargetObject, parameters);

                return obj;
            }
            catch (Exception e)
            {
                throw new TargetInvocationException(e);
            }
        }

        public virtual object Invoke(bool withTarget, object target, params object[] parameters)
        {
            try
            {
                if (!withTarget)
                {
                    parameters = new[] { target }.Concat(parameters).ToArray();
                    target = TargetObject;
                }
                if (Method == null)
                {
                    Method = InvokingIL.Create(Info);
                }

                var obj = Method.DynamicInvoke(target, parameters);

                return obj;
            }
            catch (Exception e)
            {
                throw new TargetInvocationException(e);
            }
        }

        public virtual T Invoke<T>(params object[] parameters)
        {
            try
            {
                if (Method == null)
                {
                    Method = InvokingIL.Create(Info);
                }

                var obj = Method.DynamicInvoke(TargetObject, parameters);

                return (T)obj;
            }
            catch (Exception e)
            {
                throw new TargetInvocationException(e);
            }
        }

        public virtual T Invoke<T>(bool withTarget, object target, params object[] parameters)
        {
            try
            {
                if (!withTarget)
                {
                    parameters = new[] { target }.Concat(parameters).ToArray();
                    target = TargetObject;
                }
                if (Method == null)
                {
                    Method = InvokingIL.Create(Info);
                }

                var obj = Method.DynamicInvoke(target, parameters);
                    
                return (T)obj;
            }
            catch (Exception e)
            {
                throw new TargetInvocationException(e);
            }
        }

        public virtual async Task<object> InvokeAsync(params object[] parameters)
        {
            try
            {
                var obj = Invoke(parameters);

                if (obj.GetType().IsAssignableTo(typeof(Task<object>)))
                {
                    return await (Task<object>)obj;
                }

                return await Task.FromResult<object>(obj);
            }
            catch (Exception e)
            {
                throw new TargetInvocationException(e);
            }
        }

        public virtual async Task<object> InvokeAsync(
            bool withTarget,
            object target,
            params object[] parameters
        )
        {
            try
            {
                if (!withTarget)
                {
                    parameters = new[] { target }.Concat(parameters).ToArray();
                    target = TargetObject;
                }

                var obj = Invoke(target, parameters);

                if (obj.GetType().IsAssignableTo(typeof(Task<object>)))
                {
                    return await (Task<object>)obj;
                }

                return await Task.FromResult<object>(obj);
            }
            catch (Exception e)
            {
                throw new TargetInvocationException(e);
            }
        }

        public virtual async Task<T> InvokeAsync<T>(params object[] parameters)
        {
            try
            {
                var obj = Invoke(parameters);

                if (obj.GetType().IsAssignableTo(typeof(Task<T>)))
                {
                    return await (Task<T>)obj;
                }

                return await Task.FromResult<T>((T)obj);
            }
            catch (Exception e)
            {
                throw new TargetInvocationException(e);
            }
        }

        public virtual async Task<T> InvokeAsync<T>(
            bool withTarget,
            object target,
            params object[] parameters
        )
        {
            try
            {
                if (!withTarget)
                {
                    parameters = new[] { target }.Concat(parameters).ToArray();
                    target = TargetObject;
                }

                var obj = Invoke(target, parameters);

                if (obj.GetType().IsAssignableTo(typeof(Task<T>)))
                {
                    return await (Task<T>)obj;
                }

                return await Task.FromResult<T>((T)obj);
            }
            catch (Exception e)
            {
                throw new TargetInvocationException(e);
            }
        }

        public virtual async Task<object> InvokeAsync(Arguments arguments)
        {
            Arguments
                .ForEach(
                    arg => arguments.ContainsKey(arg.Id) ? arg.Value = arguments[arg.Id] : arg.Value
                )
                .Commit();
            return await InvokeAsync(Arguments.ValueArray);
        }

        public virtual async Task<object> InvokeAsync(
            bool withTarget,
            object target,
            Arguments arguments
        )
        {
            Arguments
                .ForEach(
                    arg => arguments.ContainsKey(arg.Id) ? arg.Value = arguments[arg.Id] : arg.Value
                )
                .Commit();
            return await InvokeAsync(withTarget, target, Arguments.ValueArray);
        }

        public virtual async Task<T> InvokeAsync<T>(Arguments arguments)
        {
            Arguments
                .ForEach(
                    arg => arguments.ContainsKey(arg.Id) ? arg.Value = arguments[arg.Id] : arg.Value
                )
                .Commit();
            return await InvokeAsync<T>(Arguments.ValueArray);
        }

        public virtual async Task<T> InvokeAsync<T>(
            bool withTarget,
            object target,
            Arguments arguments
        )
        {
            Arguments
                .ForEach(
                    arg => arguments.ContainsKey(arg.Id) ? arg.Value = arguments[arg.Id] : arg.Value
                )
                .Commit();
            return await InvokeAsync<T>(withTarget, target, Arguments.ValueArray);
        }

        public object ConvertType(object source, Type destination)
        {
            object newobject = Convert.ChangeType(source, destination);
            return (newobject);
        }

        private void createUniqueKey()
        {
            Name = getFullName();
            QualifiedName = getQualifiedName();
            Id = QualifiedName.UniqueKey64();
            TypeId = Info.DeclaringType.UniqueKey();
            ;
            Time = DateTime.Now;
        }

        private static long getUniqueKey(MethodInfo info, ParameterInfo[] parameters)
        {
            var qualifiedName = getQualifiedName(info, parameters);
            var key = qualifiedName.UniqueKey64();
            return key;
        }

        private static long getUniqueSeed(MethodInfo info)
        {
            return info.UniqueKey();
        }

        private string getFullName()
        {
            return $"{Info.DeclaringType.FullName}." + $"{Info.Name}";
        }

        private static string getFullName(MethodInfo info)
        {
            return $"{info.DeclaringType.FullName}." + $"{info.Name}";
        }

        private string getQualifiedName()
        {
            return $"{Info.DeclaringType.FullName}."
                + $"{Info.Name}"
                + $"{new String(Parameters.SelectMany(p => "." + p.ParameterType.Name).ToArray())}";
        }

        private static string getQualifiedName(MethodInfo info, ParameterInfo[] parameters)
        {
            return $"{info.DeclaringType.FullName}."
                + $"{info.Name}"
                + $"{new String(parameters.SelectMany(p => "." + p.ParameterType.Name).ToArray())}";
        }

        public static string GetName(Type type, string methodName, params Type[] parameterTypes)
        {
            MethodInfo m =
                parameterTypes != null
                    ? type.GetMethod(methodName, parameterTypes)
                    : type.GetMethod(methodName);
            return getFullName(m);
        }

        public static string GetName(Type type, params Type[] parameterTypes)
        {
            if (parameterTypes != null && parameterTypes.Any())
            {
                return getFullName(
                    type.GetMethods()
                        .FirstOrDefault(
                            m =>
                                m.IsPublic
                                && m.GetParameters()
                                    .All(p => parameterTypes.Contains(p.ParameterType))
                        )
                );
            }
            else
            {
                return getFullName(type.GetMethods().FirstOrDefault(m => m.IsPublic));
            }
        }

        public static string GetName<T>(string methodName, params Type[] parameterTypes)
        {
            return GetName(typeof(T), methodName, parameterTypes);
        }

        public static string GetName<T>(params Type[] parameterTypes)
        {
            return GetName(typeof(T), parameterTypes);
        }

        public static string GetQualifiedName(
            Type type,
            string methodName,
            params Type[] parameterTypes
        )
        {
            var m = type.GetMethods()
                .FirstOrDefault(
                    m =>
                        m.GetParameters().All(p => parameterTypes.Contains(p.ParameterType))
                        && m.IsPublic
                        && m.Name == methodName
                );
            return getQualifiedName(m, m.GetParameters());
        }

        public static string GetQualifiedName(Type type, params Type[] parameterTypes)
        {
            var m = type.GetMethods()
                .FirstOrDefault(
                    m =>
                        m.IsPublic
                        && m.GetParameters().All(p => parameterTypes.Contains(p.ParameterType))
                );
            return getQualifiedName(m, m.GetParameters());
        }

        public static string GetQualifiedName<T>(params Type[] parameterTypes)
        {
            return GetQualifiedName(typeof(T), parameterTypes);
        }

        public static string GetQualifiedName<T>(string methodName, params Type[] parameterTypes)
        {
            return GetQualifiedName(typeof(T), methodName, parameterTypes);
        }        
    }

    public enum ChangeState
    {
        Unchanged,

        Adding,

        Added,

        Modifying,

        Modified,

        Deleting,

        Deleted,

        Upserting,

        Upserted,

        Patching,

        Patched
    };

    public enum StateOn
    {
        Adding,
        AddComplete,
        Deleting,
        DeleteComplete,
        Getting,
        Patching,
        PatchComplete,
        Upsert,
        UpsertComplete,
        GetComplete,
        Setting,
        SetComplete,
        Saving,
        SaveComplete,
        Filtering,
        FilterComplete,
        Finding,
        FindComplete,
        Mapping,
        MapComplete,
        Exist,
        ExistComplete,
        NonExist,
        NonExistComplete,
        Validating,
        ValidateComplete,
    }
}
