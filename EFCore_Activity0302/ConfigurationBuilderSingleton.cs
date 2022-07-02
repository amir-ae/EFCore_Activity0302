using Microsoft.Extensions.Configuration;
using System.IO;

namespace EFCore_Activity0301
{
    public sealed class ConfigurationBuilderSingleton
    {
        private static ConfigurationBuilderSingleton? _instance;
        private static readonly object instanceLock = new ();

        private static IConfigurationRoot? _configuration;

        private ConfigurationBuilderSingleton()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = builder.Build();
        }

        public static ConfigurationBuilderSingleton Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (_instance == null) _instance = new ();

                    return _instance;
                }
            }
        }

        public static IConfigurationRoot? ConfigurationRoot
        {
            get
            {
                if (_configuration == null) 
                { 
                    var x = Instance; 
                }
                return _configuration;
            }
        }

    }
}
