using Nancy.Hosting.Self;
using System;

namespace NancyRestServer
{
    /// <summary>
    /// Entry point into our program. It hosts the REST API for city suggestions.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            AppConfiguration appConfiguration = new AppConfiguration();

            HostConfiguration hostConfiguration = new HostConfiguration
            {
                UrlReservations = new UrlReservations
                {
                    CreateAutomatically = true
                }
            };
            Uri uri = new Uri("http://localhost:" + appConfiguration.Port);
            CustomBootstrapper bootstrapper = new CustomBootstrapper();

            using (NancyHost host = new NancyHost(bootstrapper, hostConfiguration, uri))
            {
                host.Start();

                Console.WriteLine("REST API hosted on port: {0}", appConfiguration.Port);

                Console.WriteLine("Press ENTER to continue...");
                Console.ReadLine();
            }
        }
    }
}
