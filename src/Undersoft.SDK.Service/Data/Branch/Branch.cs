using System.Runtime.InteropServices;

namespace Undersoft.SDK.Service.Data.Branch;

using System;
using System.Runtime.Serialization;
using Undersoft.SDK.Service.Data.Contract;
using Undersoft.SDK.Service.Data.Object;

[Serializable]
[DataContract]
[StructLayout(LayoutKind.Sequential)]
public class Branch : DataObject, IContract
{
    public Branch() : base() { }

}