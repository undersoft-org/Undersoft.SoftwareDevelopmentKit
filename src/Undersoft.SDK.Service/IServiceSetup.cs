using AutoMapper;
using System.Reflection;

namespace Undersoft.SDK.Service;

using Data.Mapper;

public partial interface IServiceSetup
{
    IServiceSetup AddMapper(IDataMapper mapper);
    IServiceSetup AddMapper(params Profile[] profiles);
    IServiceSetup AddMapper<TProfile>() where TProfile : Profile;
    IServiceSetup AddCaching();
    IServiceSetup ConfigureServices(Assembly[] assemblies = null);
    IServiceSetup AddRepositorySources(Assembly[] assemblies = null);
    IServiceSetup AddRepositoryClients(Assembly[] assemblies = null);
    IServiceSetup AddImplementations(Assembly[] assemblies = null);
    IServiceSetup MergeServices();
    IServiceRegistry Services { get; }
}