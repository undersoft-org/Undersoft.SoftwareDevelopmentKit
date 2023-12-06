using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Undersoft.SDK.Uniques;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;

namespace Undersoft.SDK.Service.Data.Entity;

using Instant.Updating;
using Instant;
using Identifier;
using Instant.Proxies;
using AutoMapper;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Undersoft.SDK.Service.Data.Event;
using Undersoft.SDK.Serialization;
using Undersoft.SDK.Service.Data.Object;


[DataContract]
[StructLayout(LayoutKind.Sequential)]
public class Entity : DataObject, IEntity
{
    public Entity() : base() { }

}
