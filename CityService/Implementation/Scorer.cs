using System;
using CityService.Interface;

namespace CityService.Implementation
{
    /// <summary>
    /// Implementation of IScorer that computes score by looking at how many letters are provided, and
    /// how far away the provided latitudes and longitudes are. If no latitudes or longitude is provided,
    /// it assumes perfect match in that direction.
    /// </summary>
    public class Scorer : IScorer
    {
        public double GetScore(City city, string queriedName, double? queriedLatitude, double? queriedLongitude)
        {
            // check number of letters correct compared to number of letters in the entire name
            double letterRatio = queriedName.Length / (double)city.ShortName.Length;

            // check distance; note: this is not the actual distance, but just based on what the use wrote in each direction
            double differenceLongitude = 0;
            if (queriedLongitude.HasValue) // assume longitude given: -180 -> 180
            {
                // find the absolute difference between the two values: 0->360
                differenceLongitude = Math.Abs(city.Longitude - queriedLongitude.Value);

                // find it relative to half a sphere since we can shorten the distance by going the other way around sphere: 0 -> 180
                if (differenceLongitude > 180)
                {
                    differenceLongitude = 360 - differenceLongitude;
                }
            }
            double differenceLatitude = 0;
            if (queriedLatitude.HasValue) // assume latitude given: -90 -> 90
            {
                // find the absolute difference between the two values: 0->180
                differenceLatitude = Math.Abs(city.Latitude - queriedLatitude.Value);
            }
            double differentSum = differenceLatitude + differenceLongitude;
            double distanceRatio = 1 - differentSum / (360);

            // return the average of the two
            return (letterRatio + distanceRatio) / 2;
        }
    }
}
