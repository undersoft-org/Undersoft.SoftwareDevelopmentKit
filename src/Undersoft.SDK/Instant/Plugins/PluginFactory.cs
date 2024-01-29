namespace Undersoft.SDK.Instant.Plugins
{
    using System.Reflection;
    using System.Runtime.Loader;
    using Undersoft.SDK.Instant.Proxies;
    using Undersoft.SDK.Series;

    public static class PluginFactory
    {
        public static ISeries<Plugin> Cache = new Registry<Plugin>(true);         
    }
}
