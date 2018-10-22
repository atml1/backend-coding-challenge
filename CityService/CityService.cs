using CityService.Implementation;
using CityService.Interface;
using System;
using System.Collections.Generic;

namespace CityService
{
    public class CityService : ICityService
    {
        private ICityStorage _cityStorage;
        private IScorer _scorer;
        private ISerializer _serializer;

        public CityService()
        {
            CityStorage cityStorage = new CityStorage
            {
                WordStorage = new WordTree()
            };
            cityStorage.LoadData("cities_canada-usa.tsv");
            _cityStorage = cityStorage;

            _scorer = new Scorer();
            _serializer = new JsonSerializer();
        }

        public string AutoComplete(string q, double? latitudeNullable, double? longitudeNullable, int maxResponseCount)
        {
            if (q == null)
            {
                return "q not specified.";
            }
            if (latitudeNullable.HasValue && (latitudeNullable.Value < -90 || latitudeNullable.Value > 90))
            {
                return "latitude should be between -90 and 90";
            }
            if (longitudeNullable.HasValue && (longitudeNullable.Value < -180 || longitudeNullable.Value > 180))
            {
                return "longitude should be between -180 and 180";
            }

            IEnumerable<City> cities = _cityStorage.GetCitiesStartsWith(q);
            List<Suggestion> suggestions = new List<Suggestion>();
            foreach (City city in cities)
            {
                double score = _scorer.GetScore(city, q, latitudeNullable, longitudeNullable);
                suggestions.Add(new Suggestion
                {
                    Name = city.FullName,
                    Latitude = city.Latitude.ToString(),
                    Longitude = city.Longitude.ToString(),
                    Score = score
                });
            }
            suggestions.Sort((s1, s2) => { return s2.Score.CompareTo(s1.Score); }); // sorting from highest to lowest
            if (maxResponseCount < suggestions.Count)
            {
                suggestions.RemoveRange(maxResponseCount, suggestions.Count - maxResponseCount);
            }

            return _serializer.Serialize(suggestions);
        }
    }
}
