using ProtoBuf.Grpc.Configuration;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;

namespace Undersoft.SDK.Service.Application.DataServer
{
    internal class GrpcDataServerBinder : ServiceBinder
    {
        private readonly IServiceRegistry registry;

        public GrpcDataServerBinder(IServiceRegistry registry)
        {
            this.registry = registry;
        }

        public override IList<object> GetMetadata(MethodInfo method, Type contractType, Type serviceType)
        {
            var resolvedServiceType = serviceType;
            if (serviceType.IsInterface)
                resolvedServiceType = registry[serviceType]?.ImplementationType ?? serviceType;

            return base.GetMetadata(method, contractType, resolvedServiceType);
        }

        protected override string GetDefaultName(Type contractType)
        {
            var val = base.GetDefaultName(contractType);
            if (val.EndsWith("`1") && contractType.IsGenericType)
            {
                var args = contractType.GetGenericArguments();
                if (args.Length == 1)
                {
                    val = val.Substring(0, val.Length - 1) + args[0].Name;
                }
            }
            return val;
        }
    }
}