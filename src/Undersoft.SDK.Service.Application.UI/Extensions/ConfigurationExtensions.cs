using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace Microsoft.Extensions.Configuration;

internal static class ConfigurationExtensions
{
    public static NameValueCollection GetEnvironmentInformation(this IConfiguration configuration)
    {
        var nv = new NameValueCollection
        {
            ["TimeStamp"] = DateTime.Now.ToString(),
            ["MachineName"] = Environment.MachineName,
            ["AppDomainName"] = AppDomain.CurrentDomain.FriendlyName,

            ["OS"] = GetOS(),
            ["OSArchitecture"] = RuntimeInformation.OSArchitecture.ToString(),
            ["ProcessArchitecture"] = RuntimeInformation.ProcessArchitecture.ToString(),
            ["Framework"] = RuntimeInformation.FrameworkDescription
        };

        var userName = configuration.GetUserName();
        if (!string.IsNullOrEmpty(userName))
        {
            nv["UserName"] = userName;
        }

        var env = configuration.GetEnvironmentName();
        if (!string.IsNullOrEmpty(env))
        {
            nv["EnvironmentName"] = env;
        }

        var iis = configuration.GetIISPath();
        if (!string.IsNullOrEmpty(iis))
        {
            nv["IISRootPath"] = iis;
        }

        var vs = configuration.GetVisualStudioVersion();
        if (!string.IsNullOrEmpty(vs))
        {
            nv["VSIDE"] = vs;
        }
        return nv;
    }

    [ExcludeFromCodeCoverage]
    public static string GetOS()
    {
        string? os = null;
        if (string.IsNullOrEmpty(os))
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                os = RuntimeInformation.OSDescription;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                os = $"OSX";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            {
                os = "FreeBSD";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                os = $"Linux";
            }
        }
        return os ?? "Unknown";
    }

    public static string GetUserName(this IConfiguration config, string defaultValue = "")
    {
        var userName = config.GetValue<string>("USERNAME");

        if (string.IsNullOrEmpty(userName))
        {
            userName = config.GetValue<string>("LOGNAME");
        }
        return userName ?? defaultValue;
    }

    public static string GetEnvironmentName(this IConfiguration config, string defaultValue = "")
    {
        return config.GetValue<string>("ASPNETCORE_ENVIRONMENT") ?? defaultValue;
    }

    public static string GetIISPath(this IConfiguration config, string defaultValue = "")
    {
        return config.GetValue<string>("ASPNETCORE_IIS_PHYSICAL_PATH") ?? defaultValue;
    }

    public static string GetVisualStudioVersion(this IConfiguration config, string defaultValue = "")
    {
        var edition = config.GetValue<string>("VisualStudioEdition");
        var version = config.GetValue<string>("VisualStudioVersion");

        var ret = $"{edition} {version}";
        if (ret == " ")
        {
            ret = defaultValue;
        }
        return ret;
    }
}
