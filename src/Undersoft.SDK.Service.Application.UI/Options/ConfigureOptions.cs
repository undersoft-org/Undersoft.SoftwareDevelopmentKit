using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public class ConfigureOptions<TOption> : ConfigureFromConfigurationOptions<TOption> where TOption : class
{
    public ConfigureOptions(IConfiguration config)
        : base(config.GetSection(typeof(TOption).Name))
    {

    }
}
