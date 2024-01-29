using FluentValidation.Results;
using ServiceStack;
using System.Text.Json;
using System.Text.Json.Serialization;
using Undersoft.SDK.Service.Server.Operation.Command;

namespace Undersoft.SDK.Service.Server.Operation.Invocation;

public abstract class InvocationBase : IInvocation
{
    public virtual long Id { get; set; }

    public virtual string Method { get; set; }
     
    public Arguments Arguments { get; set; }

    public virtual object Return { get; set; }

    public Delegate Processings { get; set; }

    [JsonIgnore]
    public virtual object Response { get; set; }

    [JsonIgnore]
    public ValidationResult Result { get; set; }

    public string ErrorMessages => Result.ToString();

    public CommandMode CommandMode { get; set; }

    public Type ServiceType { get; set; }

    public virtual object Input => Arguments;
    
    public virtual object Output => IsValid ? Response : ErrorMessages;

    public bool IsValid => Result.IsValid;

    protected InvocationBase()
    {
        Result = new ValidationResult();
    }

    protected InvocationBase(CommandMode commandMode, Type serviceType, string method) : this()
    {
        CommandMode = commandMode;
        Method = method;
        ServiceType = serviceType;
    }

    protected InvocationBase(object argument, CommandMode commandMode, Type serviceType, string method)
        : this(commandMode, serviceType, method)
    {
        var methodInfo = ServiceType.GetMethod(method, new Type[] { argument.GetType() });
        if (methodInfo != null)
        {
            Arguments = new Arguments(methodInfo.GetParameters());
            Arguments[0].Value = argument;
        }
    }

    protected InvocationBase(CommandMode commandMode, Type serviceType, string method, Arguments arguments)
        : this(commandMode, serviceType, method)
    {
        var methodInfo = ServiceType.GetMethod(method, arguments.Select(a => a.GetType()).ToArray());
        if (methodInfo != null)
        {
            Arguments = new Arguments(methodInfo.GetParameters());
            Arguments.ForEach(a => 
            { 
                if (arguments.ContainsKey(a.Name))
                    a.Value = a.Value.GetType().IsAssignableTo(typeof(JsonElement)) 
                    ? ((JsonElement)arguments[a.Name].Value).Deserialize(a.Type) 
                    : arguments[a.Name].Value; 
            });
        }
    }

    protected InvocationBase(CommandMode commandMode, Type serviceType, string method, params object[] arguments)
       : this(commandMode, serviceType, method)
    {
        var methodInfo = ServiceType.GetMethod(method, arguments.Select(a => a.GetType()).ToArray());
        if (methodInfo != null)
        {
            Arguments = new Arguments(methodInfo.GetParameters());
            Arguments.ForEach((a, x) =>{ if(a.Type == arguments[x].GetType()) a.Value = arguments[x]; } );
        }
    }

    protected InvocationBase(CommandMode commandMode, Type serviceType, string method, Dictionary<string, object> arguments)
       : this(commandMode, serviceType, method)
    {
        var methodInfo = ServiceType.GetMethod(method, arguments.Select(a => a.GetType()).ToArray());
        if (methodInfo != null)
        {
            Arguments = new Arguments(methodInfo.GetParameters());
            Arguments.ForEach(a => { if (arguments.ContainsKey(a.Name)) a.Value = ((JsonElement)arguments[a.Name]).Deserialize(a.Type); });
        }
    }

    public void SetArguments(Arguments arguments) => Arguments = arguments;
}
