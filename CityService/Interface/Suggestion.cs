using System.Runtime.Serialization;

namespace CityService.Interface
{
    /// <summary>
    /// Contains data about a suggestion of a city.
    /// </summary>
    [DataContract]
    public class Suggestion
    {
        /// <summary>
        /// The unique name of the city. E.g. "London, ON, Canada"
        /// </summary>
        [DataMember(Name="name", Order=1)] public string Name { get; set; }

        /// <summary>
        /// The latitude of the city.
        /// </summary>
        [DataMember(Name="latitude", Order=2)] public string Latitude { get; set; }

        /// <summary>
        /// The longitude of the city.
        /// </summary>
        [DataMember(Name = "longitude", Order=3)] public string Longitude { get; set; }

        /// <summary>
        /// The score of the city based on the queried parameters.
        /// </summary>
        [DataMember(Name = "score", Order=4)] public double Score { get; set; }
    }
}
