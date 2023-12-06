namespace Undersoft.SDK.Service.Host
{
    public interface IServiceHostSetup
    {
        IServiceHostSetup UseDataServices();

        IServiceHostSetup UseAdvancedProvider();

        IServiceHostSetup UseDataMigrations();
    }
}