using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Undersoft.SDK.Service.Configuration;

using Undersoft.SDK.Service.Configuration.Options;

public static class ServiceConfigurationHelper
{
    public static IConfigurationRoot BuildConfiguration(
        ConfigurationOptions options = null,
        Action<IConfigurationBuilder> builderAction = null
    )
    {
        options = options ?? new ConfigurationOptions();
        options.EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        if (options.BasePath == null)
        {
            options.BasePath = Directory.GetCurrentDirectory();
        }

        IConfigurationBuilder builder = new ConfigurationBuilder()
            .SetBasePath(options.BasePath)
            .AddJsonFile($"{options.GeneralFileName}.json", optional: true, reloadOnChange: true);
        if (options.OptionalFileNames != null && options.OptionalFileNames.Length > 0)
            options.OptionalFileNames.ForEach(
                s => builder.AddJsonFile($"{s}.json", optional: true, reloadOnChange: true)
            );

        if (!(options.EnvironmentName == null))
        {
            builder = builder.AddJsonFile(
                $"{options.GeneralFileName}.{options.EnvironmentName}.json",
                optional: true,
                reloadOnChange: true
            );
            if (options.OptionalFileNames != null && options.OptionalFileNames.Length > 0)
                options.OptionalFileNames.ForEach(
                    s =>
                        builder.AddJsonFile(
                            $"{s}.{options.EnvironmentName}.json",
                            optional: true,
                            reloadOnChange: true
                        )
                );
        }

        if (options.EnvironmentName == "Development")
        {
            if (options.UserSecretsId != null)
            {
                builder.AddUserSecrets(options.UserSecretsId);
            }
            else if (options.UserSecretsAssembly != null)
            {
                builder.AddUserSecrets(options.UserSecretsAssembly, true);
            }
        }

        builder = builder.AddEnvironmentVariables(options.EnvironmentVariablesPrefix);

        if (options.CommandLineArgs != null)
        {
            builder = builder.AddCommandLine(options.CommandLineArgs);
        }

        builderAction?.Invoke(builder);

        return builder.Build();
    }
}
