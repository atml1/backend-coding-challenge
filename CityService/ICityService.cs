namespace CityService
{
    /// <summary>
    /// Service to access data stored about cities.
    /// </summary>
    public interface ICityService
    {
        /// <summary>
        /// Suggests different stored cities matching the specified parameters.
        /// </summary>
        /// <param name="q">The beginning of the name of the city.</param>
        /// <param name="latitudeNullable">The approximate latitude of the city to narrow the results.</param>
        /// <param name="longitudeNullable">The approximate longitude of the city to narrow the results.</param>
        /// <param name="maxResponseCount">The number of cities to return.</param>
        /// <returns>A string representation of the suggested cities.</returns>
        string AutoComplete(string q, double? latitudeNullable, double? longitudeNullable, int maxResponseCount);
    }
}
