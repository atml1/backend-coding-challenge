using System.Collections.Generic;

namespace CityService.Interface
{
    /// <summary>
    /// Store cities to be looked up by the characters that the name starts with.
    /// </summary>
    public interface ICityStorage
    {
        /// <summary>
        /// Returns all possible cities in the city storage that begin with the specified characters.
        /// </summary>
        /// <param name="beginning">The first character of the names to look up.</param>
        /// <returns>All cities in the city storage that begin with the specified characters.</returns>
        IEnumerable<City> GetCitiesStartsWith(string beginning);
    }
}
