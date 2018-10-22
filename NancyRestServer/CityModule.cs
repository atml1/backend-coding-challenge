using CityService;
using Nancy;

namespace NancyRestServer
{
    /// <summary>
    /// A module to tell nancy how to route calls related to city suggestions.
    /// </summary>
    public class CityModule : NancyModule
    {
        public CityModule(IAppConfiguration appConfiguration, ICityService cityService) : base("/cities")
        {
            Get("/suggestions", (parameters) =>
            {
                string query = Request.Query.Q;
                double? latitude = Request.Query.Latitude;
                double? longitude = Request.Query.Longitude;
                int? maxCount = Request.Query.MaxCount;
                if (maxCount == null)
                {
                    maxCount = appConfiguration.DefaultMaxResponseCount;
                }
                else if (maxCount == -1)
                {
                    maxCount = int.MaxValue;
                }
                return cityService.AutoComplete(query, latitude, longitude, maxCount.Value);
            });
        }
    }
}
