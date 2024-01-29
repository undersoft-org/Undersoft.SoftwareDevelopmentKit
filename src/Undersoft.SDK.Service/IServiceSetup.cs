using AutoMapper;
using System.Reflection;

namespace Undersoft.SDK.Service;

using Data.Mapper;

public partial interface IServiceSetup
{
    IServiceSetup AddMapper(IDataMapper mapper);
    IServiceSetup AddMapper(params MapperProfile[] profiles);
    IServiceSetup AddMapper<TProfile>() where TProfile : MapperProfile;
    IServiceSetup AddCaching();
    IServiceSetup ConfigureServices(Type[] clientTypes = null);
    IServiceSetup AddRepositoryClients();
    IServiceSetup AddImplementations();
    IServiceSetup AddPropertyInjection();
    IServiceSetup MergeServices();
    IServiceRegistry Services { get; }
    IServiceManager Manager { get; }
}