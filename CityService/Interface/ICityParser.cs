using System.Collections.Generic;

namespace CityService.Interface
{
    /// <summary>
    /// Parses cities from a source.
    /// </summary>
    public interface ICityParser
    {
        /// <summary>
        /// Loads the cities from the source into memory.
        /// </summary>
        /// <returns>The list of cities that were loaded.</returns>
        IEnumerable<City> LoadCities();
    }
}
