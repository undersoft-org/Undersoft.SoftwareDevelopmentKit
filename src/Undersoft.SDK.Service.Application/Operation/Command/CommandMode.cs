using FluentValidation.Results;
using System;
using System.Text.Json.Serialization;

namespace Undersoft.SDK.Service.Application.Operation.Command
{
    [Flags]
    public enum CommandMode
    {
        Any = 31,
        Create = 1,
        Change = 2,
        Update = 4,
        Delete = 8,
        Upsert = 16,
        Invoke = 32
    }
}
