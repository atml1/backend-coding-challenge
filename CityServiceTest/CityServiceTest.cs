using System;
using System.Collections.Generic;
using CityService.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace CityServiceTest
{
    [TestClass]
    public class CityServiceTest
    {
        [TestMethod]
        public void BasicTest()
        {
            City testCity1 = new City { FullName = "Hello", ShortName = "H", Latitude = 12.3, Longitude = 45.6 };
            City testCity2 = new City { FullName = "World", ShortName = "W", Latitude = 32.1, Longitude = 65.4 };
            List<City> testCities = new List<City>(new City[] { testCity1, testCity2 });
            Dictionary<City, double> testScores = new Dictionary<City, double>();
            testScores[testCity1] = 0.5;
            testScores[testCity2] = 0.75;
            string testSerialization = "TestResponse";

            MockCityStorage cityStorage = new MockCityStorage
            {
                GetCitiesStartsWithCalledCount = 0,
                GetCitiesStartsWithParameter = null,
                CitiesToReturn = testCities
            };
            MockScorer scorer = new MockScorer
            {
                GetScoreCalledCount = 0,
                GetScoreCalledParameters = new List<MockScorer.Parameter>(),
                ScoresToReturn = testScores
            };
            MockSerializer serializer = new MockSerializer
            {
                SerializeCalledCount = 0,
                SerializeSuggestions = null,
                SerializationToReturn = testSerialization
            };

            CityService.CityService cityService = new CityService.CityService(cityStorage, scorer, serializer);

            // test the bad parameters
            string response = cityService.AutoComplete(null, null, null, 0);
            Assert.AreEqual("q not specified", response);
            response = cityService.AutoComplete("hello", -91, null, 0);
            Assert.AreEqual("latitude should be between -90 and 90", response);
            response = cityService.AutoComplete("hello", 91, null, 0);
            Assert.AreEqual("latitude should be between -90 and 90", response);
            response = cityService.AutoComplete("hello", 45, -181, 0);
            Assert.AreEqual("longitude should be between -180 and 180", response);
            response = cityService.AutoComplete("hello", 45, 181, 0);
            Assert.AreEqual("longitude should be between -180 and 180", response);

            // test the general case
            response = cityService.AutoComplete("a", 45, 32, 5);
            Assert.AreEqual(testSerialization, response);
            Assert.AreEqual(1, cityStorage.GetCitiesStartsWithCalledCount);
            Assert.AreEqual("a", cityStorage.GetCitiesStartsWithParameter);
            Assert.AreEqual(2, scorer.GetScoreCalledCount);
            Assert.AreEqual(testCity1, scorer.GetScoreCalledParameters[0].GetScoreCalledCity); // testCity1 is first since this is the order from the cityStorage
            Assert.AreEqual("a", scorer.GetScoreCalledParameters[0].GetScoreCalledName);
            Assert.AreEqual(45, scorer.GetScoreCalledParameters[0].GetScoreCalledLatitude);
            Assert.AreEqual(32, scorer.GetScoreCalledParameters[0].GetScoreCalledLongitude);
            Assert.AreEqual(testCity2, scorer.GetScoreCalledParameters[1].GetScoreCalledCity);
            Assert.AreEqual("a", scorer.GetScoreCalledParameters[1].GetScoreCalledName);
            Assert.AreEqual(45, scorer.GetScoreCalledParameters[1].GetScoreCalledLatitude);
            Assert.AreEqual(32, scorer.GetScoreCalledParameters[1].GetScoreCalledLongitude);
            Assert.AreEqual(1, serializer.SerializeCalledCount);
            Assert.AreEqual(2, serializer.SerializeSuggestions.Count);
            Assert.AreEqual(testCity2.FullName, serializer.SerializeSuggestions[0].Name); // sorted from highest to lower, so testCity2 first
            Assert.AreEqual(testCity2.Latitude.ToString(), serializer.SerializeSuggestions[0].Latitude);
            Assert.AreEqual(testCity2.Longitude.ToString(), serializer.SerializeSuggestions[0].Longitude);
            Assert.AreEqual(testScores[testCity2], serializer.SerializeSuggestions[0].Score);
            Assert.AreEqual(testCity1.FullName, serializer.SerializeSuggestions[1].Name);
            Assert.AreEqual(testCity1.Latitude.ToString(), serializer.SerializeSuggestions[1].Latitude);
            Assert.AreEqual(testCity1.Longitude.ToString(), serializer.SerializeSuggestions[1].Longitude);
            Assert.AreEqual(testScores[testCity1], serializer.SerializeSuggestions[1].Score);

            // reset the mock objects
            cityStorage.GetCitiesStartsWithCalledCount = 0;
            cityStorage.GetCitiesStartsWithParameter = null;
            scorer.GetScoreCalledCount = 0;
            scorer.GetScoreCalledParameters.Clear();
            serializer.SerializeCalledCount = 0;
            serializer.SerializeSuggestions = null;

            // test the case where the city service might limit results (testCity2 has higher score so that one is kept)
            response = cityService.AutoComplete("a", 45, 32, 1);
            Assert.AreEqual(testSerialization, response);
            Assert.AreEqual(1, cityStorage.GetCitiesStartsWithCalledCount);
            Assert.AreEqual("a", cityStorage.GetCitiesStartsWithParameter);
            Assert.AreEqual(2, scorer.GetScoreCalledCount);
            Assert.AreEqual(testCity1, scorer.GetScoreCalledParameters[0].GetScoreCalledCity); // testCity1 is first since this is the order from the cityStorage
            Assert.AreEqual("a", scorer.GetScoreCalledParameters[0].GetScoreCalledName);
            Assert.AreEqual(45, scorer.GetScoreCalledParameters[0].GetScoreCalledLatitude);
            Assert.AreEqual(32, scorer.GetScoreCalledParameters[0].GetScoreCalledLongitude);
            Assert.AreEqual(testCity2, scorer.GetScoreCalledParameters[1].GetScoreCalledCity);
            Assert.AreEqual("a", scorer.GetScoreCalledParameters[1].GetScoreCalledName);
            Assert.AreEqual(45, scorer.GetScoreCalledParameters[1].GetScoreCalledLatitude);
            Assert.AreEqual(32, scorer.GetScoreCalledParameters[1].GetScoreCalledLongitude);
            Assert.AreEqual(1, serializer.SerializeCalledCount);
            Assert.AreEqual(1, serializer.SerializeSuggestions.Count);
            Assert.AreEqual(testCity2.FullName, serializer.SerializeSuggestions[0].Name); // sorted from highest to lower, so testCity2 is the only one
            Assert.AreEqual(testCity2.Latitude.ToString(), serializer.SerializeSuggestions[0].Latitude);
            Assert.AreEqual(testCity2.Longitude.ToString(), serializer.SerializeSuggestions[0].Longitude);
            Assert.AreEqual(testScores[testCity2], serializer.SerializeSuggestions[0].Score);
        }

        private class MockCityStorage : ICityStorage
        {
            public int GetCitiesStartsWithCalledCount { get; set; }
            public string GetCitiesStartsWithParameter { get; set; }

            public IEnumerable<City> CitiesToReturn { get; set; }

            public IEnumerable<City> GetCitiesStartsWith(string beginning)
            {
                GetCitiesStartsWithParameter = beginning;
                GetCitiesStartsWithCalledCount++;
                return CitiesToReturn;
            }
        }

        private class MockScorer : IScorer
        {
            public int GetScoreCalledCount { get; set; }
            public List<Parameter> GetScoreCalledParameters { get; set; }

            public Dictionary<City, double> ScoresToReturn { get; set; }

            public double GetScore(City city, string queriedName, double? queriedLatitude, double? queriedLongitude)
            {
                GetScoreCalledParameters.Add(new Parameter
                {
                    GetScoreCalledCity = city,
                    GetScoreCalledName = queriedName,
                    GetScoreCalledLatitude = queriedLatitude,
                    GetScoreCalledLongitude = queriedLongitude
                });
                GetScoreCalledCount++;
                return ScoresToReturn.ContainsKey(city)? ScoresToReturn[city] : 0;
            }

            public class Parameter
            {
                public City GetScoreCalledCity { get; set; }
                public string GetScoreCalledName { get; set; }
                public double? GetScoreCalledLatitude { get; set; }
                public double? GetScoreCalledLongitude { get; set; }
            }
        }

        private class MockSerializer : ISerializer
        {
            public int SerializeCalledCount { get; set; }
            public List<Suggestion> SerializeSuggestions { get; set; }

            public string SerializationToReturn { get; set; }

            public string Serialize(IEnumerable<Suggestion> suggestions)
            {
                SerializeSuggestions = suggestions.ToList();
                SerializeCalledCount++;
                return SerializationToReturn;
            }
        }
    }
}
