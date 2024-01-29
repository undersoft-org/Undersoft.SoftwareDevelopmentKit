namespace Undersoft.SDK.Service.Hosting
{
    public interface IServiceHostSetup
    {
        IServiceHostSetup UseDataServices();

        IServiceHostSetup UseInternalProvider();

        IServiceHostSetup UseDataMigrations();

        IServiceHostSetup RebuildProviders();

        IServiceManager Manager { get; }
    }
}