namespace CityService.Interface
{
    /// <summary>
    /// Computes scores for cities.
    /// </summary>
    public interface IScorer
    {
        /// <summary>
        /// Computes the score to a city based on the query parameters.
        /// </summary>
        /// <param name="city">The city to compare with.</param>
        /// <param name="queriedName">The name that was queried.</param>
        /// <param name="queriedLatitude">The latitude that was queried.</param>
        /// <param name="queriedLongitude">The longitude that was queried.</param>
        /// <returns>The score between 0 and 1 inclusive, with 1 being the most accurate score.</returns>
        double GetScore(City city, string queriedName, double? queriedLatitude, double? queriedLongitude);
    }
}
