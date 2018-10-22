namespace CityService.Interface
{
    /// <summary>
    /// Contains data about a city.
    /// </summary>
    public class City
    {
        /// <summary>
        /// The name of the city. E.g. "London".
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// The unique name of the city. E.g. "London, ON, Canada"
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// The latitude of the city.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// The longitude of the city.
        /// </summary>
        public double Longitude { get; set; }
    }
}
