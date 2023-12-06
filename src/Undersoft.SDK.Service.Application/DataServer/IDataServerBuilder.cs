using Microsoft.EntityFrameworkCore;
using Undersoft.SDK.Service.Data.Store;
using System;

namespace Undersoft.SDK.Service.Application.DataServer;

public interface IDataServerBuilder<TStore> : IDataServerBuilder where TStore : IDataServiceStore { }

public interface IDataServerBuilder : IDisposable, IAsyncDisposable
{
    string RoutePrefix { get; set; }

    int PageLimit { get; set; }

    void Build();

    IDataServerBuilder AddDataServices<TContext>() where TContext : DbContext;

    IDataServerBuilder AddIdentityActionSet();
}
