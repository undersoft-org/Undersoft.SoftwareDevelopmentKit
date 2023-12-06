using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Undersoft.SDK.Service.Application.Documentation
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class IgnoreApiAttribute : ActionFilterAttribute { }
}

