using System.Collections.Generic;
using CityService.Interface;

namespace CityService.Implementation
{
    /// <summary>
    /// An implementation of ICityStorage that loads and parses city data from a file.
    /// </summary>
    public class CityStorage : ICityStorage
    {
        /// <summary>
        /// The parser to get the city data from.
        /// </summary>
        public ICityParser Parser { get; set; }

        /// <summary>
        /// The word storage to be able to look up city names by first letters quickly.
        /// </summary>
        public IWordStorage WordStorage { get; set; }

        private Dictionary<string, List<City>> _cities = new Dictionary<string, List<City>>(); // multiple cities might have the same name

        // need to lock on the usage of the WordStorage and the _cities dictionary since may be accessed from different threads
        private object _dataLock = new object();

        public IEnumerable<City> GetCitiesStartsWith(string beginning)
        {
            lock (_dataLock)
            {
                IEnumerable<string> cityNames = WordStorage.AutoComplete(beginning.ToLowerInvariant());
                List<City> cities = new List<City>();
                foreach (string cityName in cityNames)
                {
                    foreach (City city in _cities[cityName])
                    {
                        cities.Add(city);
                    }
                }
                return cities;
            }
        }

        public void LoadData()
        {
            lock (_dataLock)
            {
                WordStorage.Clear();
            }

            foreach (City city in Parser.LoadCities())
            {
                AddCity(city);
            }
        }

        public void AddCity(City city)
        {
            lock (_dataLock)
            {
                if (!_cities.ContainsKey(city.ShortName))
                {
                    WordStorage.Add(city.ShortName);
                    _cities[city.ShortName] = new List<City>();
                }
                _cities[city.ShortName].Add(city);
            }
        }
    }
}
