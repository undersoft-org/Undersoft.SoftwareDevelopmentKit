using System.Collections.Generic;

namespace Undersoft.SDK.Service.Configuration;
using Microsoft.Extensions.Configuration;

using Undersoft.SDK.Service.Configuration.Options;
using Undersoft.SDK.Service.Data.Repository.Source;

public interface IServiceConfiguration : IConfiguration
{
    string Version { get; }
    IServiceConfiguration Configure<TOptions>(string sectionName) where TOptions : class;
    IConfigurationSection Client(string name);
    IConfigurationSection DataCacheLifeTime();
    int ClientPoolSize(IConfigurationSection endpoint);
    string ClientConnectionString(IConfigurationSection client);
    string ClientConnectionString(string name);
    ClientProvider ClientProvider(IConfigurationSection client);
    ClientProvider ClientProvider(string name);
    IEnumerable<IConfigurationSection> Clients();
    string Description { get; }
    string DataServiceRoutes(string name);
    IConfigurationSection Source(string name);
    int SourcePoolSize(IConfigurationSection endpoint);
    string SourceConnectionString(IConfigurationSection endpoint);
    string SourceConnectionString(string name);
    SourceProvider SourceProvider(IConfigurationSection endpoint);
    SourceProvider SourceProvider(string name);
    IEnumerable<IConfigurationSection> Sources();
    string Title { get; }
    IConfigurationSection Repository();
    IConfigurationSection IdentityServer();
    string IdentityServerBaseUrl();
    string IdentityServerApiName();
    string[] IdentityServerScopes();
    string[] IdentityServerRoles();
    IdentityOptions GetIdentityConfiguration();
    IdentityOptions Identity { get; }
}