using System;
using System.Configuration;

namespace NancyRestServer
{
    /// <summary>
    /// Implementation of IAppConfiguration that loads the configuration information from the app.config file.
    /// </summary>
    public class AppConfiguration : IAppConfiguration
    {
        public int Port { get; private set; }
        public int DefaultMaxResponseCount { get; private set; }

        public AppConfiguration()
        {
            string portText = "port";
            int port;
            if (!int.TryParse(ConfigurationManager.AppSettings[portText], out port))
            {
                Console.Error.WriteLine("\"{0}\" in config file not an integer. Defaulting to 80.", portText);
                port = 80;
            }
            Port = port;

            string defaultMaxResponseCountText = "defaultMaxResponseCount";
            int defaultMaxResponseCount;
            if (!int.TryParse(ConfigurationManager.AppSettings[defaultMaxResponseCountText], out defaultMaxResponseCount))
            {
                Console.Error.WriteLine("\"{0}\" in config file not an integer. Defaulting to 5.", defaultMaxResponseCountText);
                defaultMaxResponseCount = 5;
            }
            DefaultMaxResponseCount = defaultMaxResponseCount;
        }
    }
}
