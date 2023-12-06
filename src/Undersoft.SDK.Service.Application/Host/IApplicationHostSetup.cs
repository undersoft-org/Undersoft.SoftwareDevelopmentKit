namespace Undersoft.SDK.Service.Application
{
    public interface IApplicationHostSetup
    {

        IApplicationHostSetup UseHeaderForwarding();

        IApplicationHostSetup UseStandardSetup(string[] apiVersions);

        IApplicationHostSetup UseDataServices();

        IApplicationHostSetup UseDefaultProvider();

        IApplicationHostSetup UseInternalProvider();

        IApplicationHostSetup UseDataMigrations();

        IApplicationHostSetup UseEndpoints();

        IApplicationHostSetup UseJwtUserInfo();

        IApplicationHostSetup RebuildProviders();

        IApplicationHostSetup UseSwaggerSetup(string[] apiVersions);

        IApplicationHostSetup MapFallbackToFile(string filePath);
    }
}