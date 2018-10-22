using CityService;
using Nancy;
using Nancy.TinyIoc;

namespace NancyRestServer
{
    /// <summary>
    /// A custom bootstrapper to tell Nancy framework how to load our classes based on interfaces.
    /// </summary>
    public class CustomBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            // tells Nancy framework to create a single instance of the implementation for the lifetime of the application
            // to be passed to modules with interface as parameter in constructor
            container.Register<ICityService, CityService.CityService>().AsSingleton();
            container.Register<IAppConfiguration, AppConfiguration>().AsSingleton();
        }
    }
}
