﻿using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Undersoft.SDK.Service.Application.Server;

using Undersoft.SDK.Service.Data.Store;
using Undersoft.SDK.Service.Server;

public partial interface IApplicationServerSetup : IServerSetup
{
    IServerSetup ConfigureApplicationServer(
        bool includeSwagger,
        Type[]? sourceTypes = null,
        Type[]? clientTypes = null
    );
}
