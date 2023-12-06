using Microsoft.Extensions.DependencyInjection;

namespace Undersoft.SDK.Service.Application
{
    public static class ApplicationSetupExtensions
    {
        public static IApplicationSetup AddApplicationSetup(this IServiceCollection services, IMvcBuilder mvcBuilder = null)
        {
            return new ApplicationSetup(services);
        }
    }
}
