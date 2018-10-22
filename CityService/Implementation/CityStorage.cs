using System;
using System.Collections.Generic;
using CityService.Interface;
using System.IO;

namespace CityService.Implementation
{
    /// <summary>
    /// An implementation of ICityStorage that loads and parses city data from a file.
    /// </summary>
    public class CityStorage : ICityStorage
    {
        /// <summary>
        /// The word storage to be able to look up city names by first letters quickly.
        /// </summary>
        public IWordStorage WordStorage { get; set; }

        private Dictionary<string, List<City>> _cities = new Dictionary<string, List<City>>(); // multiple cities might have the same name

        // need to lock on the usage of the WordStorage and the _cities dictionary since may be accessed from different threads
        private object _dataLock = new object();

        // constants to verify the data in the file is correct
        private const int NAME_INDEX = 1;
        private const int ADMIN1_INDEX = 10;
        private const int COUNTRY_INDEX = 8;
        private const int LAT_INDEX = 4;
        private const int LONG_INDEX = 5;
        private readonly Heading[] expectedHeadings = new Heading[]
            {
                new Heading(NAME_INDEX, "name"),
                new Heading(ADMIN1_INDEX, "admin1"),
                new Heading(COUNTRY_INDEX, "country"),
                new Heading(LAT_INDEX, "lat"),
                new Heading(LONG_INDEX, "long")
            };

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

        public void LoadData(string filepath)
        {
            try
            {
                lock (_dataLock)
                {
                    WordStorage.Clear();
                }

                bool readHeader = false;
                using (FileStream fs = new FileStream(filepath, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        int lineNumber = 0;
                        string line = sr.ReadLine();
                        while (line != null)
                        {
                            lineNumber++;
                            string[] lineElements = line.Split(new char[] { '\t' });

                            if (!readHeader)
                            {
                                VerifyHeader(lineElements);
                                readHeader = true;
                            }
                            else
                            {
                                City city = ParseLineElements(lineElements, lineNumber);
                                AddCity(city);
                            }

                            line = sr.ReadLine();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Failed to parse city data from file: {0}", e.Message);
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

        private void VerifyHeader(string[] headerLine)
        {
            // check the header to see if the columns we expect are in the correct position

            foreach (Heading expectedHeading in expectedHeadings)
            {
                if (headerLine.Length < expectedHeading.Index || !expectedHeading.Label.Equals(headerLine[expectedHeading.Index]))
                {
                    throw new ArgumentException(String.Format("Incorrect heading. Expected '{0}' at column '{1}'",
                            expectedHeading.Label, expectedHeading.Index));
                }
            }
        }

        private City ParseLineElements(string[] lineElements, int lineNumber)
        {
            string name = ParseString("name", NAME_INDEX, lineElements, lineNumber);
            string admin1 = ParseString("admin1", ADMIN1_INDEX, lineElements, lineNumber);
            string country = ParseString("country", COUNTRY_INDEX, lineElements, lineNumber);
            double latitude = ParseDouble("latitude", LAT_INDEX, lineElements, lineNumber);
            double longitude = ParseDouble("longitude", LONG_INDEX, lineElements, lineNumber);

            if (country.ToLowerInvariant().Equals("ca"))
            {
                admin1 = CanadaCodeLookup(admin1);
            }

            string fullName = String.Format("{0}, {1}, {2}", name, admin1, country);
            return new City
            {
                ShortName = name.ToLowerInvariant(), // city names with different capitalization should still be the same city
                FullName = fullName,
                Latitude = latitude,
                Longitude = longitude
            };
        }

        private static string CanadaCodeLookup(string admin1)
        {
            // from http://download.geonames.org/export/dump/admin1CodesASCII.txt
            switch (admin1)
            {
                case "01":
                    return "AB";
                case "02":
                    return "BC";
                case "03":
                    return "MB";
                case "04":
                    return "NB";
                case "05":
                    return "NL";
                case "07":
                    return "NS";
                case "08":
                    return "ON";
                case "09":
                    return "PE";
                case "10":
                    return "QC";
                case "11":
                    return "SK";
                case "12":
                    return "YT";
                case "13":
                    return "NT";
                case "14":
                    return "NU";
                default:
                    throw new ArgumentException(String.Format("Could not parse admin1 '{0}' for canada", admin1));
            }
        }

        private static string ParseString(string name, int index, string[] lineElements, int lineNumber)
        {
            if (lineElements.Length < index)
            {
                throw new ArgumentException(String.Format("Could not find {0} at column {1} on line {2}", name, index, lineNumber));
            }
            return lineElements[index];
        }

        private static double ParseDouble(string name, int index, string[] lineElements, int lineNumber)
        {
            if (lineElements.Length < index)
            {
                throw new ArgumentException(String.Format("Could not find {0} at column {1} on line {2}", name, index, lineNumber));
            }

            double value;
            if (!double.TryParse(lineElements[index], out value))
            {
                throw new ArgumentException(String.Format("Could not parse {0} {1} on line {2}", name, lineElements[index], lineNumber));
            }

            return value;
        }

        private class Heading
        {
            public int Index { get; private set; }
            public string Label { get; private set; }

            public Heading(int index, string label)
            {
                Index = index;
                Label = label;
            }
        }
    }
}
